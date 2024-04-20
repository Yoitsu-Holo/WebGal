using WebGal.Types;

namespace WebGal.Animations;

class AnimationBrownian : IAnimation
{
	private AnimationBrownianData data = new();
	private readonly Random rand = new();
	public AnimationData DoAnimation(long timeOff)
	{
		var (x, y) = (rand.NextDouble(), rand.NextDouble());
		return new() { PosOff = new((double)x, (double)y) };
	}

	public void SetParama(object parama)
	{
		if (parama is AnimationBrownianData p)
			data = p;
	}
}

public record struct AnimationBrownianData
{
}