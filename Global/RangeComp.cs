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
}