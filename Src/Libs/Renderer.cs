using System.Data;
using SkiaSharp;
using WebGal.Libs.Base;

namespace WebGal.Libs;
public class Renderer
{
	private SKCanvas _canvas = null!;
	private Scene _scene = new();
	private readonly SceneManager _sceneManager = null!;
	private bool _renderingFlag;


	/// <summary>
	/// 指定场景管理器
	/// </summary>
	/// <param name="sceneManager">场景管理器</param>
	public Renderer(SceneManager sceneManager) => _sceneManager = sceneManager;


	/// <summary>
	/// 从指定时间开始加载一个场景
	/// </summary>
	/// <param name="sceneName">场景名称，在场景管理中被注册</param>
	/// <param name="startTime">开始时间，单位毫秒</param>
	public void LoadScene(string sceneName, long startTime)
	{
		_renderingFlag = true;
		_scene = _sceneManager.LoadScene(sceneName);
		_scene.SetBeginTime(startTime);
	}


	/// <summary>
	/// 获取场景的音频媒体
	/// </summary>
	/// <returns>两个映射，分别是循环播放音乐和单次播放音乐</returns>
	// public (Dictionary<string, byte[]>, Dictionary<string, byte[]>) GetSceneAudioMedia()
	// {
	// 	return (_scene.LoopAudiosList, _scene.OneShotAudiosList);
	// }

	public void Clear()
	{
		_canvas?.Clear();

	}

	public void Render(SKCanvas canvas, long timeoff, bool force)
	{
		if (_canvas != canvas)
		{
			Clear();
			_canvas = canvas;
		}
		if (_renderingFlag || force)
			Rendering(timeoff);
	}


	/// <summary>
	/// 渲染下一帧
	/// </summary>
	/// <param name="timeoff"></param>
	private void Rendering(long timeoff)
	{
		_canvas.Clear();
		foreach (var (layerId, layer) in _scene.Layers)
		{
			layer.GenNextFrame(timeoff);

			if (layer.BackGroundSKBitmap is not null)
				_canvas.DrawBitmap(layer.FrameBuffer, layer.PosAt(timeoff));

			// if ( is List<LayerText> texts)
			foreach (var text in layer.Text)
				_canvas.DrawText(text.Text, layer.AbsolutePos(text.Pos), text.Paint);
		}
		_renderingFlag = _scene.HasAnimation(timeoff);
		_canvas.Flush();
	}

	public void StopAnimation()
	{
		_scene.SetBeginTime(long.MinValue);
	}

	public bool HasAnimation(long timeoff) => _scene.HasAnimation(timeoff);
}
