namespace WebGal.Services.Module;

public class Animation
{
	private float DeltaX => EndPosition.X - BeginPosition.X;
	private float DeltaY => EndPosition.Y - BeginPosition.Y;

	// 动画开始时图层处于的偏移量
	public (float X, float Y) BeginPosition { get; set; } = new(0, 0);
	// 动画结束后图层处于的偏移量
	public (float X, float Y) EndPosition { get; set; } = new(0, 0);
	public long DelayTime;
	public long BeginTime;
	public long EndTime { get => BeginTime + DelayTime; }
	public AnimationFunction AniFunction = new();

	public bool HasAnimation(long timeoff) { return timeoff < EndTime; }
	public (int X, int Y) GetOffset(long time)
	{
		if (DelayTime == 0 || time > EndTime || time < BeginTime)
			return (0, 0);
		var (dx, dy) = AniFunction.GetDelta((float)(time - BeginTime) / DelayTime);
		// Console.WriteLine($"{nameof(Animation)}:{time - BeginTime},{DelayTime}:{dx},{dy}");
		return ((int)(BeginPosition.X + (DeltaX * dx)), (int)(BeginPosition.Y + (DeltaY * dy)));
	}

}

public class AnimationFunction
{
	public virtual (float X, float Y) GetDelta(float deltaTime)
	{
		return (deltaTime, deltaTime);
	}
}