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

	private void Rendering(int timeoff)
	{
		foreach (var (layerId, layer) in _scene.Layers)
		{
			Console.Write($"{DateTimeOffset.Now.Second * 1000 + DateTimeOffset.Now.Millisecond}, ");
			layer.NextFrame(timeoff);
			if (layer.BackGroundSKBitmap is not null)
				_canvas.DrawBitmap(layer.FrameBuffer, layer.FramePosition);

			if (layer.Text is List<LayerText> texts)
				foreach (var text in texts)
					_canvas.DrawText(text.Text, layer.AbsolutePos(text.Pos), text.Paint);
		}
		Console.WriteLine($"{DateTimeOffset.Now.Second * 1000 + DateTimeOffset.Now.Millisecond}");

		_canvas.Flush();

		if (_scene is null)
			_renderFlag = false;
		_renderFlag = false;
	}

	public SKBitmap GetNextFrame(int timeoff, bool force = false)
	{
		if (_renderFlag || force)
			Rendering(timeoff);
		return _buffer;
	}

	public void LoadScene(string sceneName)
	{
		Console.WriteLine(_sceneManager.ToString());
		_scene = _sceneManager.LoadScene(sceneName);
		_renderFlag = true;
	}
}
