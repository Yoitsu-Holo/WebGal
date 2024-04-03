using WebGal.Types;

namespace WebGal.Layer.Controller;

public class ControllerSliderHorizontal : ControllerSliderBase
{

	public ControllerSliderHorizontal()
	{
		_attributeChange = true;

		InitBase();
		InitAttribute(new(new IVector(100, 300), new IVector(200, 20)), new IVector(10, 20));
		InitImage();
	}

	protected override void ThumbLimitSet(IVector thumbDelta)
	{
		_thumbDelta = new(Math.Max(0, Math.Min(Size.X - _thumbSize.X, thumbDelta.X)), 0);
		_value = 1.0f * _thumbDelta.X / (Size.X - _thumbSize.X);
	}
}