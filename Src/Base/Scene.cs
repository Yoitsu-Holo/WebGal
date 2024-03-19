using WebGal.Global;

namespace WebGal.Libs.Base;

/// <summary>
/// 一系列场景数据，包含多个图层。
/// 被 Scene Manager 设置，被 Render 使用。
/// </summary>
public class Scene
{
	public bool StateHasChange = true;


	public readonly HashSet<string> LoopAudioSet = [];
	public readonly HashSet<string> OneShotAudioSet = [];


	public SortedDictionary<int, Layer> Layers = new();

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

	public void PushLayer(int layerId, Layer layer) => Layers[layerId] = layer;
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