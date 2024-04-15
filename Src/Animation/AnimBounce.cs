using WebGal.Types;

namespace WebGal.Animations;

class AnimationBounce : IAnimation
{
	private float x = 0, y = 0;
	private float dx = 1.5F, dy = -0.79F;
	private double timepre = -1;
	public AnimationData GetOffset(long timeOff)
	{
		double timeObs = Global.NowTime.UtcMinisecond / 1000f;
		if (timepre < 0)
			timepre = timeObs;
		float dt = (float)(timeObs - timepre) / 1000;
		var (nx, ny) = (x + dx * dt, y + dy * dt);

		if (nx > 1 || nx < 0) dx = -dx;
		if (ny > 1 || ny < 0) dy = -dy;

		if (x < 0) x = 0;
		if (x > 1) x = 1;
		if (y < 0) y = 0;
		if (y > 1) y = 1;

		(x, y) = (x + dx * dt, y + dy * dt);

		timepre = timeObs;
		return new() { PosOff = new(x, y), };
	}
}