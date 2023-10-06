namespace WebGal.Services;

using SkiaSharp;
using WebGal.Services.Debug;
using WebGal.Services.Module;
using Microsoft.JSInterop;

public class GameManager
{
	private readonly IJSRuntime _js;
	private readonly SceneManager _sceneManager;
	private readonly Render _render;
	private Scene _scene = new();
	private readonly Interpreter _interpreter;
	private readonly ResourceManager _resourceManager;

	public GameManager(HttpClient httpClient, IJSRuntime js)
	{
		_js = js;
		_resourceManager = new(httpClient);
		_sceneManager = new();
		_render = new(_sceneManager);
		_interpreter = new(_sceneManager, _resourceManager);
	}

	private void LoadScene(string sceneName, long startTime)
	{
		_scene = _sceneManager.LoadScene(sceneName);
		_scene.SetBeginTime(startTime);
	}

	public void GetFrame(SKCanvas canvas, long timeoff, bool force = false)
	{
		_render.GetNextFrame(canvas, timeoff, force);
	}

	public async void LoadMedia(Dictionary<string, string> loopAudios, Dictionary<string, string> oneShotAduio)
	{
		// loopAduio.Clear();
		// oneShotAduio.Clear();
		foreach (var (audioName, byteStream) in _scene.LoopAudiosList)
		{
			var audio = await _js.InvokeAsync<string>("audioOggToLink", byteStream);
			loopAudios.Add(audioName, audio);
		}
		foreach (var (audioName, byteStream) in _scene.OneShotAudiosList)
		{
			var audio = await _js.InvokeAsync<string>("audioOggToLink", byteStream);
			oneShotAduio.Add(audioName, audio);
		}
		return;
	}

	#region Debug
	private readonly Test _test = new();


	public async Task DoTest()
	{
		// await _interpreter.DoTest("testScene");
		await _interpreter.ParsingAsync();
		var timeoff = DateTimeOffset.Now.Ticks / 10000L;
		LoadScene("TestScene", timeoff);
		_render.LoadScene(_scene);
	}
	#endregion
}