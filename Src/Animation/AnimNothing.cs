using WebGal.Types;

namespace WebGal.Animations;

class AnimationNothing : AnimationBase
{
	public override void DoAnimation(ref AnimationData data, long timeOff) { }
	public override void SetParama(object parama) { }
}

public record struct AnimationNothingData
{
}