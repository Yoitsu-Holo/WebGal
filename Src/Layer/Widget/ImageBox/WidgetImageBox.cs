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
	public override void SetImage(SKBitmap image, IVector satrtPosition, int imageId = 0)
	{
		_imageBuffer = image.SubBitmap(new IRect(satrtPosition, Size));
		_dirty = true;
	}

	public override void Render(SKCanvas canvas, bool force)
	{
		if (Status == LayerStatus.Unvisable || _imageBuffer.IsNull)
			return;
		if (_dirty || force || _renderBuffer is null)
		{
			// throw new Exception("Test Error");
			_renderBuffer = _imageBuffer.Resize(Size, SKFilterQuality.High);
			_dirty = false;
		}
		canvas.DrawBitmap(_renderBuffer, Position);
	}

	public override void ExecuteAction(EventArgs eventArgs) { }
}