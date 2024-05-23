using WebGal.Types;

namespace WebGal.Animations;

class AnimationBrownian : AnimationBase
{
	private AnimationBrownianData data = new();
	private readonly Random rand = new();
	public override AnimationData DoAnimation(long timeOff)
	{
		var (x, y) = (rand.NextDouble(), rand.NextDouble());
		return new() { PosOff = new((double)x, (double)y) };
	}

	public override void SetParama(object parama)
	{
		if (parama is AnimationBrownianData p)
			data = p;
	}
}

public record struct AnimationBrownianData
{
}