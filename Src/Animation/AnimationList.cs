namespace WebGal.Animations;

public interface IAnimation
{
	public (float dx, float dy) GetOffset(float timeOff, long timeObs);
}

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
	private float dx = 1.5F, dy = -0.5F;
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