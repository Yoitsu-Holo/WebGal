using SkiaSharp;
using WebGal.Types;

namespace WebGal.Animations;

class AnimationRotate : IAnimation
{
	private readonly double _z;
	private SKMatrix _matrix = SKMatrix.Identity;
	private double timeStart = -1;


	public AnimationRotate() { }

	public AnimationRotate(double redis)
	{
		_z = redis;
	}

	public AnimationData DoAnimation(long timeOff)
	{
		double timeObs = Global.NowTime.UtcMinisecond;
		if (timeStart < 0)
			timeStart = timeObs;
		double dt = (timeObs - timeStart) / 1000;

		_matrix = SKMatrix.CreateRotation((float)(dt * _z));

		return new() { Transform = _matrix, };
	}
}