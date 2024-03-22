using WebGal.Types;

namespace WebGal.Animations;

public interface IAnimation
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="timeOff">时间偏移量，秒为单位</param>
	/// <param name="timeObs">时间绝对值，ms为单位</param>
	/// <returns></returns>
	public FVector GetOffset(double timeOff, long timeObs);
}
