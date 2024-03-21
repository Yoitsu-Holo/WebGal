using WebGal.Global;

namespace WebGal.Libs.Base;

/// <summary>
/// 多个场景的组合，预先定义的场景结构。场景事件也是由此进入处理。
/// </summary>
public class Layout
{
	public bool StateHasChange = true;


	public readonly HashSet<string> LoopAudioSet = [];
	public readonly HashSet<string> OneShotAudioSet = [];


	public SortedDictionary<int, Scene> Layers = new();

	public bool HasAnimation(long timeoff)
	{
		if (StateHasChange)
		{
			StateHasChange = false;
			return true;
		}
		bool hasAnimation = false;
		foreach (var (_, layer) in Layers)
			hasAnimation |= layer.HasAnimation(timeoff);
		return hasAnimation;
	}

	public void PushLayer(int layerId, Scene layer) => Layers[layerId] = layer;
	public void Clear() => Layers.Clear();


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