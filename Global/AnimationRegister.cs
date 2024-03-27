using WebGal.Animations;

namespace WebGal.Global;

class AnimationRegister
{
	public static IAnimation GetAnimation(string AnimationName) => AnimationName switch
	{
		"brownian" => new AnimationBrownian(),
		"bounce" => new AnimationBounce(),
		"noting" => new AnimationNothing(),
		_ => new AnimationDefault(),
	};
}