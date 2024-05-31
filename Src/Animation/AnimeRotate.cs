using SkiaSharp;
using WebGal.Types;

namespace WebGal.Animations;

class AnimationRotate : AnimationBase
{
	public AnimationRotateData data;
	private SKMatrix _matrix = SKMatrix.Identity;
	private float timeStart = -1;

	public override AnimationData DoAnimation(long timeOff)
	{
		float timeObs = Global.NowTime.UtcMinisecond;
		if (timeStart < 0)
			timeStart = timeObs;
		float dt = (timeObs - timeStart) / 1000;

		_matrix = SKMatrix.CreateRotation(dt * data.Z);

		return new() { Transform = _matrix, };
	}

	public override void SetParama(object parama)
	{
		if (parama is AnimationRotateData p)
			data = p;
	}
}

public record struct AnimationRotateData
{
	public float Z;
}