namespace WebGal.Global;

static class NowTime
{
	public static int Second => DateTime.Now.Second;
	public static long Minisecond => DateTimeOffset.Now.Ticks / 10000L;
	public static long Tick => DateTimeOffset.Now.Ticks;
	public static int UtcSecond => DateTime.UtcNow.Second;
	public static long UtcMinisecond => DateTime.UtcNow.Ticks / 10000L;
	public static long UtcTick => DateTime.UtcNow.Ticks;
}