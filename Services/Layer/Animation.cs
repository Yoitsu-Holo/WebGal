namespace WebGal.Services.Module;

public class Animation
{
	// 动画开始时图层处于的偏移量
	public (float X, float Y) BeginPosition { get; set; } = new(0, 0);
	// 动画结束后图层处于的偏移量
	public (float X, float Y) EndPosition { get; set; } = new(0, 0);
	public long DelayTime { get; set; }
	public long BeginTime { get; set; }
	public long EndTime { get => BeginTime + DelayTime; }

	public Func<float, (float, float)> AniFunction = d => (d, d);

	public bool HasAnimation(long timeoff) { return timeoff < EndTime; }


	/// <summary>
	/// 给定绝对时间(毫秒)，计算得出图层偏移量(根据场景创建时)
	/// </summary>
	/// <param name="time">绝对时间</param>
	/// <returns></returns>
	public (int X, int Y) GetOffset(long time)
	{
		if (DelayTime == 0 || time > EndTime || time < BeginTime)
			return (0, 0);
		var (dx, dy) = AniFunction((float)(time - BeginTime) / DelayTime);
		float DeltaX() => EndPosition.X - BeginPosition.X;
		float DeltaY() => EndPosition.Y - BeginPosition.Y;
		return ((int)(BeginPosition.X + (DeltaX() * dx)), (int)(BeginPosition.Y + (DeltaY() * dy)));
	}
}

class AnimationList
{
	public Dictionary<string, Func<float, (float, float)>> Functions = new()
	{
		{"Default", d => (d, d)},
	};
}