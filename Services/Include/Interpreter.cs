using System.Text.Json;
using SkiaSharp;
using WebGal.Global;
using WebGal.Types;
using WebGal.Libs.Base;
using WebGal.MeoInterpreter;

namespace WebGal.Services.Include;

public class Interpreter
{
	private readonly SceneManager _sceneManager;
	private readonly ResourceManager _resourceManager;
	private readonly MoeInterpreter _moeInterpreter;

	private readonly Queue<string> _unloadedResPackName = new();

	/// <summary>
	/// N elements, range [0, N)
	/// </summary>
	private readonly List<string> _nodesList = new();

	/// <summary>
	/// N elements, range [0, N)
	/// </summary>
	private readonly List<string> _nodeSceneList = new();


	/// <summary>
	/// N elements, range [0, N)
	/// </summary>
	private readonly Dictionary<string, int> _nodes = new();

	/// <summary>
	/// N elements, range [0, N)
	/// </summary>
	private readonly Dictionary<string, int> _nodeScenes = new();

	/// <summary>
	/// N nodes , NodeId in [0, N).
	/// M Scenes, SceneId in [0, M)
	/// </summary>
	private (int NodeId, int SceneId) _parsingPointer;

	public Interpreter(SceneManager sceneManager, ResourceManager resourceManager)
	{
		_sceneManager = sceneManager;
		_resourceManager = resourceManager;
		_moeInterpreter = new(_resourceManager);
	}

	public void Clear()
	{
		_unloadedResPackName.Clear();

		_nodes.Clear();
		_nodeScenes.Clear();

		_nodesList.Clear();
		_nodeSceneList.Clear();

		_parsingPointer = (0, 0);
	}

	/// <summary>
	/// 开始执行解释流程，唯一公共对外口
	/// </summary>
	/// <returns></returns>
	public async Task ParsingNextSceneAsync()
	{
		// 如果当前节点有未解释的场景，尝试解释下一个场景，否则解释下一个节点
		if (_nodeSceneList.Count != 0)
		{
			if (_parsingPointer.SceneId < _nodeSceneList.Count - 1)
				_parsingPointer.SceneId++;
			else if (_parsingPointer.NodeId < _nodesList.Count - 1 && _parsingPointer.SceneId >= _nodeSceneList.Count - 1)
				SetNewNodeId(_parsingPointer.NodeId + 1);
		}

		// 如果当前节点从未被解释过，那么处理该节点
		if (_nodeSceneList.Count == 0)
			await ProcessNodeAsync();


		Console.WriteLine($"{_parsingPointer.NodeId}:{_parsingPointer.SceneId}");
		await ProcessResourceAsync();
		ProcessSceneAsync();
	}

	private void SetNewNodeId(int id)
	{
		_parsingPointer = (id, 0);
		_nodeSceneList.Clear();
		_nodeScenes.Clear();
	}

	public void SetNode(string nodeName) => SetNewNodeId(_nodes[nodeName]);
	public void JumpToNode(string nodeName) => SetNewNodeId(_nodes[nodeName]);

	/// <summary>
	/// 当一个游戏呗加载时，拉取当前游戏的所有脚本文件
	/// </summary>
	/// <param name="gameName"></param>
	/// <returns></returns>
	public async Task SetGameAsync(string gameName)
	{
		var gameBase = "Data/" + gameName + "/";
		_resourceManager.basePath = gameBase;
		await _resourceManager.PullScriptAsync();
		await _resourceManager.PullScriptAsync("elf", "/TestMoe1/main.elf");

		var mainObj = JsonSerializer.Deserialize<GameStructure>(_resourceManager.GetScript(), JsonConfig.Options);

		for (int nodeId = 0; nodeId < mainObj.NodeURLs.Count; nodeId++)
		{
			UrlStructure node = mainObj.NodeURLs[nodeId];
			_nodesList.Add(node.Name);
			_nodes[node.Name] = nodeId;
		}

		List<Task> tasks = [.. mainObj.NodeURLs.Select(script => _resourceManager.PullScriptAsync(script.Name, script.URL))];
		await Task.WhenAll(tasks);

		//! Test Moe Script
		// Console.WriteLine(_resourceManager.GetScript("elf"));
		await _moeInterpreter.LoadELF(_resourceManager.GetScript("elf"));
		_moeInterpreter.Dump();
	}

	/// <summary>
	/// 异步方式加载游戏的一个场景的流程
	/// </summary>
	/// <param name="nodeName"></param>
	/// <returns></returns>
	/// <exception cref="Exception">节点值非法(节点值未默认)</exception>
	private async Task ProcessNodeAsync()
	{
		var nodeName = _nodesList[_parsingPointer.NodeId];
		var nowNode = JsonSerializer.Deserialize<NodeStructure>(_resourceManager.GetScript(nodeName), JsonConfig.Options);

		if (nowNode == default)
			throw new Exception("No Node");

		// 根据场景添加该场景所有需要的资源
		List<Task> tasks = [];
		var resourseTasks = nowNode.ResouresPackURL?.Select(resourcePack =>
			{
				_unloadedResPackName.Enqueue(resourcePack.Name);
				return _resourceManager.PullScriptAsync(resourcePack.Name, resourcePack.URL);
			});
		if (resourseTasks is not null)
			tasks.AddRange(resourseTasks);

		// 根据场景的SceneUrl来获取Scene脚本
		if (nowNode.SceneURLs is not null)
			for (int sceneId = 0; sceneId < nowNode.SceneURLs.Count; sceneId++)
			{
				UrlStructure scene = nowNode.SceneURLs[sceneId];
				_nodeSceneList.Add(scene.Name);
				_nodeScenes[scene.Name] = sceneId;
				tasks.Add(_resourceManager.PullScriptAsync(scene.Name, scene.URL));
			}

		await Task.WhenAll(tasks);
	}


	/// <summary>
	/// 同步处理所有需要加载的场景，取每个图层需要的资源，然后通过调用 PackLayer(layerName)来打包图层，
	/// 所有图层打包完成后调用 PackScene(sceneName) 来将图层打包，并且放入资源管理器
	/// </summary>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	private void ProcessSceneAsync()
	{
		var sceneName = _nodeSceneList[_parsingPointer.SceneId];

		// 先将场景名字放入队列，再添加名字到场景的映射，减少资源占用
		_sceneManager.SceneNameList.Enqueue(sceneName);
		if (_sceneManager.ContainsScene(sceneName))
			return;

		string sceneScript = _resourceManager.GetScript(sceneName);

		SceneStructure sceneStructure = JsonSerializer.Deserialize<SceneStructure>(sceneScript, JsonConfig.Options);

		if (sceneStructure.Layers is null)
			throw new Exception("No Scene Layer");

		Scene scene = new();

		foreach (var layer in sceneStructure.Layers)
		{
			if (layer.Name is null)
				throw new Exception("Null layer name");
			scene.PushLayer(layer.Name, PackLayer(layer));
		}

		if (sceneStructure.Events is not null)
			foreach (var @event in sceneStructure.Events)
			{
				if (@event.Triger is null)
					throw new Exception("No triger");
				if (@event.Action is null)
					throw new Exception("No action");

				// SetLayerAction(@event.Triger.Value, @event.Action, scene);

				var (triger, actions) = (@event.Triger.Value, @event.Action);

				if (triger.LayerName is not null)
					scene.RegitserMouseAction(triger.LayerName, triger.MouseEvent, actions);
				else
					scene.RegitserMouseDefaultAction(triger.MouseEvent, actions);
			}
		_sceneManager.PushScene(sceneName, scene);
	}

	/// <summary>
	/// 根据 json 中的描述包装一个图层
	/// </summary>
	/// <param name="layerStructure">传入图层 json 描述</param>
	/// <exception cref="Exception">节点相关资源未被正常加载</exception>
	private Libs.Base.Layer PackLayer(LayerStructure layerStructure)
	{
		if (layerStructure.Name is null)
			throw new Exception("layer dont have name");

		Libs.Base.Layer layer = new()
		{
			Pos = layerStructure.Position
		};

		var (width, height) = layerStructure.WinSize;

		if (layerStructure.LayerType is null)
			throw new Exception("Unknow Layer Type");

		// 插入图片
		if (layerStructure.LayerType == "image")
		{
			if (layerStructure.Image is null)
				throw new Exception($"Layer {layerStructure.Name} image Not Found");

			var image = _resourceManager.GetImage(layerStructure.Image);

			if (layerStructure.WinSize == default)
				(width, height) = (image.Width, image.Height);
			layer.WinSize = new(width, height);
			layer.BackGroundSKBitmap = new(width, height, LayerConfig.DefaultColorType, LayerConfig.DefaultAlphaType);

			var cutPosition = layerStructure.CutPosition;
			var cutWinSize = layerStructure.CutWinSize;
			if (cutWinSize == default)
				cutWinSize = new(width, height);

			using SKCanvas canvas = new(layer.BackGroundSKBitmap);
			canvas.DrawBitmap(
				image,
				new SKRectI(
					cutPosition.X,
					cutPosition.Y,
					cutPosition.X + cutWinSize.Width,
					cutPosition.Y + cutWinSize.Height
				),
				new SKRectI(
					0,
					0,
					cutWinSize.Width,
					cutWinSize.Height
				)
			);
			canvas.Flush();
		}

		// 绘制简单图形
		if (layerStructure.LayerType == "shape")
		{
			layer.WinSize = new(width, height);
			layer.BackGroundSKBitmap = new(width, height, LayerConfig.DefaultColorType, LayerConfig.DefaultAlphaType);

			using SKCanvas canvas = new(layer.BackGroundSKBitmap);
			canvas.DrawRect(
				new SKRect(0, 0, layer.WinSize.Width, layer.WinSize.Height),
				new SKPaint
				{
					Color = new SKColor(
						layerStructure.ShapeColor.R,
						layerStructure.ShapeColor.G,
						layerStructure.ShapeColor.B,
						layerStructure.ShapeColor.A
					)
				}
			);
			canvas.Flush();
		}

		// 插入文本
		if (layerStructure.LayerType == "text")
		{
			if (layerStructure.Text is null)
				throw new Exception($"Layer {layerStructure.Name} textLayer Not Found");

			var texts = layerStructure.Text;
			foreach (var text in texts)
			{
				if (text.Text is null)
					throw new Exception("No Text Set");
				LayerText layerText = new()
				{
					Text = text.Text,
					Pos = new(text.Offset.X, text.Offset.Y),
					Paint = LayerConfig.DefaultTextPaint,
				};

				var textPaint = text.Paint;
				var textColor = textPaint.Color;

				if (textPaint != default)
				{
					layerText.Paint.FakeBoldText = textPaint.Blod;
					layerText.Paint.IsAntialias = textPaint.Antialias;
					layerText.Paint.TextSize = textPaint.TextSize;
				}
				if (textColor != default)
					layerText.Paint.Color = new SKColor(textColor.R, textColor.G, textColor.B, textColor.A);

				layer.Text.Add(layerText);
			}
		}

		// 加载动画
		if (layerStructure.Animation is not null)
		{
			layer.Anim = new()
			{
				BeginPosition = (FVector)layerStructure.BeginPosition,
				EndPosition = (FVector)layerStructure.EndPosition,
				DelayTime = layerStructure.Time,
				AnimationClass = AnimationRegister.GetAnimation(layerStructure.Animation)
			};
		}

		// 添加属性
		layer.DynamicAttribute = layerStructure.Attribute;
		layer.OriginalAttribute = layerStructure.Attribute;

		return layer;
	}

	/// <summary>
	/// 以异步方式加载未加载队列中的资源文件
	/// </summary>
	/// <returns></returns>
	private async Task ProcessResourceAsync()
	{
		List<Task> tasks = new();
		while (_unloadedResPackName.Count > 0)
		{
			var resourcePackScript = _resourceManager.GetScript(_unloadedResPackName.Dequeue());
			ResouresStructure resouresPack = JsonSerializer.Deserialize<ResouresStructure>(resourcePackScript, JsonConfig.Options);

			if (resouresPack.ImageURL is not null)
				tasks.AddRange(resouresPack.ImageURL.Select(image => _resourceManager.PullImageAsync(image.Name, image.URL)));

			if (resouresPack.AudioURL is not null)
				tasks.AddRange(resouresPack.AudioURL.Select(aduio => _resourceManager.PullAudioAsync(aduio.Name, aduio.URL)));
		}
		await Task.WhenAll(tasks);
	}
}