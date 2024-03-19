using WebGal.Types;

namespace WebGal.Layer.Controller;

public class ControllerSliderVertical : ControllerSliderBase
{
	public ControllerSliderVertical()
	{
		_attributeChange = true;

		InitBase();
		InitAttribute(new(new IVector(100, 400), new IVector(20, 200)), new IVector(20, 10));
		InitImage();
	}

	protected override void ThumbLimitSet(IVector thumbDelta)
	{
		_thumbDelta = new(0, Math.Max(0, Math.Min(Size.Height - _thumbSize.Height, thumbDelta.Y)));
		_value = 1.0f * _thumbDelta.Height / (Size.Height - _thumbSize.Height);
	}
}