using SkiaSharp;
using WebGal.Extend;
using WebGal.Types;

namespace WebGal.Layer.Widget;

public class WidgetImageBox : LayerBase
{
	private SKBitmap _imageBuffer = new();
	private SKBitmap? _renderBuffer;

	public override void SetImage(SKBitmap image, int imageId = 0)
	{
		_imageBuffer = image;
		_dirty = true;
	}
	public override void SetImage(SKBitmap image, IRect imageWindow, int imageId = 0)
	{
		_imageBuffer = image.SubBitmap(imageWindow);
		_dirty = true;
	}

	public override void Render(SKCanvas canvas, bool force)
	{
		if (Status == LayerStatus.Unvisable || _imageBuffer.IsNull)
			return;
		if (_dirty || force || _renderBuffer is null)
		{
			_renderBuffer = _imageBuffer.Resize(Size, SKFilterQuality.High);
			_dirty = false;
		}
		canvas.Save();
		canvas.SetMatrix(_animationData.Transform);
		canvas.DrawBitmap(_renderBuffer, Position + (IVector)_animationData.PosOff);
		canvas.Restore();
	}

	public override void ExecuteAction(EventArgs eventArgs) { }

	public override void DoAnimation(long timeOff) => _animationData = Animation.DoAnimation(timeOff);
}