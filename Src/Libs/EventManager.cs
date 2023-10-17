using SkiaSharp;
using WebGal.Global;

namespace Web.Libs;

public class EventManager
{
	private readonly HashSet<SKRectI> _leftClickEvent = new();
	private readonly HashSet<SKRectI> _rightClickEvent = new();
	private readonly HashSet<SKRectI> _moveOnClickEvent = new();

	public void RegitserLeftClickActionTest(SKRectI range)
	{
		_leftClickEvent.Add(range);
	}

	public void RegitserRightClickActionTest(SKRectI range)
	{
		_rightClickEvent.Add(range);
	}

	public void RegitserMoveOnActionTest(SKRectI range)
	{
		_moveOnClickEvent.Add(range);
	}


	public void OnLeftClick(SKPointI point)
	{
		foreach (var rect in _leftClickEvent)
		{
			if (RangeComp.InRange(new(rect.Left, rect.Right), point.X) && RangeComp.InRange(new(rect.Top, rect.Bottom), point.Y))
				Console.WriteLine("catch left click");
		}
	}

	public void OnRightClick(SKPointI point)
	{
		foreach (var rect in _rightClickEvent)
		{
			if (RangeComp.InRange(new(rect.Left, rect.Right), point.X) && RangeComp.InRange(new(rect.Top, rect.Bottom), point.Y))
				Console.WriteLine("catch right click");
		}
	}

	public void OnMoveOn(SKPointI point)
	{
		foreach (var rect in _moveOnClickEvent)
		{
			if (RangeComp.InRange(new(rect.Left, rect.Right), point.X) && RangeComp.InRange(new(rect.Top, rect.Bottom), point.Y))
				Console.WriteLine("catch move on");
		}
	}
}