using SkiaSharp;
using WebGal.Global;
using WebGal.Types;

namespace WebGal.Layer.Widget;

public class WidgetColorBox : LayerBase
{
	private SKBitmap _imageBuffer = new();
	private SKColor _color = new();

	public override void SetColor(SKColor color, IVector size = default, int imageId = 0)
	{
		_color = color;
		_imageBuffer = new(size.X, size.Y, LayerConfig.DefaultColorType, LayerConfig.DefaultAlphaType);
		_dirty = true;
	}

	public override void Render(SKCanvas canvas, bool force)
	{
		if (Status == LayerStatus.Unvisable || _imageBuffer.IsNull)
			return;
		if (_dirty)
		{
			using SKCanvas tempCanvas = new(_imageBuffer);
			tempCanvas.DrawRect(
				new SKRect(0, 0, _imageBuffer.Width, _imageBuffer.Height),
				new SKPaint { Color = _color }
			);
			tempCanvas.Flush();
		}
		canvas.DrawBitmap(_imageBuffer, Position);
	}

	public override void ExecuteAction(EventArgs eventArgs) { }
}