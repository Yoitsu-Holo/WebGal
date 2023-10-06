using System.Diagnostics.CodeAnalysis;
using SkiaSharp;
using WebGal.Services.Module;

namespace WebGal.Services.Data;

public class FrameBuffer
{
	public bool ChangedFlag { get; set; } = true;
	public SKBitmap Bitmap { get; set; }
	private SKCanvas _canvas;
	private readonly SortedDictionary<string, Layer> _layers = new();
	private FrameBufferConfig _config;

	private List<string> _layerNames => _config.LayerNames;


	public FrameBuffer(FrameBufferConfig config)
	{
		SetFrameBuffer(config);
	}

	[MemberNotNull(nameof(_config), nameof(Bitmap), nameof(_canvas))]
	public void SetFrameBuffer(FrameBufferConfig config)
	{
		_config = config;
		_layers.Clear();
		foreach (var layerName in config.LayerNames)
			_layers[layerName] = new();
		Bitmap = new(_config.Width, _config.Height, SKColorType.Rgba8888, SKAlphaType.Unpremul);
		_canvas = new(Bitmap);
	}

	public int Width { get { return _config.Width; } }
	public int Height { get { return _config.Height; } }

	public void Render()
	{
		foreach (var layername in _layerNames)
		{
			Console.WriteLine(layername);
			var layer = _layers[layername];

			if (layer.BackGroundSKBitmap is SKBitmap image)
			{
				using var img = image.Resize(layer.WinSize, SKFilterQuality.High);
				_canvas.DrawBitmap(img, layer.Pos);
			}
			// Console.WriteLine($"{layer.Pos.X},{layer.Pos.Y}");

			if (layer.Text is List<LayerText> texts)
				foreach (var text in texts)
					_canvas.DrawText(text.Text, layer.AbsolutePos(text.Pos), text.Paint);

		}
		_canvas.Flush();
		ChangedFlag = false;
		Console.WriteLine("---");
	}

	#region Debug
	public async Task Test()
	{
		FrameBufferConfig config = new();
		config.PushLayer("background");
		config.PushLayer("stand");
		config.PushLayer("textbox");
		config.PushLayer("textcontent");

		config.SetResolution(1280, 720);

		SetFrameBuffer(config);

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

		_layers["background"] = layer;
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

		_layers["textbox"] = layer;
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
		_layers["textcontent"] = layer;
		#endregion
		return;
	}
	#endregion
}