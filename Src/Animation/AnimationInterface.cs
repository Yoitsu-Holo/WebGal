namespace WebGal.Animations;

public interface IAnimation
{
	public (float dx, float dy) GetOffset(float timeOff, long timeObs);
}
