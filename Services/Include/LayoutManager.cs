using SkiaSharp;
using WebGal.Handler;
using WebGal.Libs.Base;

namespace WebGal.Services.Include;

/// <summary>
/// 用于表示游戏中的所有界面。
/// 所有的界面都应该在LayoutManager中注册，例如主界面，菜单，游戏内ADV场景。
/// 事件处理流程：LayoutManeger -> Layout -> Scene -> Layer[最终] -> Scene[自身] -> Layout[自身] -> LayoutManeger[自身]
/// </summary>
public class LayoutManager
{
	public readonly Dictionary<int, Layout> Layouts = [];
	public int ActiveLayout = 0; // 0: 主界面，-1: 测试界面

	public void Clear() => Layouts.Clear();

	public bool ShouldRender()
	{
		if (Layouts.TryGetValue(ActiveLayout, out Layout? layout))
			if (layout.ShouldRender())
				return true;
		return false;
	}

	public void Render(SKCanvas canvas, bool force)
	{
		if (Layouts.TryGetValue(ActiveLayout, out Layout? layout))
			layout.Render(canvas, force);
	}

	public void ProcessEvent(EventArgs eventdata)
	{
		if (Layouts.TryGetValue(ActiveLayout, out Layout? value))
			value.ProcessEvent(eventdata);
	}
}