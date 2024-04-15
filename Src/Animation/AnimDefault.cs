using WebGal.Types;

namespace WebGal.Animations;

class AnimationDefault : IAnimation
{
	public AnimationData GetOffset(long timeOff) => new() { PosOff = new(timeOff, timeOff), };
}