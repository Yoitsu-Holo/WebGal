using SkiaSharp;
using WebGal.Global;
using WebGal.Services.Module;

namespace WebGal.Services;

public class Interpreter
{
	private readonly SceneManager _sceneManager;
	private readonly ResourceManager _resourceManager;
	public Interpreter(SceneManager sceneManager, ResourceManager resourceManager)
	{
		_sceneManager = sceneManager;
		_resourceManager = resourceManager;
	}

	public async Task DoTest(string testCaseName = "defualtTest")
	{
		// Load Scene 1
		List<Task> tasks = new()
		{
			_resourceManager.PullImageAsync("bg1", "Data/Test/pack/bg/bg010a.png"),
			_resourceManager.PullImageAsync("st1-1", "Data/Test/pack/stand/04/st-aoi_a101.png"),
			_resourceManager.PullAudioAsync("bgm","/Data/Test/pack/sound/bgm/bgm02_b.ogg")
		};

		await Task.WhenAll(tasks);

		Scene TestScene = new();

		Layer layer;
		LayerText layerText;

		#region  background
		{
			layer = new()
			{
				Pos = new(0, 0),
				WinSize = new(1280, 720),
				BackGroundSKBitmap = _resourceManager.GetImage("bg1")
			};
			TestScene.PushLayer("background", layer);
		}
		#endregion

		#region Stand
		{
			var img = _resourceManager.GetImage("st1-1");
			layer = new()
			{
				Pos = new(50, -200),
				WinSize = new(img.Width, img.Height),
				BackGroundSKBitmap = img,
				Anim = new()
				{
					BeginPosition = (-500, 0),
					EndPosition = (0, 0),
					DelayTime = 4000,
				}
			};
			TestScene.PushLayer("stand", layer);
		}
		#endregion

		#region  Textbox
		{
			layer = new()
			{
				Pos = new(20, 550),
				WinSize = new(1240, 150),
				BackGroundSKBitmap = new(1220, 90, LayerConfig.DefualtColorType, LayerConfig.DefualtAlphaType)
			};
			using SKCanvas canvas = new(layer.BackGroundSKBitmap);
			canvas.DrawRect(new SKRect(0, 0, layer.WinSize.Width, layer.WinSize.Height), new SKPaint
			{
				Color = new SKColor(186, 184, 187, 180),
				// Color = SKColors.Aqua,
			});
			canvas.Flush();
			TestScene.PushLayer("textbox", layer);
		}
		#endregion

		#region Text
		{
			layer = new()
			{
				Pos = new(30, 570),
				WinSize = new(1220, 90)
			};

			#region test1
			{
				layerText = new()
				{
					Text = "WebGal",
					Pos = new SKPoint(60, 20),
					Paint = LayerConfig.DefualtTextPaint
				};
				layer.Text.Add(layerText);
			}
			#endregion

			#region test2
			{
				layerText = new()
				{
					Text = "Hello World..................................................................................",
					Pos = new SKPoint(100, 50),
					Paint = new SKPaint
					{
						Color = SKColors.Brown,
						TextSize = 30,
						FakeBoldText = true,
						IsAntialias = true
					},
				};
				layer.Text.Add(layerText);
			}

			TestScene.PushLayer("text", layer);
			#endregion
		}
		#endregion

		// Console.WriteLine(Convert.ToBase64String(_resourceManager.GetAudio("bgm")));
		TestScene.LoopAudiosList["bgm"] = _resourceManager.GetAudio("bgm");

		_sceneManager.PushScene(testCaseName, TestScene);
		return;
	}
}