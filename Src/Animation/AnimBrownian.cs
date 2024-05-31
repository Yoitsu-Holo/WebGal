using WebGal.Types;

namespace WebGal.Animations;

class AnimationBrownian : AnimationBase
{
	private AnimationBrownianData data = new();
	private readonly Random rand = new();
	public override AnimationData DoAnimation(long timeOff)
	{
		var (x, y) = ((float)rand.NextDouble(), (float)rand.NextDouble());
		return new() { PosOff = new(x, y) };
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