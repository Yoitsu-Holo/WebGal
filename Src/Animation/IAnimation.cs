using WebGal.Libs.Base;

namespace WebGal.Animations;

public interface IAnimation
{
	public FVector GetOffset(float timeOff, long timeObs);
}
