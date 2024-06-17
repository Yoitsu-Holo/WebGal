using SkiaSharp;
using WebGal.Types;

namespace WebGal.Animations;

class AnimationRotate : AnimationBase
{
	public AnimationRotateData _data;
	private SKMatrix _matrix = SKMatrix.Identity;
	private float timeStart = -1;

	public override void DoAnimation(ref AnimationData aniData, long timeOff)
	{
		float timeObs = Global.NowTime.UtcMinisecond;
		if (timeStart < 0)
			timeStart = timeObs;
		float dt = (timeObs - timeStart) / 1000;

		aniData.Transform = SKMatrix.Concat(aniData.Transform, SKMatrix.CreateRotation(dt * _data.Z));
	}

	public override void SetParama(object parama)
	{
		if (parama is AnimationRotateData p) _data = p;
	}
}

public record struct AnimationRotateData
{
	public float Z;
}