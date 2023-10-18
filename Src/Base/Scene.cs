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
	public bool StateHasChange;
	public SortedDictionary<int, Layer> Layers = new();
	public bool IsStatic { get; set; }

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
		StateHasChange = true;
	}

	public void StopAnimation()
	{
		SetBeginTime(long.MinValue);
		StateHasChange = true;
	}

	// public void SetFadeIn(SKBitmap mask, float time) => (_fadeMask.In, _fadeTime.In) = (mask, time);
	// public void SetFadeOut(SKBitmap mask, float time) => (_fadeMask.Out, _fadeTime.Out) = (mask, time);


	// Action
	private readonly Dictionary<SKRectI, List<ActionTriger>> _leftClickAction = new();
	private readonly Dictionary<SKRectI, List<ActionTriger>> _rightClickAction = new();
	private readonly Dictionary<SKRectI, List<ActionTriger>> _moveOnAction = new();
	public void RegitserLeftClickAction(SKRectI range, ActionTriger action)
	{
		if (_leftClickAction.ContainsKey(range) == false)
			_leftClickAction[range] = new();
		_leftClickAction[range].Add(action);
	}

	public void RegitserRightClickAction(SKRectI range, ActionTriger action)
	{
		if (_rightClickAction.ContainsKey(range) == false)
			_rightClickAction[range] = new();
		_rightClickAction[range].Add(action);
	}

	public void RegitserMoveOnAction(SKRectI range, ActionTriger action)
	{
		if (_moveOnAction.ContainsKey(range) == false)
			_moveOnAction[range] = new();
		_moveOnAction[range].Add(action);
	}

	public void OnLeftClick(SKPointI point)
	{
		foreach (var (rect, actions) in _leftClickAction)
		{
			if (RangeComp.OutRange(new(rect.Left, rect.Right), point.X) || RangeComp.OutRange(new(rect.Top, rect.Bottom), point.Y))
				continue;
			StateHasChange = true;
			foreach (var action in actions)
				SetActoin(action);
			Console.WriteLine("Scene: catch left click");
		}
	}

	public void OnRightClick(SKPointI point)
	{
		foreach (var (rect, actions) in _rightClickAction)
		{
			if (RangeComp.OutRange(new(rect.Left, rect.Right), point.X) || RangeComp.OutRange(new(rect.Top, rect.Bottom), point.Y))
				continue;
			StateHasChange = true;
			foreach (var action in actions)
				SetActoin(action);

			Console.WriteLine("Scene: catch right click");
		}
	}

	public void OnMoveOn(SKPointI point)
	{
		foreach (var (rect, actions) in _moveOnAction)
		{
			if (RangeComp.OutRange(new(rect.Left, rect.Right), point.X) || RangeComp.OutRange(new(rect.Top, rect.Bottom), point.Y))
				continue;
			StateHasChange = true;

			foreach (var action in actions)
				SetActoin(action);
			Console.WriteLine("Scene: catch move on");
		}
	}

	private void SetActoin(ActionTriger action)
	{
		if (action.LayerName is null)
			throw new Exception("Scene Action: action layer Name not set");
		var layerId = _layersId[action.LayerName];

		bool isHide = action.IsHide;
		LayerAtrribute layerAtrribute = new()
		{
			IsHide = isHide,
		};

		Layers[layerId].DynamicAttribute = layerAtrribute;
		Console.WriteLine($"{action.LayerName}:{action.IsHide}");
	}
}