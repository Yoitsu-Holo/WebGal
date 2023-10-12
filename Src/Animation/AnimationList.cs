namespace WebGal.Animations;

class AnimationDefault : IAnimation
{
	public (float dx, float dy) GetOffset(float timeOff, long timeObs)
	{
		return (timeOff, timeOff);
	}
}

class AnimationBounce : IAnimation
{
	private float x = 0, y = 0;
	private float dx = 1.5F, dy = -0.79F;
	private long timepre = -1;
	public (float dx, float dy) GetOffset(float timeOff, long timeObs)
	{
		if (timepre == -1)
			timepre = timeObs;
		float dt = (float)(timeObs - timepre) / 1000;
		var (nx, ny) = (x + dx * dt, y + dy * dt);

		if (nx > 1 || nx < 0)
			dx = -dx;
		if (ny > 1 || ny < 0)
			dy = -dy;

		(x, y) = (x + dx * dt, y + dy * dt);

		timepre = timeObs;
		return (x, y);
	}
}

class AnimationBrownian : IAnimation
{
	readonly Random rand = new();
	public (float dx, float dy) GetOffset(float timeOff, long timeObs)
	{
		var (x, y) = (rand.NextDouble(), rand.NextDouble());
		return ((float)x, (float)y);
	}
}