using SkiaSharp;
using WebGal.Global;
using WebGal.Layer;

namespace WebGal.Libs.Base;

/// <summary>
/// 多个场景的组合，预先定义的场景结构。场景事件也是由此进入处理。
/// </summary>
public class Layout
{
	private long StartTime { get; set; } = 0;

	public int SceneId { get; set; } = -1;

	public SortedDictionary<int, ILayer> Layers { get; set; } = [];    // 界面动态场景，如对话框文字等。

	public void Clear()
	{
		Layers.Clear();
	}

	public void SetStart() => StartTime = NowTime.Minisecond;
	public void SetStop() => StartTime = -1;

	public bool ShouldRender()
	{
		if (StartTime == 0)
			StartTime = NowTime.Minisecond;
		long timeOff = NowTime.Minisecond - StartTime;

		foreach (var (_, layer) in Layers)
			if (layer.HasAnimation(timeOff))
				return true;
		return false;
	}

	public void Render(SKCanvas canvas, bool force)
	{
		if (StartTime == 0)
			StartTime = NowTime.Minisecond;
		long timeOff = NowTime.Minisecond - StartTime;

		foreach (var (_, layer) in Layers)
		{
			layer.DoAnimation(timeOff);
			layer.Render(canvas, force);
		}
	}

	public void ProcessEvent(EventArgs eventArgs)
	{
		foreach (var (_, layer) in Layers)
			layer.Action(this, eventArgs);
	}
}