using WebGal.Types;

namespace WebGal.Animations;

class AnimationNothing : IAnimation
{
	public AnimationData GetOffset(long timeOff) => new();
}