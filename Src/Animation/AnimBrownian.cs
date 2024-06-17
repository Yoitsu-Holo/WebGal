using WebGal.Types;

namespace WebGal.Animations;

class AnimationBrownian : AnimationBase
{
	private AnimationBrownianData _data = new();
	private readonly Random rand = new();
	public override void DoAnimation(ref AnimationData data, long timeOff)
	{
		data.PosOff = new((float)rand.NextDouble(), (float)rand.NextDouble());
	}

	public override void SetParama(object parama)
	{
		if (parama is AnimationBrownianData p) _data = p;
	}
}

public record struct AnimationBrownianData
{
}