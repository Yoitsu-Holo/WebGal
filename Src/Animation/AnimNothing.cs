using WebGal.Types;

namespace WebGal.Animations;

class AnimationNothing : IAnimation
{
	public AnimationData GetOffset(double timeOff, long timeObs) => new();
}