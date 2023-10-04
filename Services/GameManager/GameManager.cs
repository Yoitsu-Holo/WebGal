namespace WebGal.Services;

using SkiaSharp;
using WebGal.Services.Debug;


public class GameManager
{
	private readonly Render _render = new();
	private readonly Interpreter _interpreter = new();
	private readonly ResourceManager _resourceManager = new();
	private readonly SceneManager _sceneManager = new();


	public SKBitmap GetFrame(int timeoff, bool force = false)
	{
		return _render.GetNextFrame(timeoff, force);
	}

	#region Debug
	private readonly Test _test = new();

	public async void DoTest()
	{
		// await _test.DoTest();
		return;
	}
	#endregion
}