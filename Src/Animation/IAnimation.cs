using WebGal.Types;

namespace WebGal.Animations;

public interface IAnimation
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="timeOff">时间偏移量，ms为单位</param>
	/// <returns></returns>
	public void DoAnimation(ref AnimationData aniData, long timeOff);
	public void SetParama(object parama);
}
