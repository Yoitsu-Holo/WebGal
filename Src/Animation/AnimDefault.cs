using WebGal.Types;

namespace WebGal.Animations;

class AnimationDefault : IAnimation
{
	public FVector GetOffset(float timeOff, long timeObs) => new(timeOff, timeOff);
}