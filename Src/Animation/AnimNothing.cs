using WebGal.Types;

namespace WebGal.Animations;

class AnimationNothing : IAnimation
{
	public FVector GetOffset(double timeOff, long timeObs) => new(0, 0);
}