using SkiaSharp;
using WebGal.Global;

namespace Web.Libs;

public class EventManager
{
	private readonly HashSet<SKRectI> _rects = new();

	public void RegitserClickActionTest(SKRectI range)
	{
		_rects.Add(range);
	}

	public void OnClick(SKPointI point)
	{
		foreach (var rect in _rects)
		{
			if (RangeComp.InRange(new(rect.Left, rect.Right), point.X) && RangeComp.InRange(new(rect.Top, rect.Bottom), point.Y))
				Console.WriteLine("catch click");
		}
	}
}