using WebGal.Types;

namespace WebGal.Animations;

class AnimationBrownian : IAnimation
{
	readonly Random rand = new();
	public AnimationData GetOffset(long timeOff)
	{
		var (x, y) = (rand.NextDouble(), rand.NextDouble());
		return new() { PosOff = new((double)x, (double)y) };
	}
}