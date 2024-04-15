using WebGal.Types;

namespace WebGal.Animations;

class AnimationDefault : IAnimation
{
	public AnimationData GetOffset(double timeOff, long timeObs) => new() { PosOff = new(timeOff, timeOff), };
}