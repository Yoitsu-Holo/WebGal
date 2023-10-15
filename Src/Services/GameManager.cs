using SkiaSharp;
using Microsoft.JSInterop;
using WebGal.Libs;
using Web.Libs;
using WebGal.Libs.Base;
using WebGal.Global;

namespace WebGal.Services;
public class GameManager
{
	private readonly IJSRuntime _js;                        //^ JavaScript 互操作运行时
	private readonly Interpreter _interpreter;              //^ 脚本解释器
	private readonly ResourceManager _resourceManager;      //^ 资源管理器
	private readonly SceneManager _sceneManager;            //^ 场景管理器
	private readonly EventManager _eventManager;            //^ 事件管理器：（点击)
	private readonly Renderer _render = new();              //^ 渲染器
	private Scene? _scene;
	private Dictionary<string, string>? _loopAudiosRef;     //^ 循环音频库
	private Dictionary<string, string>? _oneShotAduioRef;   //^ 单次音频库

	private string _sceneName = "StartMenu";

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
		_interpreter = new(_sceneManager, _resourceManager);
		_eventManager = new();
	}

	public void Render(SKCanvas canvas, long timeoff, bool force = false)
	{
		if (_scene is null)
			throw new Exception("render scene not set");
		_render.Render(canvas, _scene, timeoff, force);
	}

	private void LoadScene()
	{
		if (_sceneManager.SceneNameList.Count != 0)
			_sceneName = _sceneManager.SceneNameList.Peek();

		if (_scene is null || !_scene.IsStatic)
		{
			_sceneManager.SceneNameList.Dequeue();
			_scene = _sceneManager.LoadScene(_sceneName);
			_scene.StartAnimation();
		}
	}

	public async Task OnClickAsync(SKPointI pos)
	{
		if (_scene is null)
			throw new Exception("scene not set");
		_eventManager.OnClick(pos);

		// 如果动画没有结束，那么结束动画保留这一帧
		if (_scene.HasAnimation(NowTime.Minisecond))
		{
			_scene.StopAnimation();
			return;
		}

		// 如果当前场景动画结束，切换到下一场景
		await _interpreter.ParsingNextAsync();
		LoadScene();
		await LoadMedia();
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
		_render.Clear();
		_sceneManager.Clear();
		_resourceManager.Clear();
		_interpreter.Clear();

		// 注册一个点击事件，位置是 （50，50）~（100，100）的正方形
		_eventManager.RegitserClickActionTest(new SKRectI(50, 50, 100, 100));

		await _interpreter.SetGameAsync(gameName);
		await _interpreter.ParsingNextAsync();

		LoadScene();
	}
	#endregion
}