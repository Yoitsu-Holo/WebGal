using SkiaSharp;
using WebGal.Global;

namespace WebGal.Libs.Base;

/// <summary>
/// 一系列场景数据，包含多个图层。
/// 被 Scene Manager 设置，被 Render 使用。
/// </summary>
public class Scene
{
	// private (SKBitmap In, SKBitmap Out) _fadeMask;
	// private (float In, float Out) _fadeTime; //ms
	private readonly Dictionary<string, int> _layersId = new();
	private readonly Dictionary<int, string> _layersName = new();
	private int _layerCount = 0;
	private bool _stateHasChange;
	public SortedDictionary<int, Layer> Layers = new();
	public bool IsStatic { get; set; }

	public bool HasAnimation(long timeoff)
	{
		if (_stateHasChange)
		{
			_stateHasChange = false;
			return true;
		}
		bool hasAnimation = false;
		foreach (var (_, layer) in Layers)
			hasAnimation |= layer.HasAnimation(timeoff);
		return hasAnimation;
	}
	// public Dictionary<string, byte[]> LoopAudiosList = new();
	// public Dictionary<string, byte[]> OneShotAudiosList = new();


	public void PushLayer(string name, Layer layer)
	{
		_layersId[name] = _layerCount;
		_layersName[_layerCount] = name;
		Layers[_layerCount] = layer;

		_layerCount++;
	}

	public void Clear()
	{
		Layers.Clear();
		_layersName.Clear();
		_layersId.Clear();
		_layerCount = 0;
	}

	public void SetBeginTime(long beginTime)
	{
		foreach (var (_, layer) in Layers)
			layer.BeginTime = beginTime;
	}

	public void StartAnimation()
	{
		SetBeginTime(NowTime.Minisecond);
		_stateHasChange = true;
	}

	public void StopAnimation()
	{
		SetBeginTime(long.MinValue);
		_stateHasChange = true;
	}

	// public void SetFadeIn(SKBitmap mask, float time) => (_fadeMask.In, _fadeTime.In) = (mask, time);
	// public void SetFadeOut(SKBitmap mask, float time) => (_fadeMask.Out, _fadeTime.Out) = (mask, time);
}