using WebGal.Types;

namespace WebGal.Controller;

class ControllerSliderHorizontal : ControllerSliderBase
{

	public ControllerSliderHorizontal()
	{
		_attributeChange = true;

		InitBase();
		InitAttribute(new(new IVector(100, 300), new IVector(200, 20)), new IVector(10, 20));
		InitImage();
	}

	protected override IVector ThumbLimiter(IVector thumbDelta) => new(Math.Max(0, Math.Min(_size.Width - _thumbSize.Width, thumbDelta.X)), 0);
}