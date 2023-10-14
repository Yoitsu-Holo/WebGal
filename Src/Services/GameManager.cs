using SkiaSharp;
using Microsoft.JSInterop;
using WebGal.Libs;

namespace WebGal.Services;
public class GameManager
{
	private readonly IJSRuntime _js;                      //^ JavaScript 互操作运行时
	private readonly Interpreter _interpreter;            //^ 脚本解释器
	private readonly ResourceManager _resourceManager;    //^ 资源管理器
	private readonly SceneManager _sceneManager;          //^ 场景管理器
	private readonly Renderer _render;                      //^ 渲染器
	private Dictionary<string, string>? _loopAudiosRef;  //^ 循环音频库
	private Dictionary<string, string>? _oneShotAduioRef;//^ 单次音频库

	private string _sceneName = "main";

	/// <summary>
	/// 构造函数，由系统执行依赖注入
	/// </summary>
	/// <param name="httpClient"></param>
	/// <param name="js"></param>
	public GameManager(HttpClient httpClient, IJSRuntime js)
	{
		_js = js;
		_resourceManager = new(httpClient);
		_sceneManager = new();
		_render = new(_sceneManager);
		_interpreter = new(_sceneManager, _resourceManager);
	}

	public void Render(SKCanvas canvas, long timeoff, bool force = false) => _render.Render(canvas, timeoff, force);

	public async Task OnClickAsync(SKPoint pos)
	{
		long timeoff = DateTimeOffset.Now.Ticks / 10000L;
		// 如果动画没有结束，那么结束动画保留这一帧
		// 如果当前场景动画结束，切换到下一场景
		if (_render.HasAnimation(timeoff))
		{
			_render.StopAnimation();
			return;
		}
		await _interpreter.ParsingNextAsync();
		if (_sceneManager.SceneNameList.Count() != 0)
			_sceneName = _sceneManager.SceneNameList.Dequeue();
		_render.LoadScene(_sceneName, DateTimeOffset.Now.Ticks / 10000L);
		await LoadMedia();
		Console.WriteLine("_sceneManager.LoopAudioSet:");
		foreach (var audio in _sceneManager.LoopAudioSet)
			Console.WriteLine(audio);
	}


	public void SetMediaList(Dictionary<string, string> loopAudiosRef, Dictionary<string, string> oneShotAduioRef)
	{
		_loopAudiosRef = loopAudiosRef;
		_oneShotAduioRef = oneShotAduioRef;
	}

	/// <summary>
	/// 加载媒体资源
	/// </summary>
	/// <exception cref="Exception">未正确设置媒体资源列表</exception>
	public async Task LoadMedia()
	{
		if (_loopAudiosRef == null || _oneShotAduioRef == null)
			throw new Exception("Media List not set");

		foreach (var (audioName, _) in _loopAudiosRef)
			if (!_sceneManager.LoopAudioSet.Contains(audioName))
				_loopAudiosRef.Remove(audioName);

		foreach (var (audioName, _) in _oneShotAduioRef)
			if (!_sceneManager.OneShotAudioSet.Contains(audioName))
				_oneShotAduioRef.Remove(audioName);

		foreach (var audioName in _sceneManager.LoopAudioSet)
		{
			if (_loopAudiosRef.ContainsKey(audioName))
				continue;

			var byteStream = _resourceManager.GetAudio(audioName);
			var audio = await _js.InvokeAsync<string>("audioOggToLink", byteStream);
			_loopAudiosRef.Add(audioName, audio);
		}

		foreach (var audioName in _sceneManager.OneShotAudioSet)
		{
			if (_oneShotAduioRef.ContainsKey(audioName))
				continue;

			var byteStream = _resourceManager.GetAudio(audioName);
			var audio = await _js.InvokeAsync<string>("audioOggToLink", byteStream);
			_oneShotAduioRef.Add(audioName, audio);
		}

		return;
	}

	#region Debug
	/// <summary>
	/// 测试程序接口，内部填入测试流程代码
	/// </summary>
	/// <returns></returns>
	[Obsolete("Debug Only")]
	public async Task DoTest(string gameName)
	{
		_sceneManager.Clear();
		_resourceManager.Clear();
		_interpreter.Clear();
		await _interpreter.SetGameAsync(gameName);
		await _interpreter.ParsingNextAsync();
		await OnClickAsync(new SKPoint(0, 0));
		// _render.LoadScene(_sceneName = _sceneManager.SceneNameList.Dequeue(), DateTimeOffset.Now.Ticks / 10000L);
	}
	#endregion
}