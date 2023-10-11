using WebGal.Animations;

namespace WebGal.Global;

class AnimationRegister
{
	public static IAnimation GetAnimation(string AnimationName)
	{
		return AnimationName switch
		{
			"brownian" => new AnimationBrownian(),
			"bounce" => new AnimationBounce(),
			_ => new AnimationDefault(),
		};
	}
}