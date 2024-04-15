using WebGal.Types;

namespace WebGal.Animations;

class AnimationBrownian : IAnimation
{
	readonly Random rand = new();
	public AnimationData GetOffset(double timeOff, long timeObs)
	{
		var (x, y) = (rand.NextDouble(), rand.NextDouble());
		return new() { PosOff = new((double)x, (double)y) };
	}
}