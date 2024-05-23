using WebGal.Types;

namespace WebGal.Animations;

/// <summary>
/// Do Nothing
/// </summary>
class AnimationBase : IAnimation
{
	public virtual AnimationData DoAnimation(long timeOff) => new() { PosOff = new(0, 0), };
	public virtual void SetParama(object parama) { }
}