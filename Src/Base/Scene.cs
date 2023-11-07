using WebGal.Event;
using WebGal.Global;
using WebGal.Types;

namespace WebGal.Libs.Base;

/// <summary>
/// 一系列场景数据，包含多个图层。
/// 被 Scene Manager 设置，被 Render 使用。
/// </summary>
public class Scene
{
	public EventHandler<JumpEventArgs>? OnJump;

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

	private readonly List<(string, MouseEvent, List<ActionStructure>)> _mouseEvents = new();
	private readonly List<(MouseEvent, List<ActionStructure>)> _mouseDefaultEvents = new();

	public void RegitserMouseAction(string trigerLayerName, MouseEvent mouseEvent, List<ActionStructure> actions)
	{
		_mouseEvents.Add((trigerLayerName, mouseEvent, actions));
	}

	public void RegitserMouseDefaultAction(MouseEvent mouseEvent, List<ActionStructure> actions)
	{
		_mouseDefaultEvents.Add((mouseEvent, actions));
	}

	public bool DoMouseEvent(MouseEvent mouseEvent)
	{
		bool trigered = false;
		foreach (var (trigerLayerName, targetEvent, actions) in _mouseEvents)
		{
			if (targetEvent.Button != mouseEvent.Button || targetEvent.Status != mouseEvent.Status)
				continue;

			trigered = true;
			int trigerLayerId = _layersId[trigerLayerName];
			Layer trigerLayer = Layers[trigerLayerId];

			var point = mouseEvent.Position;

			if (RangeComp.OutRange(new(trigerLayer.Pos.X, trigerLayer.Pos.X + trigerLayer.WinSize.Width), point.X) ||
				RangeComp.OutRange(new(trigerLayer.Pos.Y, trigerLayer.Pos.Y + trigerLayer.WinSize.Height), point.Y))
				continue;

			StateHasChange = true;

			foreach (var action in actions)
				DoActions(action);
		}

		if (trigered)
			return trigered;

		// 执行默认动作
		foreach (var (targetEvent, actions) in _mouseDefaultEvents)
		{
			if (targetEvent.Button != mouseEvent.Button || targetEvent.Status != mouseEvent.Status)
				continue;
			trigered = true;
			StateHasChange = true;
			foreach (var action in actions)
				DoActions(action);
		}
		return trigered;
	}

	private void DoActions(ActionStructure action)
	{
		if (action.LayerName is not null)
		{
			var actionLayerId = _layersId[action.LayerName];
			Layers[actionLayerId].DynamicAttribute = action.Attribute;
		}
		if (action.JumpNodeLabel is not null)
		{
			OnJump?.Invoke(this, new JumpEventArgs()
			{
				JumpNodeLabel = action.JumpNodeLabel,
				JumpSceneLabel = action.JumpSceneLabel
			});
			// Console.WriteLine($"jump to {action.JumpNodeLabel}");
		}
	}
}