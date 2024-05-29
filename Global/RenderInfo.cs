namespace WebGal.Global;
public class RenderInfo
{
	private static readonly List<long> _renderTime = [];
	private static bool _recordInfo = false;

	public static void Record(long time)
	{
		if (_recordInfo)
			_renderTime.Add(time);
	}

	public static void SatrtRecord(bool flag = true) => _recordInfo = flag;

	public static void Clear() => _renderTime.Clear();

	public static List<long> GetRecord() => _renderTime;
}