using SkiaSharp;
using WebGal.Services.Data;

namespace WebGal.Services.Debug;

public class Test
{
	#region Debug
	public async Task DoTest()
	{
		Scene newScene = new();
		Layer layer;
		LayerText layerText;
		HttpClient httpClient = new();

		#region  background
		layer = new()
		{
			Pos = new(0, 0),
			WinSize = new(1280, 720)
		};
		string bg = "http://localhost:5205/Data/game_demo/pack/bg/bg010a.png";
		var stream = await httpClient.GetStreamAsync(bg);
		layer.BackGroundSKBitmap = SKBitmap.Decode(stream);
		newScene.PushLayer("background", layer);
		#endregion

		#region  Textbox
		layer = new()
		{
			Pos = new(20, 550),
			WinSize = new(1240, 150),
			BackGroundSKBitmap = new(1220, 90, SKColorType.Rgba8888, SKAlphaType.Unpremul)
		};
		SKCanvas canvas = new(layer.BackGroundSKBitmap);
		canvas.DrawRect(new SKRect(0, 0, layer.WinSize.Width, layer.WinSize.Height), new SKPaint
		{
			Color = new SKColor(186, 184, 187, 180),
		});
		canvas.Flush();
		newScene.PushLayer("textbox", layer);
		#endregion

		#region text
		layer = new()
		{
			Pos = new(30, 570),
			WinSize = new(1220, 90)
		};
		layerText = new()
		{
			Text = "WebGal",
			Pos = new SKPoint(60, 20),
			Paint = new SKPaint
			{
				Color = SKColors.Black,
				TextSize = 30,
				FakeBoldText = true,
			},
		};
		layer.Text.Add(layerText);

		layerText = new()
		{
			Text = "Hello World",
			Pos = new SKPoint(100, 50),
			Paint = new SKPaint
			{
				Color = SKColors.Aqua,
				TextSize = 30,
				FakeBoldText = true,
			},
		};
		layer.Text.Add(layerText);
		newScene.PushLayer("text", layer);
		#endregion

		return;
	}
	#endregion
}