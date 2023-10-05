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

	public void GetFrame(SKCanvas canvas, long timeoff, bool force = false)
	{
		_render.GetNextFrame(canvas, timeoff, force);
	}

	#region Debug
	private readonly Test _test = new();

	public async Task DoTest()
	{
		await _interpreter.DoTest("testScene");
		var timeoff = DateTimeOffset.Now.Ticks / 10000L;
		_render.LoadScene("testScene", timeoff);
	}
	#endregion
}