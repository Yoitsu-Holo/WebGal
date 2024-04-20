using WebGal.Types;

namespace WebGal.Animations;

class AnimationNothing : IAnimation
{
	public AnimationData DoAnimation(long timeOff) => new();

	public void SetParama(object parama) { }
}

public record struct AnimationNothingData
{
}