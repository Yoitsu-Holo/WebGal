namespace WebGal.Services;

using SkiaSharp;
using WebGal.Global;
using WebGal.Services.Module;

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
	private Scene _scene = new();
	private readonly SceneManager _sceneManager = null!;

	#endregion

	private bool _renderFlag = true;

	public Render(SceneManager sceneManager)
	{
		_sceneManager = sceneManager;
		_canvas = new(_buffer);
	}

	private void Rendering()
	{
		foreach (var (layerId, layer) in _scene.Layers)
		{
			if (layer.BackGroundSKBitmap is SKBitmap image)
			{
				using var img = image.Resize(layer.WinSizeI, SKFilterQuality.Medium);
				_canvas.DrawBitmap(img, layer.Pos);
			}

			if (layer.Text is List<LayerText> texts)
				foreach (var text in texts)
					_canvas.DrawText(text.Text, layer.AbsolutePos(text.Pos), text.Paint);

		}

		// #region Debug

		// _canvas.DrawRect(20, 20, 1000, 600, new SKPaint
		// {
		// 	Color = SKColors.Aqua,
		// 	StrokeWidth = 5,
		// });

		// #endregion

		_canvas.Flush();

		if (_scene is null)
			_renderFlag = false;
		_renderFlag = false;
	}

	public SKBitmap GetNextFrame(int timeoff, bool force = false)
	{
		if (_renderFlag || force)
			Rendering();
		return _buffer;
	}

	public void LoadScene(string sceneName)
	{
		Console.WriteLine(_sceneManager.ToString());
		_scene = _sceneManager.LoadScene(sceneName);
		_renderFlag = true;
	}
}
