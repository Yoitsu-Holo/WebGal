namespace WebGal.Services;

using SkiaSharp;
using WebGal.Services.Debug;

public class GameManager
{
	private readonly SceneManager _sceneManager;
	private readonly Render _render;
	private readonly Interpreter _interpreter;
	private readonly ResourceManager _resourceManager;

	public GameManager(HttpClient httpClient)
	{
		_resourceManager = new(httpClient);
		_sceneManager = new();
		_render = new(_sceneManager);
		_interpreter = new(_sceneManager, _resourceManager);
	}

	public SKBitmap GetFrame(int timeoff, bool force = false)
	{
		return _render.GetNextFrame(timeoff, force);
	}

	#region Debug
	private readonly Test _test = new();

	public async void DoTest(int timeoff)
	{
		await _interpreter.DoTest("testScene");
		_render.LoadScene("testScene");
		_render.GetNextFrame(timeoff);
	}
	#endregion
}