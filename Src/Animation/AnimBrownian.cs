using WebGal.Types;

namespace WebGal.Animations;

class AnimationBrownian : IAnimation
{
	readonly Random rand = new();
	public FVector GetOffset(float timeOff, long timeObs)
	{
		var (x, y) = (rand.NextDouble(), rand.NextDouble());
		return new((float)x, (float)y);
	}
}