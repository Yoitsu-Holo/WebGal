using WebGal.Types;

namespace WebGal.Animations;

class AnimationDefault : IAnimation
{
	public FVector GetOffset(double timeOff, long timeObs) => new(timeOff, timeOff);
}