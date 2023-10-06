using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text.Json;
using SkiaSharp;
using WebGal.Global;
using WebGal.Services.Data;
using WebGal.Services.Module;

namespace WebGal.Services;

public class Interpreter
{
	// private Dictionary<string, List<string>> _nodeName = new();
	private List<string> _sceneName = new();
	private Dictionary<string, List<string>> _layerName = new();
	private Dictionary<string, Layer> _layers = new();
	private Queue<string> _UnloadedResPackName = new();

	private readonly SceneManager _sceneManager;
	private readonly ResourceManager _resourceManager;
	public Interpreter(SceneManager sceneManager, ResourceManager resourceManager)
	{
		_sceneManager = sceneManager;
		_resourceManager = resourceManager;
	}

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

	private async Task ProcessNodeAsync(string nowNodeName)
	{
		string partScript = _resourceManager.GetScript(nowNodeName);
		NodeStructure node = JsonSerializer.Deserialize<NodeStructure>(partScript);

		if (node == default)
			throw new Exception("No volume");

		var urlTasks = node.ResouresPackURL?.Select(resourcePack =>
			{
				_UnloadedResPackName.Enqueue(resourcePack.Name);
				return _resourceManager.PullScriptAsync(resourcePack.Name, resourcePack.URL);
			});
		if (urlTasks is not null)
			await Task.WhenAll(urlTasks);

		// 非叶子节点，递归添加
		if (!node.IsLeaf)
		{
			if (node.NodeURL is null)
				throw new Exception("NodeURL is null");

			var sceneTasks = node.NodeURL.Select(nodeURL => _resourceManager.PullScriptAsync(nodeURL.Name, nodeURL.URL));
			await Task.WhenAll(sceneTasks);

			foreach (var nextNode in node.NodeURL)
				await ProcessNodeAsync(nextNode.Name);
		}
		// 叶子节点，添加场景
		else
		{
			var sceneTasks = node.SceneURL?.Select(sceneScriptURL =>
				{
					_sceneName.Add(sceneScriptURL.Name);
					return _resourceManager.PullScriptAsync(sceneScriptURL.Name, sceneScriptURL.URL);
				});
			if (sceneTasks is not null)
				await Task.WhenAll(sceneTasks);
		}
	}

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
			(int R, int G, int B, int A) = (shapeColor.R, shapeColor.G, shapeColor.B, shapeColor.A);

			layer.BackGroundSKBitmap = new(width, height, LayerConfig.DefualtColorType, LayerConfig.DefualtAlphaType);
			using SKCanvas canvas = new(layer.BackGroundSKBitmap);
			canvas.DrawRect(new SKRect(0, 0, layer.WinSize.Width, layer.WinSize.Height), new SKPaint
			{
				Color = new SKColor(186, 184, 187, 180),
			});
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
				if (text.Text is null)
					throw new Exception($"Layer {layerStructure.Name} text Not Found");
				LayerText layerText = new()
				{
					Text = text.Text,
					Pos = new(text.Offset.X, text.Offset.Y),
					Paint = LayerConfig.DefualtTextPaint
				};

				var textPaint = text.Paint;
				if (textPaint != default)
				{
					layerText.Paint.FakeBoldText = textPaint.Blod;
					layerText.Paint.IsAntialias = textPaint.Antialias;
					layerText.Paint.TextSize = textPaint.TextSize;
				}
				layer.Text.Add(layerText);
			}
		}

		if (layerStructure.Animation is not null)
		{
			layer.Anim = new()
			{
				BeginPosition = (layerStructure.BeginPosition.X, layerStructure.BeginPosition.Y),
				EndPosition = (layerStructure.EndPosition.X, layerStructure.EndPosition.Y),
				DelayTime = layerStructure.Time,
			};
		}
		_layers[layerStructure.Name] = layer;
	}

	private void PackScene(string sceneName)
	{
		Scene scene = new();
		foreach (var layerName in _layerName[sceneName])
			scene.PushLayer(layerName, _layers[layerName]);
		scene.LoopAudiosList["bgm"] = _resourceManager.GetAudio("bgm"); //!
		_sceneManager.PushScene(sceneName, scene);
	}

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

	public async Task ParsingAsync()
	{
		await _resourceManager.PullScriptAsync("main", "Data/Test/main.json");
		await ProcessNodeAsync("main");
		await ProcessResourceAsync();
		await ProcessSceneAsync();

		Console.WriteLine("pre load done"); //!
	}

	// public async Task DoTest(string testCaseName = "defualtTest")
	// {
	// 	// Load Scene 1
	// 	List<Task> tasks = new()
	// 	{
	// 		_resourceManager.PullImageAsync("bg1", "Data/Test/pack/bg/bg010a.png"),
	// 		_resourceManager.PullImageAsync("st1-1", "Data/Test/pack/stand/04/st-aoi_a101.png"),
	// 		_resourceManager.PullAudioAsync("bgm","Data/Test/pack/sound/bgm/bgm02_b.ogg")
	// 	};

	// 	await Task.WhenAll(tasks);

	// 	Scene TestScene = new();

	// 	Layer layer;
	// 	LayerText layerText;

	// 	#region  background
	// 	{
	// 		layer = new()
	// 		{
	// 			Pos = new(0, 0),
	// 			WinSize = new(1280, 720),
	// 			BackGroundSKBitmap = _resourceManager.GetImage("bg1")
	// 		};
	// 		TestScene.PushLayer("background", layer);
	// 	}
	// 	#endregion

	// 	#region Stand
	// 	{
	// 		var img = _resourceManager.GetImage("st1-1");
	// 		layer = new()
	// 		{
	// 			Pos = new(50, -200),
	// 			WinSize = new(img.Width, img.Height),
	// 			BackGroundSKBitmap = img,
	// 			Anim = new()
	// 			{
	// 				BeginPosition = (-500, 0),
	// 				EndPosition = (0, 0),
	// 				DelayTime = 4000,
	// 			}
	// 		};
	// 		TestScene.PushLayer("stand", layer);
	// 	}
	// 	#endregion

	// 	#region  Textbox
	// 	{
	// 		layer = new()
	// 		{
	// 			Pos = new(20, 550),
	// 			WinSize = new(1240, 150),
	// 			BackGroundSKBitmap = new(1240, 150, LayerConfig.DefualtColorType, LayerConfig.DefualtAlphaType)
	// 		};
	// 		using SKCanvas canvas = new(layer.BackGroundSKBitmap);
	// 		canvas.DrawRect(new SKRect(0, 0, layer.WinSize.Width, layer.WinSize.Height), new SKPaint
	// 		{
	// 			Color = new SKColor(186, 184, 187, 180),
	// 			// Color = SKColors.Aqua,
	// 		});
	// 		canvas.Flush();
	// 		TestScene.PushLayer("textbox", layer);
	// 	}
	// 	#endregion

	// 	#region Text
	// 	{
	// 		layer = new()
	// 		{
	// 			Pos = new(30, 570),
	// 			WinSize = new(1220, 90)
	// 		};

	// 		#region test1
	// 		{
	// 			layerText = new()
	// 			{
	// 				Text = "WebGal",
	// 				Pos = new SKPoint(60, 20),
	// 				Paint = LayerConfig.DefualtTextPaint
	// 			};
	// 			layer.Text.Add(layerText);
	// 		}
	// 		#endregion

	// 		#region test2
	// 		{
	// 			layerText = new()
	// 			{
	// 				Text = "Hello World..................................................................................",
	// 				Pos = new SKPoint(100, 50),
	// 				Paint = new SKPaint
	// 				{
	// 					Color = SKColors.Brown,
	// 					TextSize = 30,
	// 					FakeBoldText = true,
	// 					IsAntialias = true
	// 				},
	// 			};
	// 			layer.Text.Add(layerText);
	// 		}

	// 		TestScene.PushLayer("text", layer);
	// 		#endregion
	// 	}
	// 	#endregion

	// 	TestScene.LoopAudiosList["bgm"] = _resourceManager.GetAudio("bgm");
	// 	_sceneManager.PushScene(testCaseName, TestScene);

	// 	// TestStructure test = new(new()
	// 	// {
	// 	// 	new("Test1", "World"),
	// 	// 	new("Test2", "Hello")
	// 	// });
	// 	// Console.WriteLine(JsonSerializer.Serialize(test));

	// 	return;
	// }
}