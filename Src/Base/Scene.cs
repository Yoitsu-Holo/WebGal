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
	private readonly Dictionary<string, List<ActionStructure>> _leftClickAction = new();
	private readonly Dictionary<string, List<ActionStructure>> _rightClickAction = new();
	private readonly Dictionary<string, List<ActionStructure>> _holdAction = new();
	private readonly Dictionary<string, List<ActionStructure>> _moveOnAction = new();

	public void RegitserLeftClickAction(string trigerLayerName, ActionStructure action)
	{
		if (_leftClickAction.ContainsKey(trigerLayerName) == false)
			_leftClickAction[trigerLayerName] = new();
		_leftClickAction[trigerLayerName].Add(action);
	}

	public void RegitserRightClickAction(string trigerLayerName, ActionStructure action)
	{
		if (_rightClickAction.ContainsKey(trigerLayerName) == false)
			_rightClickAction[trigerLayerName] = new();
		_rightClickAction[trigerLayerName].Add(action);
	}

	public void RegitserMoveOnAction(string trigerLayerName, ActionStructure action)
	{
		if (_moveOnAction.ContainsKey(trigerLayerName) == false)
			_moveOnAction[trigerLayerName] = new();
		_moveOnAction[trigerLayerName].Add(action);
	}

	public void RegitserHoldAction(string trigerLayerName, ActionStructure action)
	{
		if (_holdAction.ContainsKey(trigerLayerName) == false)
			_holdAction[trigerLayerName] = new();
		_holdAction[trigerLayerName].Add(action);
	}

	public void OnLeftClick(SKPointI point)
	{
		foreach (var (trigerLayerName, actions) in _leftClickAction)
		{
			int trigerLayerId = _layersId[trigerLayerName];
			Layer trigerLayer = Layers[trigerLayerId];

			if (RangeComp.OutRange(new(trigerLayer.Pos.X, trigerLayer.Pos.X + trigerLayer.WinSize.Width), point.X) ||
				RangeComp.OutRange(new(trigerLayer.Pos.Y, trigerLayer.Pos.Y + trigerLayer.WinSize.Height), point.Y))
				continue;
			StateHasChange = true;
			foreach (var action in actions)
				DoActoin(action);
		}
	}

	public void OnRightClick(SKPointI point)
	{
		foreach (var (trigerLayerName, actions) in _rightClickAction)
		{
			int trigerLayerId = _layersId[trigerLayerName];
			Layer trigerLayer = Layers[trigerLayerId];

			if (RangeComp.OutRange(new(trigerLayer.Pos.X, trigerLayer.Pos.X + trigerLayer.WinSize.Width), point.X) ||
				RangeComp.OutRange(new(trigerLayer.Pos.Y, trigerLayer.Pos.Y + trigerLayer.WinSize.Height), point.Y))
				continue;
			StateHasChange = true;
			foreach (var action in actions)
				DoActoin(action);
		}
	}

	public void OnMoveOn(SKPointI point)
	{
		foreach (var (trigerLayerName, actions) in _moveOnAction)
		{
			int trigerLayerId = _layersId[trigerLayerName];
			Layer trigerLayer = Layers[trigerLayerId];

			if (RangeComp.OutRange(new(trigerLayer.Pos.X, trigerLayer.Pos.X + trigerLayer.WinSize.Width), point.X) ||
				RangeComp.OutRange(new(trigerLayer.Pos.Y, trigerLayer.Pos.Y + trigerLayer.WinSize.Height), point.Y))
				continue;
			StateHasChange = true;

			foreach (var action in actions)
				DoActoin(action);
		}
	}

	public void OnHold(SKPointI point)
	{
		foreach (var (trigerLayerName, actions) in _holdAction)
		{
			int trigerLayerId = _layersId[trigerLayerName];
			Layer trigerLayer = Layers[trigerLayerId];

			if (RangeComp.OutRange(new(trigerLayer.Pos.X, trigerLayer.Pos.X + trigerLayer.WinSize.Width), point.X) ||
				RangeComp.OutRange(new(trigerLayer.Pos.Y, trigerLayer.Pos.Y + trigerLayer.WinSize.Height), point.Y))
				continue;
			StateHasChange = true;

			foreach (var action in actions)
				DoActoin(action);
		}
	}

	private void DoActoin(ActionStructure action)
	{
		if (action.LayerName is null)
			throw new Exception("Scene Action: action layer Name not set");
		var layerId = _layersId[action.LayerName];

		Layers[layerId].DynamicAttribute = action.Attribute;
	}
}