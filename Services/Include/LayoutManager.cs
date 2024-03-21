using WebGal.Event;
using WebGal.Libs.Base;
using WebGal.Types;

namespace WebGal.Services.Include;

/// <summary>
/// 用于表示游戏中的所有界面。
/// 所有的界面都应该在LayoutManager中注册，例如主界面，菜单，游戏内ADV场景。
/// 事件处理流程：LayoutManeger -> Layout -> Scene -> Layer[最终] -> Scene[自身] -> Layout[自身] -> LayoutManeger[自身]
/// </summary>
public class LayoutManager
{
	public readonly Dictionary<string, Layout> Layouts = [];

	public void Clear()
	{
		Layouts.Clear();
	}

	// Action
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
			Libs.Base.Scene trigerLayer = Layers[trigerLayerId];

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

	public bool DoKeyboradEvent(KeyboardEvent keyboardEvent)
	{
		return true;
	}
}