using WebGal.Types;

namespace WebGal.Animations;

class AnimationBounce : AnimationBase
{
	private AnimationBounceData data = new();
	private FVector _pos;
	private double timepre = -1;

	public override AnimationData DoAnimation(long timeOff)
	{
		double timeObs = Global.NowTime.UtcMinisecond;
		if (timepre < 0)
			timepre = timeObs;
		double dt = timeObs - timepre;
		FVector npos = new(_pos.X + data.Delta.X * dt, _pos.Y + data.Delta.Y * dt);

		if (npos.X < 0) { npos.X = 0; data.Delta.X = -data.Delta.X; }
		if (npos.X > data.Range.X) { npos.X = data.Range.X; data.Delta.X = -data.Delta.X; }

		if (npos.Y < 0) { npos.Y = 0; data.Delta.Y = -data.Delta.Y; }
		if (npos.Y > data.Range.Y) { npos.Y = data.Range.Y; data.Delta.Y = -data.Delta.Y; }

		_pos = npos;
		timepre = timeObs;
		return new() { PosOff = _pos };
	}

	public override void SetParama(object parama)
	{
		if (parama is AnimationBounceData p)
			data = p;
	}
}

public record struct AnimationBounceData
{
	public FVector Range;
	public FVector Delta;
}