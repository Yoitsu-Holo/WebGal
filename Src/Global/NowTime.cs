namespace WebGal.Global;

static class NowTime
{
	public static long Minisecond => DateTimeOffset.Now.Ticks / 10000L;
}