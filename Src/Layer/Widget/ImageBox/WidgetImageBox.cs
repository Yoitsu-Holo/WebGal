using SkiaSharp;

namespace WebGal.Layer.Widget;

public class WidgetImageBox : LayerBase
{
	private SKBitmap _imageBuffer = new();
	private SKBitmap? _renderBuffer;

	public override void SetImage(SKBitmap image, int imageId = 0) => _imageBuffer = image;

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
}