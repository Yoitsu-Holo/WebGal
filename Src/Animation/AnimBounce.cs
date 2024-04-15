using SkiaSharp;
using WebGal.Types;

namespace WebGal.Animations;

class AnimationBounce : IAnimation
{
	public FVector Range;
	public FVector Delta;
	private FVector _pos;
	private SKMatrix _matrix = SKMatrix.Identity;
	private double timepre = -1;


	public AnimationBounce() { }

	public AnimationBounce(FVector range, FVector delta)
	{
		Range = range;
		Delta = delta;
	}

	public AnimationData DoAnimation(long timeOff)
	{
		double timeObs = Global.NowTime.UtcMinisecond;
		if (timepre < 0)
			timepre = timeObs;
		double dt = timeObs - timepre;
		FVector npos = new(_pos.X + Delta.X * dt, _pos.Y + Delta.Y * dt);

		if (npos.X < 0) { npos.X = 0; Delta.X = -Delta.X; }
		if (npos.X > Range.X) { npos.X = Range.X; Delta.X = -Delta.X; }

		if (npos.Y < 0) { npos.Y = 0; Delta.Y = -Delta.Y; }
		if (npos.Y > Range.Y) { npos.Y = Range.Y; Delta.Y = -Delta.Y; }

		_pos = npos;
		timepre = timeObs;
		return new() { PosOff = _pos, Transform = _matrix, };
	}
}