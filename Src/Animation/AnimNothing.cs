using WebGal.Types;

namespace WebGal.Animations;

class AnimationNothing : AnimationBase
{
	public override AnimationData DoAnimation(long timeOff) => new();

	public override void SetParama(object parama) { }
}

public record struct AnimationNothingData
{
}