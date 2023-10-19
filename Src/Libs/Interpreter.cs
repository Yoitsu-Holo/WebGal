using System.Text.Json;
using System.Text.Json.Serialization;
using SkiaSharp;
using WebGal.Global;
using WebGal.Libs.Base;

namespace WebGal.Libs;
public class Interpreter
{
	// private Dictionary<string, List<string>> _nodeName = new();
	private readonly Queue<string> _sceneName = new();
	private readonly Queue<string> _unloadedResPackName = new();
	private readonly SceneManager _sceneManager;
	private readonly ResourceManager _resourceManager;

	// 用于存储每一层Node的信息
	private readonly Stack<(string, IEnumerator<UrlStructure>)> _nodeEnum = new();


	public void Clear()
	{
		_nodeEnum.Clear();
		_sceneName.Clear();
		_unloadedResPackName.Clear();
	}

	// public Interpreter(SceneManager sceneManager, ResourceManager resourceManager, EventManager eveneManager)
	public Interpreter(SceneManager sceneManager, ResourceManager resourceManager)
	{
		_sceneManager = sceneManager;
		_resourceManager = resourceManager;
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
			ResouresStructure resouresPack = JsonSerializer.Deserialize<ResouresStructure>(resourcePackScript);

			if (resouresPack.ImageURL is not null)
				tasks.AddRange(resouresPack.ImageURL.Select(image => _resourceManager.PullImageAsync(image.Name, image.URL)));

			if (resouresPack.AudioURL is not null)
				tasks.AddRange(resouresPack.AudioURL.Select(aduio => _resourceManager.PullAudioAsync(aduio.Name, aduio.URL)));
		}
		await Task.WhenAll(tasks);
	}

	// todo 暂只能一次性加载全部流程,无法指定顺序(单场景测试)
	/// <summary>
	/// 异步方式加载游戏流程(节点树)，
	/// 并且根据节点树设置需要加载的资源（本身不加载资源）
	/// </summary>
	/// <param name="nowNodeName"></param>
	/// <returns></returns>
	/// <exception cref="Exception">节点值非法(节点值未默认)</exception>
	private async Task ProcessNodeAsync()
	{
		while (_nodeEnum.Count != 0)
		{
			var (nowNodeName, nextNodeEnum) = _nodeEnum.Peek();
			var nowNode = JsonSerializer.Deserialize<NodeStructure>(_resourceManager.GetScript(nowNodeName));

			if (nowNode == default)
				throw new Exception("No volume");


			// 添加资源配置脚本，并且将资源加入预加载队列
			var urlTasks = nowNode.ResouresPackURL?.Select(resourcePack =>
				{
					_unloadedResPackName.Enqueue(resourcePack.Name);
					return _resourceManager.PullScriptAsync(resourcePack.Name, resourcePack.URL);
				});
			if (urlTasks is not null)
				await Task.WhenAll(urlTasks);

			if (nowNode.NodeURL is null)
				throw new Exception("empty Node");

			if (nextNodeEnum.MoveNext() == false)
				goto AfterProcess; // 跳转到后处理部分


			var nextNodeUrl = nextNodeEnum.Current;
			await _resourceManager.PullScriptAsync(nextNodeUrl.Name, nextNodeUrl.URL);

			if (nowNode.IsLeaf) // 叶子节点，添加场景
			{
				_sceneName.Enqueue(nextNodeUrl.Name);
				break;
			}
			else
			{
				var nextNode = JsonSerializer.Deserialize<NodeStructure>(_resourceManager.GetScript(nextNodeUrl.Name));
				if (nextNode.NodeURL is null)
					throw new Exception("empty Node");
				_nodeEnum.Push((nextNodeUrl.Name, nextNode.NodeURL.GetEnumerator()));

				// 添加音频资源列表
				if (nextNode.LoopAudio is not null)
					foreach (var loopAudio in nextNode.LoopAudio)
						_sceneManager.LoopAudioSet.Add(loopAudio);

				if (nextNode.OneShotAudio is not null)
					foreach (var oneShotAudio in nextNode.OneShotAudio)
						_sceneManager.OneShotAudioSet.Add(oneShotAudio);
			}

			continue; // 主逻辑代码结束

		AfterProcess: // 删除不必要的资源
			_nodeEnum.Pop(); // 移除节点

			// 删除音频资源列表
			if (nowNode.LoopAudio is not null)
				foreach (var loopAudio in nowNode.LoopAudio)
					_sceneManager.LoopAudioSet.Remove(loopAudio);

			if (nowNode.OneShotAudio is not null)
				foreach (var oneShotAudio in nowNode.OneShotAudio)
					_sceneManager.OneShotAudioSet.Remove(oneShotAudio);
		}
	}


	/// <summary>
	/// 根据 json 中的描述包装一个图层
	/// </summary>
	/// <param name="layerStructure">传入图层 json 描述</param>
	/// <exception cref="Exception">节点相关资源未被正常加载</exception>
	private Layer PackLayer(LayerStructure layerStructure)
	{
		if (layerStructure.Name is null)
			throw new Exception("layer dont have name");

		Layer layer = new()
		{
			Pos = new(layerStructure.Position.X, layerStructure.Position.Y)
		};

		(int width, int height) = (layerStructure.WinSize.Width, layerStructure.WinSize.Height);


		if (layerStructure.LayerType is null)
			throw new Exception("Unknow Layer Type");

		// 插入图片
		if (layerStructure.LayerType == "image")
		{
			if (layerStructure.Image is null)
				throw new Exception($"Layer {layerStructure.Name} image Not Found");

			var image = _resourceManager.GetImage(layerStructure.Image);

			Console.WriteLine($"Image Size: {image.Width},{image.Height}");

			if (layerStructure.WinSize == default)
				(width, height) = (image.Width, image.Height);
			layer.WinSize = new(width, height);
			layer.BackGroundSKBitmap = new(width, height, LayerConfig.DefaultColorType, LayerConfig.DefaultAlphaType);

			var cutPosition = layerStructure.CutPosition;
			var cutWinSize = layerStructure.CutWinSize;
			if (cutWinSize == default)
				cutWinSize = new(width, height);

			// Console.WriteLine($"({cutPosition.X},{cutPosition.Y}) : ({cutWinSize.Width},{cutWinSize.Height}) -> ({layerStructure.Position.X},{layerStructure.Position.Y})");

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
				BeginPosition = (layerStructure.BeginPosition.X, layerStructure.BeginPosition.Y),
				EndPosition = (layerStructure.EndPosition.X, layerStructure.EndPosition.Y),
				DelayTime = layerStructure.Time,
				AnimationClass = AnimationRegister.GetAnimation(layerStructure.Animation)
			};
		}


		// 添加属性
		layer.DynamicAttribute = layerStructure.Atrribute;
		layer.OriginalAttribute = layerStructure.Atrribute;

		return layer;
	}

	private void SetLayerAction(TrigerStructure triger, List<ActionStructure> actions, Scene scene)
	{
		foreach (var action in actions)
		{
			if (triger.MouseEvent is not null)
			{
				switch (triger.MouseEvent)
				{
					case "LeftClick":
						scene.RegitserLeftClickAction(triger.LayerName, action);
						break;
					case "RightClick":
						scene.RegitserRightClickAction(triger.LayerName, action);
						break;
					case "MoveOn":
						scene.RegitserMoveOnAction(triger.LayerName, action);
						break;
					default:
						scene.RegitserLeftClickAction(triger.LayerName, action);
						break;
				}
			}
		}
	}

	/// <summary>
	/// 同步处理所有需要加载的场景，取每个图层需要的资源，然后通过调用 PackLayer(layerName)来打包图层，
	/// 所有图层打包完成后调用 PackScene(sceneName) 来将图层打包，并且放入资源管理器
	/// </summary>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	private void ProcessSceneAsync()
	{
		while (_sceneName.Count != 0)
		{
			var sceneName = _sceneName.Dequeue();
			// 先将场景名字放入队列，再添加名字到场景的映射，减少资源占用
			_sceneManager.SceneNameList.Enqueue(sceneName);
			if (_sceneManager.ContainsScene(sceneName))
				continue;

			string sceneScript = _resourceManager.GetScript(sceneName);
			SceneStructure sceneStructure = JsonSerializer.Deserialize<SceneStructure>(sceneScript);

			if (sceneStructure.Layers is null)
				throw new Exception("No Scene Layer");

			Scene scene = new()
			{
				IsStatic = sceneStructure.IsStatic
			};

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

					SetLayerAction(@event.Triger.Value, @event.Action, scene);
				}
			_sceneManager.PushScene(sceneName, scene);
		}
	}


	// todo 功能不完善，后续会加入实时解析功能
	// todo 执行一次有效加载时，函数会加载一整个Node下的所有场景，并且会自动判断是否需要解析下一个Node
	/// <summary>
	/// 开始执行解释流程，唯一公共对外口
	/// </summary>
	/// <returns></returns>
	public async Task ParsingNextAsync()
	{
		if (_sceneManager.SceneNameList.Count == 0)
			await ProcessNodeAsync();
		await ProcessResourceAsync();
		ProcessSceneAsync();
	}

	public async Task SetGameAsync(string gameName)
	{
		var gameBase = "Data/" + gameName + "/";
		_resourceManager.basePath = gameBase;
		await _resourceManager.PullScriptAsync();

		var mainObj = JsonSerializer.Deserialize<NodeStructure>(_resourceManager.GetScript());
		if (mainObj.NodeURL is null)
			throw new Exception("empty Node");
		_nodeEnum.Push((
			"main",
			mainObj.NodeURL.GetEnumerator()
		));

		if (mainObj.LoopAudio is not null)
			foreach (var loopAudio in mainObj.LoopAudio)
				_sceneManager.LoopAudioSet.Add(loopAudio);

		if (mainObj.OneShotAudio is not null)
			foreach (var oneShotAudio in mainObj.OneShotAudio)
				_sceneManager.OneShotAudioSet.Add(oneShotAudio);

		//! test
		// MouseEvent evn = new()
		// {
		// 	Button = MouseButton.LButton,
		// 	Status = MouseStatus.ButtonDown
		// };
		// var options = new JsonSerializerOptions
		// {
		// 	WriteIndented = true,
		// 	Converters =
		// 	{
		// 		new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
		// 	}
		// };
		// var str = JsonSerializer.Serialize(evn, options);
		// Console.WriteLine(str);
	}
}