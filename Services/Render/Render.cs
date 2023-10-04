namespace WebGal.Services;

using SkiaSharp;
using WebGal.Global;
using WebGal.Services.Data;

public class Render
{
	#region Matedata
	public SKSizeI Resolution { get; set; } = new(
		SceneConfig.DefualtWidth,
		SceneConfig.DefualtHeight
	);
	#endregion


	#region render Data
	private readonly SKBitmap _buffer = new(
		SceneConfig.DefualtWidth,
		SceneConfig.DefualtHeight,
		SceneConfig.DefualtColorType,
		SceneConfig.DefualtAlphaType
	);

	private readonly SKCanvas _canvas;
	public Scene RenderScene { get => _renderScene; set { _renderScene = value; _renderFlag = true; } }
	#endregion

	private bool _renderFlag = true;
	private Scene _renderScene = new();

	public Render()
	{
		_canvas = new(_buffer);
	}

	private void Rendering()
	{
		foreach (var (layerId, layer) in RenderScene.Layers)
		{
			Console.WriteLine(layerId);

			if (layer.BackGroundSKBitmap is SKBitmap image)
			{
				using var img = image.Resize(layer.WinSizeI, SKFilterQuality.High);
				_canvas.DrawBitmap(img, layer.Pos);
			}

			if (layer.Text is List<LayerText> texts)
				foreach (var text in texts)
					_canvas.DrawText(text.Text, layer.AbsolutePos(text.Pos), text.Paint);

		}

		#region Debug

		_canvas.DrawRect(20, 20, 100, 100, new SKPaint
		{
			Color = SKColors.Aqua,
			StrokeWidth = 5,
		});

		#endregion

		_canvas.Flush();

		if (RenderScene is null)
			_renderFlag = false;

		Console.WriteLine("---");
	}

	/// <summary>
	/// todo
	/// </summary>
	/// <returns>false</returns>
	private bool NeedRerendering()
	{
		return false;
	}

	public SKBitmap GetNextFrame(int timeoff, bool force = false)
	{
		if (NeedRerendering() || force)
			Rendering();
		return _buffer;
	}
}
