using System.Security.Cryptography.X509Certificates;
using WebGal.Types;

namespace WebGal.Global;

record struct Range(int left, int right);

static class RangeComp
{
	public static bool InRange(Range range, int value)
	{
		var compRange = range;
		if (range.left > range.right)
			compRange = new(range.right, range.left);
		return value >= compRange.left && value <= compRange.right;
	}

	public static bool OutRange(Range range, int value)
	{
		return !InRange(range, value);
	}

	public static bool InRange(IRect rect, IVector pos)
	{
		return InRange(new(rect.Left, rect.Right), pos.X) && InRange(new(rect.Top, rect.Bottom), pos.Y);
	}

	public static bool OutRange(IRect rect, IVector pos)
	{
		return !InRange(rect, pos);
	}
}