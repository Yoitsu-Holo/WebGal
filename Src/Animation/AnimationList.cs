using WebGal.Libs.Base;

namespace WebGal.Animations;

class AnimationDefault : IAnimation
{
	public FVector GetOffset(float timeOff, long timeObs) => new(timeOff, timeOff);
}

class AnimationBounce : IAnimation
{
	private float x = 0, y = 0;
	private float dx = 1.5F, dy = -0.79F;
	private long timepre = -1;
	public FVector GetOffset(float timeOff, long timeObs)
	{
		if (timepre == -1)
			timepre = timeObs;
		float dt = (float)(timeObs - timepre) / 1000;
		var (nx, ny) = (x + dx * dt, y + dy * dt);

		if (nx > 1 || nx < 0)
			dx = -dx;
		if (ny > 1 || ny < 0)
			dy = -dy;

		if (x < 0)
			x = 0;
		if (x > 1)
			x = 1;
		if (y < 0)
			y = 0;
		if (y > 1)
			y = 1;

		(x, y) = (x + dx * dt, y + dy * dt);

		timepre = timeObs;
		return new(x, y);
	}
}

class AnimationBrownian : IAnimation
{
	readonly Random rand = new();
	public FVector GetOffset(float timeOff, long timeObs)
	{
		var (x, y) = (rand.NextDouble(), rand.NextDouble());
		return new((float)x, (float)y);
	}
}