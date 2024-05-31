using WebGal.Types;

namespace WebGal.Layer.Controller;

public class ControllerSliderHorizontal : ControllerSliderBase
{
	public ControllerSliderHorizontal()
	{
		_attributeChange = true;
		Position = new IVector(100, 300);

		InitBase();
		InitAttribute(new IVector(200, 20), new IVector(10, 20));

		for (int i = -1; i <= 4; i++)
			_image[i] = new();
	}

	protected override void ThumbLimitSet(IVector thumbDelta)
	{
		_thumbDelta = new(Math.Max(0, Math.Min(Size.X - _thumbSize.X, thumbDelta.X)), 0);
		_value = 1.0f * _thumbDelta.X / (Size.X - _thumbSize.X);
	}
}