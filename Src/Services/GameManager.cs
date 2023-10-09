namespace WebGal.Services;

using SkiaSharp;
using WebGal.Libs.Module;
using Microsoft.JSInterop;
using WebGal.Libs;

public class GameManager
{
	private readonly IJSRuntime _js;                      //^ JavaScript 互操作运行时
	private readonly Interpreter _interpreter;            //^ 脚本解释器
	private readonly ResourceManager _resourceManager;    //^ 资源管理器
	private readonly SceneManager _sceneManager;          //^ 场景管理器
	private readonly Render _render;                      //^ 渲染器
	private Scene _scene = new();                         //^ 当前加载的场景
	private Dictionary<string, string> _loopAudiosRef = null!;  //^ 循环音频库
	private Dictionary<string, string> _oneShotAduioRef = null!;//^ 单次音频库

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

	/// <summary>
	/// 
	/// </summary>
	/// <param name="canvas"></param>
	public void SetTargetCanvas(SKCanvas canvas) => _render.SetTargetCanvas(canvas);

	/// <summary>
	/// 将一个指定的时间的场景渲染到指定画布上
	/// </summary>
	/// <param name="timeoff">设定场景时间</param>
	/// <param name="force">可选参数，忽略渲染器的惰性渲染机制，采用强制渲染</param>
	public void GetFrame(long timeoff, bool force = false) => _render.GetNextFrame(timeoff, force);

	public void OnClick(SKPoint pos)
	{
		_render.LoadScene("TestScene", DateTimeOffset.Now.Ticks / 10000L);
	}

	public void SetMediaList(Dictionary<string, string> loopAudiosRef, Dictionary<string, string> oneShotAduioRef)
	{
		_loopAudiosRef = loopAudiosRef;
		_oneShotAduioRef = oneShotAduioRef;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="loopAudiosRef"></param>
	/// <param name="oneShotAduioRef"></param>
	public async void LoadMedia()
	{
		var (loopAudiosList, lneShotAudiosList) = _render.GetSceneAudioMedia();
		foreach (var (audioName, byteStream) in loopAudiosList)
		{
			var audio = await _js.InvokeAsync<string>("audioOggToLink", byteStream);
			_loopAudiosRef.Add(audioName, audio);
		}
		foreach (var (audioName, byteStream) in lneShotAudiosList)
		{
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
		if (gameName == "Test1" || gameName == "Test2")
		{
			await _interpreter.ParsingAsync(gameName);
			_render.LoadScene("TestScene", DateTimeOffset.Now.Ticks / 10000L);
		}
		else
		{
			return;
		}
	}
	#endregion
}