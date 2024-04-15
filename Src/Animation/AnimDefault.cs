using WebGal.Types;

namespace WebGal.Animations;

class AnimationDefault : IAnimation
{
	public AnimationData DoAnimation(long timeOff) => new() { PosOff = new(timeOff, timeOff), };
}