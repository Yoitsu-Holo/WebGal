using SkiaSharp;
using Microsoft.JSInterop;
using WebGal.Services.Include;

namespace WebGal.Services;
public class GameManager
{
	private readonly IJSRuntime _js;                   //^ JavaScript 互操作运行时
	private readonly LayoutManager _layoutManeger;     //^ 界面管理器
	private readonly AudioManager _audioManager;       //^ 音频管理器
	private readonly ResourceManager _resourceManager; //^ 资源管理器

	/// <summary>
	/// 构造函数，由系统执行依赖注入
	/// </summary>
	/// <param name="httpClient"></param>
	/// <param name="js"></param>
	public GameManager(HttpClient httpClient, LayoutManager layoutManager, AudioManager audioManager, IJSRuntime js)
	{
		_js = js;
		_layoutManeger = layoutManager;
		_audioManager = audioManager;
		_resourceManager = new(httpClient);
	}

	public void Clear()
	{
		// _layoutManeger.Clear();
		_resourceManager.Clear();
	}

	public void Render(SKCanvas canvas, bool force = false)
	{
		// _layoutManeger.Render(canvas, force);
	}

	public async Task ProcEvent(EventArgs eventArgs)
	{
		await Task.Run(() => { }); // Just make compiler happy
		_layoutManeger.ProcessEvent(eventArgs);
	}

	/// <summary>
	/// 加载媒体资源
	/// </summary>
	/// <exception cref="Exception">未正确设置媒体资源列表</exception>
	public async Task LoadMedia()
	{
		// todo
		await Task.Run(() => { }); // Just make compiler happy
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
		await Task.Run(() => { }); // Just make compiler happy
								   // _layoutManeger.BuildTest();
								   // _layoutManeger.DoTest();
	}
	#endregion
}