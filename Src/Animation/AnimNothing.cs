using WebGal.Types;

namespace WebGal.Animations;

class AnimationNothing : IAnimation
{
	public FVector GetOffset(float timeOff, long timeObs) => new(0, 0);
}