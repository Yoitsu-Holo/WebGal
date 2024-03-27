using SkiaSharp;

namespace WebGal.Layer.Widget;

public class WidgetImageBox : LayerBase
{
	private SKBitmap _imageBuffer = new();

	public override void SetImage(SKBitmap image, int imageId = 0) => _imageBuffer = image;

	public override void Render(SKCanvas canvas)
	{
		if (Status == LayerStatus.Unvisable)
			return;
		canvas.DrawBitmap(_imageBuffer, Position);
	}
}