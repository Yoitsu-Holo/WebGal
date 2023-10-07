namespace WebGal.Services;

using SkiaSharp;
using WebGal.Global;
using WebGal.Services.Module;

public class Render
{
	#region Matedata
	// public SKSizeI Resolution { get; set; } = new(
	// 	SceneConfig.DefaultWidth,
	// 	SceneConfig.DefaultHeight
	// );
	#endregion


	#region render Data
	// private readonly SKBitmap _buffer = new(
	// 	SceneConfig.DefaultWidth,
	// 	SceneConfig.DefaultHeight,
	// 	SceneConfig.DefaultColorType,
	// 	SceneConfig.DefaultAlphaType
	// );

	private SKCanvas _canvas = null!;
	private Scene _scene = new();
	private readonly SceneManager _sceneManager = null!;

	#endregion

	/// <summary>
	/// 指定场景管理器
	/// </summary>
	/// <param name="sceneManager">场景管理器</param>
	public Render(SceneManager sceneManager) => _sceneManager = sceneManager;


	/// <summary>
	/// 设置绘画图层
	/// </summary>
	/// <param name="canvas">目标图层</param>
	public void SetCanvas(SKCanvas canvas) => _canvas = canvas;


	/// <summary>
	/// 从指定时间开始加载一个场景
	/// </summary>
	/// <param name="sceneName">场景名称，在场景管理中被注册</param>
	/// <param name="startTime">开始时间，单位毫秒</param>
	public void LoadScene(string sceneName, long startTime)
	{
		_scene = _sceneManager.LoadScene(sceneName);
		_scene.SetBeginTime(startTime);
	}


	/// <summary>
	/// 设置要渲染的画布
	/// </summary>
	/// <param name="canvas"></param>
	public void SetTargetCanvas(SKCanvas canvas)
	{
		if (_canvas != canvas)
		{
			_canvas = canvas;
			_scene.RenderFlag = true;
		}
	}


	/// <summary>
	/// 渲染下一帧，并且写入到目标画布里面
	/// </summary>
	/// <param name="canvas">目标画布</param>
	/// <param name="timeoff">时间差</param>
	/// <param name="force">强制渲染选项</param>
	public void GetNextFrame(long timeoff, bool force = false)
	{
		if (_scene.RenderFlag || force)
			Rendering(timeoff);
	}


	/// <summary>
	/// 获取场景的音频媒体
	/// </summary>
	/// <returns>两个映射，分别是循环播放音乐和单次播放音乐</returns>
	public (Dictionary<string, byte[]>, Dictionary<string, byte[]>) GetSceneAudioMedia()
	{
		return (_scene.LoopAudiosList, _scene.OneShotAudiosList);
	}


	/// <summary>
	/// 渲染下一帧
	/// </summary>
	/// <param name="timeoff"></param>
	private void Rendering(long timeoff)
	{
		_canvas.Clear();

		_scene.RenderFlag = false;
		foreach (var (layerId, layer) in _scene.Layers)
		{
			layer.GenNextFrame(timeoff);

			if (layer.BackGroundSKBitmap is not null)
				_canvas.DrawBitmap(layer.FrameBuffer, layer.PosAt(timeoff));

			if (layer.Text is List<LayerText> texts)
				foreach (var text in texts)
					_canvas.DrawText(text.Text, layer.AbsolutePos(text.Pos), text.Paint);

			_scene.RenderFlag |= layer.Anim.HasAnimation(timeoff);
		}

		_canvas.Flush();
	}
}
