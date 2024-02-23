using WebGal.Types;

namespace WebGal.Controller;

class ControllerSliderVertical : ControllerSliderBase
{
	public ControllerSliderVertical()
	{
		_attributeChange = true;

		InitBase();
		InitAttribute(new(new IVector(100, 400), new IVector(20, 200)), new IVector(20, 10));
		InitImage();
	}

	protected override IVector ThumbLimiter(IVector thumbDelta) => new(0, Math.Max(0, Math.Min(_size.Height - _thumbSize.Height, thumbDelta.Y)));
}