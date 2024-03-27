using SkiaSharp;
using WebGal.Global;
using WebGal.Handler;
using WebGal.Layer;

namespace WebGal.Libs.Base;

/// <summary>
/// 多个场景的组合，预先定义的场景结构。场景事件也是由此进入处理。
/// </summary>
public class Layout
{
	private long StartTime { get; set; } = 0;

	public int SceneId { get; set; } = -1;

	public Scene StstcScene { get; set; } = new();      // 界面静态场景，如按钮、滑块等控制器，或者是文本框等。
	public SortedDictionary<int, Scene> DynamicScene { get; set; } = [];    // 界面动态场景，如对话框文字等。

	public void Clear()
	{
		StstcScene.Clear();
		DynamicScene.Clear();
	}

	public void SetStart() => StartTime = NowTime.Minisecond;
	public void SetStop() => StartTime = -1;

	public void Render(SKCanvas canvas)
	{
		if (StartTime == 0)
			StartTime = NowTime.Minisecond;
		long timeOff = NowTime.Minisecond - StartTime;

		foreach (var layer in AllScene())
		{
			layer.DoAnimation(timeOff);
			layer.Render(canvas);
		}
	}

	public void ProcessEvent(EventArgs eventArgs)
	{
		// throw new NotImplementedException();
	}


	private IEnumerable<ILayer> AllScene()
	{
		if (SceneId == -1)
			yield break;
		IEnumerator<KeyValuePair<int, ILayer>> staticLayer = StstcScene.Layers.GetEnumerator();
		IEnumerator<KeyValuePair<int, ILayer>> dynamicLayer = DynamicScene[SceneId].Layers.GetEnumerator();

		while (staticLayer != null && dynamicLayer != null && (staticLayer.Current.Value != null || dynamicLayer.Current.Value != null))
		{
			if (staticLayer.Current.Value is null)
			{
				if (dynamicLayer.Current.Value != null)
					yield return dynamicLayer.Current.Value;
				dynamicLayer.MoveNext();
				continue;
			}

			if (dynamicLayer.Current.Value is null)
			{
				if (staticLayer.Current.Value != null)
					yield return staticLayer.Current.Value;
				staticLayer.MoveNext();
				continue;
			}

			if (staticLayer.Current.Key < dynamicLayer.Current.Key)
			{
				yield return staticLayer.Current.Value;
				staticLayer.MoveNext();
			}
			else
			{
				yield return dynamicLayer.Current.Value;
				dynamicLayer.MoveNext();
			}
		}
	}

	public void BuildTest()
	{

	}
}