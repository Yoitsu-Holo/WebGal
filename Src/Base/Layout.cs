using WebGal.Global;

namespace WebGal.Libs.Base;

/// <summary>
/// 多个场景的组合，预先定义的场景结构。场景事件也是由此进入处理。
/// </summary>
public class Layout
{

	public List<Scene> StaticScene = []; // 界面静态场景，如按钮、滑块等控制器，或者是文本框等。
	public List<Scene> DynamicScene = []; // 界面动态场景，如对话框文字等。

	public void Clear() => StaticScene.Clear();


	public void SetBeginTime(long beginTime)
	{
		foreach (var (_, layer) in Layers)
			layer.BeginTime = beginTime;
	}

	public void StartAnimation()
	{
		SetBeginTime(NowTime.Minisecond);
		StateHasChange = true;
	}

	public void StopAnimation()
	{
		SetBeginTime(long.MinValue);
		StateHasChange = true;
	}
}