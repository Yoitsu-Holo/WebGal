using WebGal.Types;

namespace WebGal.Animations;

public interface IAnimation
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="timeOff">时间偏移量，ms为单位</param>
	/// <returns></returns>
	public AnimationData DoAnimation(long timeOff);
	public void SetParama(object parama);
}
