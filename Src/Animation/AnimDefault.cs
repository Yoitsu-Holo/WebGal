using WebGal.Types;

namespace WebGal.Animations;

class AnimationDefault : IAnimation
{
	private AnimationDefaultData data = new();
	public AnimationData DoAnimation(long timeOff) => new() { PosOff = new(timeOff, timeOff), };

	public void SetParama(object parama)
	{
		if (parama is AnimationDefaultData p)
			data = p;
	}
}

public record struct AnimationDefaultData
{
}