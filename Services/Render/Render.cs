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

	private SKCanvas _canvas;
	private Scene _scene = new();
	private readonly SceneManager _sceneManager = null!;

	#endregion

	public Render(SceneManager sceneManager)
	{
		_sceneManager = sceneManager;
		_canvas = new(_buffer);
		// var context = GRContext.CreateGl();
		// var surface = SKSurface.Create(context, false, _buffer.Info);
		// _canvas = surface.Canvas;
		// surface.Draw()
	}

	public void LoadScene(string sceneName, long startTime)
	{
		Console.WriteLine(_sceneManager.ToString());
		_scene = _sceneManager.LoadScene(sceneName);
		_scene.SetBeginTime(startTime);
	}


	public void GetNextFrame(SKCanvas canvas, long timeoff, bool force = false)
	{
		_canvas = canvas;
		if (_scene.NeedRendering || force)
			Rendering(timeoff);
	}


	private void Rendering(long timeoff)
	{
		_canvas.Clear();

		// Console.WriteLine($"rendering time: {timeoff}");
		_scene.NeedRendering = false;
		foreach (var (layerId, layer) in _scene.Layers)
		{
			// Console.WriteLine($"{timeoff}, {layerId}");
			layer.GenNextFrame(timeoff);

			if (layer.BackGroundSKBitmap is not null)
				_canvas.DrawBitmap(layer.FrameBuffer, layer.PosAt(timeoff));

			if (layer.Text is List<LayerText> texts)
				foreach (var text in texts)
					_canvas.DrawText(text.Text, layer.AbsolutePos(text.Pos), text.Paint);

			_scene.NeedRendering |= layer.Anim.HasAnimation(timeoff);
		}
		// Console.WriteLine(DateTimeOffset.Now.Ticks / 10000);

		_canvas.Flush();
	}
}
