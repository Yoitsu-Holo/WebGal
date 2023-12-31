using SkiaSharp;
using Microsoft.JSInterop;
using WebGal.Libs.Base;
using WebGal.Global;
using WebGal.Services.Include;
using WebGal.Event;

namespace WebGal.Services;
public class GameManager
{
	private readonly IJSRuntime _js;                        //^ JavaScript 互操作运行时
	private readonly Interpreter _interpreter;              //^ 脚本解释器
	private readonly ResourceManager _resourceManager;      //^ 资源管理器
	private readonly SceneManager _sceneManager = new();    //^ 场景管理器
	private readonly Renderer _render = new();              //^ 渲染器
	private Scene? _scene;

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
		_interpreter = new(_sceneManager, _resourceManager);
	}

	public void Clear()
	{
		_render.Clear();
		_sceneManager.Clear();
		_resourceManager.Clear();
		_interpreter.Clear();
		_scene = null;
	}

	public void Render(SKCanvas canvas, long timeoff, bool force = false)
	{
		if (_scene is null)
		{
			return;
			throw new Exception("render scene not set");
		}
		_render.Render(canvas, _scene, timeoff, force);
	}

	public async Task ProcessMouseEvent(MouseEvent mouseEvent)
	{
		if (_scene is null)
			return;
		if (_scene.DoMouseEvent(mouseEvent))
			return;

		if (mouseEvent.Button == MouseButton.LButton && mouseEvent.Status == MouseStatus.Up)
		{
			await GetNextScene();
			Console.WriteLine("NextScene");
		}
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
		await _interpreter.SetGameAsync(gameName);
		await LoadNextScene();
	}
	#endregion

	private async Task GetNextScene()
	{
		if (_scene is null)
			return;

		// 如果动画没有结束，那么结束动画保留这一帧
		if (_scene.HasAnimation(NowTime.Minisecond))
		{
			_scene.StopAnimation();
			return;
		}

		// 如果当前场景动画结束，切换到下一场景
		await LoadNextScene();
	}

	private async Task LoadNextScene()
	{
		await _interpreter.ParsingNextSceneAsync();
		LoadScene();
		// await LoadMedia();
	}

	private void LoadScene()
	{
		if (_sceneManager.SceneNameList.Count != 0)
		{
			LoadScene(_sceneManager.SceneNameList.Peek());
			_sceneManager.SceneNameList.Dequeue();
		}
	}

	private void LoadScene(string sceneName)
	{
		_sceneName = sceneName;
		_scene = _sceneManager.LoadScene(sceneName);
		_scene.StartAnimation();
		_scene.OnJump = LoadNode;
	}

	private async void LoadNode(object? sender, JumpEventArgs args)
	{
		if (args.JumpNodeLabel is not null)
		{
			Console.WriteLine($"jump to {args.JumpNodeLabel}");
			_interpreter.SetNode(args.JumpNodeLabel);
			await LoadNextScene();
		}
	}
}