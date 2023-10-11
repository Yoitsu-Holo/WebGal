using System.Text.Json;
using SkiaSharp;
using WebGal.Global;
using WebGal.Libs.Base;

namespace WebGal.Libs;
public class Interpreter
{
	// private Dictionary<string, List<string>> _nodeName = new();
	private readonly List<string> _sceneName = new();
	private readonly Dictionary<string, List<string>> _layerName = new();
	private readonly Dictionary<string, Layer> _layers = new();
	private readonly Queue<string> _UnloadedResPackName = new();
	private readonly SceneManager _sceneManager;
	private readonly ResourceManager _resourceManager;

	// 用于存储每一层Node的信息
	private readonly Stack<(string, List<UrlStructure>.Enumerator)> _nodeEnum = new();


	private void Clear()
	{
		_sceneName.Clear();
		_layerName.Clear();
		_layers.Clear();
		_UnloadedResPackName.Clear();
	}

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
		while (_UnloadedResPackName.Count > 0)
		{
			var resourcePackScript = _resourceManager.GetScript(_UnloadedResPackName.Dequeue());
			ResouresStructure resouresPack = JsonSerializer.Deserialize<ResouresStructure>(resourcePackScript);

			if (resouresPack.ImageURL is not null)
			{
				var imageTasks = resouresPack.ImageURL.Select(image => _resourceManager.PullImageAsync(image.Name, image.URL));
				await Task.WhenAll(imageTasks);
			}
			if (resouresPack.AudioURL is not null)
			{
				var audioTasks = resouresPack.AudioURL.Select(aduio => _resourceManager.PullAudioAsync(aduio.Name, aduio.URL));
				await Task.WhenAll(audioTasks);
			}
		}
	}


	// todo 暂只能一次性加载全部流程,无法指定顺序(单场景测试)
	/// <summary>
	/// 异步方式加载游戏流程(节点树)
	/// </summary>
	/// <param name="nowNodeName"></param>
	/// <returns></returns>
	/// <exception cref="Exception">节点值非法(节点值未默认)</exception>
	private async Task ProcessNodeAsync()
	{
		while (_nodeEnum.Count != 0)
		{
			var (nodeName, nodeEnum) = _nodeEnum.Peek();
			var node = JsonSerializer.Deserialize<NodeStructure>(_resourceManager.GetScript(nodeName));

			Console.WriteLine(nodeName); //!

			if (node == default)
				throw new Exception("No volume");

			// 添加资源
			var urlTasks = node.ResouresPackURL?.Select(resourcePack =>
				{
					_UnloadedResPackName.Enqueue(resourcePack.Name);
					return _resourceManager.PullScriptAsync(resourcePack.Name, resourcePack.URL);
				});
			if (urlTasks is not null)
				await Task.WhenAll(urlTasks);

			await ProcessResourceAsync();

			if (node.NodeURL is null)
				throw new Exception("empty Node");

			if (nodeEnum.MoveNext() == false)
			{
				_nodeEnum.Pop();
				continue;
			}

			var nextNodeUrl = nodeEnum.Current;
			Console.WriteLine($"{nextNodeUrl.Name},{nextNodeUrl.URL}"); //!
			await _resourceManager.PullScriptAsync(nextNodeUrl.Name, nextNodeUrl.URL);

			if (node.IsLeaf)// 叶子节点，添加场景
			{
				_sceneName.Add(nextNodeUrl.Name);
				Console.WriteLine("Scene"); //!
				break;
			}
			else
			{
				Console.WriteLine("Node"); //!
				var nodeObj = JsonSerializer.Deserialize<NodeStructure>(_resourceManager.GetScript());
				if (nodeObj.NodeURL is null)
					throw new Exception("empty Node");
				_nodeEnum.Push((nextNodeUrl.Name, nodeObj.NodeURL.GetEnumerator()));
			}

			// todo 删除不必要的资源
		}
	}


	/// <summary>
	/// 根据 json 中的描述包装一个图层
	/// </summary>
	/// <param name="layerStructure">传入图层 json 描述</param>
	/// <exception cref="Exception">节点相关资源未被正常加载</exception>
	private void PackLayer(LayerStructure layerStructure)
	{
		if (layerStructure.Name is null)
			throw new Exception("layer dont have name");

		Layer layer = new();

		(int posX, int posY) = (layerStructure.Position.X, layerStructure.Position.Y);
		(int width, int height) = (layerStructure.WinSize.Width, layerStructure.WinSize.Height);

		layer.Pos = new(posX, posY);
		if (layerStructure.WinSize != default)
			layer.WinSize = new(width, height);

		// 插入图片
		if (layerStructure.IsImageLayer)
		{
			if (layerStructure.Image is null)
				throw new Exception($"Layer {layerStructure.Name} image Not Found");

			var image = _resourceManager.GetImage(layerStructure.Image);
			layer.BackGroundSKBitmap = image;
			if (layerStructure.WinSize == default)
				layer.WinSize = new(image.Width, image.Height);
		}

		// 绘制简单图形
		if (layerStructure.IsShapeLayer)
		{
			var shapeColor = layerStructure.ShapeColor;
			(byte R, byte G, byte B, byte A) = (shapeColor.R, shapeColor.G, shapeColor.B, shapeColor.A);

			layer.BackGroundSKBitmap = new(width, height, LayerConfig.DefaultColorType, LayerConfig.DefaultAlphaType);
			using SKCanvas canvas = new(layer.BackGroundSKBitmap);
			canvas.DrawRect(
				new SKRect(0, 0, layer.WinSize.Width, layer.WinSize.Height),
				new SKPaint { Color = new SKColor(R, G, B, A) }
			);
			canvas.Flush();
		}

		// 插入文本
		if (layerStructure.IsTextLayer)
		{
			if (layerStructure.Text is null)
				throw new Exception($"Layer {layerStructure.Name} textLayer Not Found");

			var texts = layerStructure.Text;
			foreach (var text in texts)
			{
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
		_layers[layerStructure.Name] = layer;
	}


	/// <summary>
	/// 根据场景和图层的名称映射来包装一个新的场景，并且加入到资源管理器中
	/// </summary>
	/// <param name="sceneName"></param>
	private void PackScene(string sceneName)
	{
		Scene scene = new();
		foreach (var layerName in _layerName[sceneName])
			scene.PushLayer(layerName, _layers[layerName]);
		scene.LoopAudiosList.Add("bgm", _resourceManager.GetAudio("bgm"));
		_sceneManager.PushScene(sceneName, scene);
	}


	/// <summary>
	/// 同步处理所有需要加载的场景，取每个图层需要的资源，然后通过调用 PackLayer(layerName)来打包图层，所有图层打包完成后调用 PackScene(sceneName) 来讲图层打包，并且放入资源管理器
	/// </summary>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	private async Task ProcessSceneAsync()
	{
		foreach (var sceneName in _sceneName)
		{
			if (!_layerName.ContainsKey(sceneName))
				_layerName[sceneName] = new();

			string sceneScript = _resourceManager.GetScript(sceneName);
			SceneStructure scene = JsonSerializer.Deserialize<SceneStructure>(sceneScript);

			if (scene == default)
				throw new Exception("No volume");

			var urlTasks = scene.ResouresPackURL?.Select(resource =>
			{
				_UnloadedResPackName.Enqueue(resource.Name);
				return _resourceManager.PullScriptAsync(resource.Name, resource.URL);
			});
			await ProcessResourceAsync();

			if (urlTasks is not null)
				await Task.WhenAll(urlTasks);

			if (scene.Layer is not null)
			{
				foreach (var layer in scene.Layer)
				{
					if (layer.Name is null)
						throw new Exception("Null layer name");

					PackLayer(layer);
					_layerName[sceneName].Add(layer.Name);
				}
				PackScene(sceneName);
			}
		}
	}


	// todo 功能不完善，后续会加入实时解析功能
	/// <summary>
	/// 开始执行解释流程，唯一公共对外口
	/// </summary>
	/// <returns></returns>
	public async Task ParsingNextAsync()
	{
		await ProcessNodeAsync();
		await ProcessSceneAsync();
	}

	public async Task SetGameAsync(string gameName)
	{
		Clear();
		_resourceManager.Clear();
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
	}
}