using WebGal.Types;

namespace WebGal.Animations;

class AnimationBounce : AnimationBase
{
	private AnimationBounceData _data = new();
	private float timepre = -1;

	public override void DoAnimation(ref AnimationData aniData, long timeOff)
	{
		float timeObs = Global.NowTime.UtcMinisecond;
		if (timepre < 0) timepre = timeObs;
		float dt = timeObs - timepre;

		FVector npos = new(aniData.PosOff.X + _data.Delta.X * dt, aniData.PosOff.Y + _data.Delta.Y * dt);

		if (npos.X < 0) { npos.X = 0; _data.Delta.X = -_data.Delta.X; }
		if (npos.X > _data.Range.X) { npos.X = _data.Range.X; _data.Delta.X = -_data.Delta.X; }

		if (npos.Y < 0) { npos.Y = 0; _data.Delta.Y = -_data.Delta.Y; }
		if (npos.Y > _data.Range.Y) { npos.Y = _data.Range.Y; _data.Delta.Y = -_data.Delta.Y; }

		aniData.PosOff = npos;
		timepre = timeObs;
	}

	public override void SetParama(object parama)
	{
		if (parama is AnimationBounceData p) _data = p;
	}
}

public record struct AnimationBounceData
{
	public FVector Range;
	public FVector Delta;
}