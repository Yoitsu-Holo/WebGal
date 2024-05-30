using WebGal.Types;

namespace WebGal.Layer.Controller;

public class ControllerSliderVertical : ControllerSliderBase
{
	public ControllerSliderVertical()
	{
		_attributeChange = true;
		Position = new IVector(100, 400);

		InitBase();
		InitAttribute(new IVector(20, 200), new IVector(20, 10));
	}

	protected override void ThumbLimitSet(IVector thumbDelta)
	{
		_thumbDelta = new(0, Math.Max(0, Math.Min(Size.Y - _thumbSize.Y, thumbDelta.Y)));
		_value = 1.0f * _thumbDelta.Y / (Size.Y - _thumbSize.Y);
	}
}