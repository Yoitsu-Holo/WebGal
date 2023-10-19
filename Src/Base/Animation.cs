using WebGal.Animations;

namespace WebGal.Libs.Base;
public class Animation
{
	// 动画开始时图层处于的偏移量
	public FVector BeginPosition { get; set; } = new(0, 0);
	// 动画结束后图层处于的偏移量
	public FVector EndPosition { get; set; } = new(0, 0);
	public long DelayTime { get; set; }
	public long BeginTime { get; set; }
	public long EndTime { get => BeginTime + DelayTime; }
	public bool HasAnimation(long timeoff) { return timeoff < EndTime; }

	public IAnimation AnimationClass { get; set; } = new AnimationDefault();

	/// <summary>
	/// 给定绝对时间(毫秒)，计算得出图层偏移量(根据场景创建时)
	/// </summary>
	/// <param name="time">绝对时间</param>
	/// <returns></returns>
	public IVector GetOffset(long time)
	{
		if (DelayTime == 0 || time > EndTime || time < BeginTime)
			return new(0, 0);

		var Offset = AnimationClass.GetOffset((float)(time - BeginTime) / DelayTime, time);
		var delta = EndPosition - BeginPosition;
		return (IVector)(BeginPosition + (Offset * delta));
	}
}