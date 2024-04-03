using SkiaSharp;
using WebGal.Global;
using WebGal.Types;

namespace WebGal.Layer.Widget;

public class WidgetColorBox : LayerBase
{
	private SKBitmap? _renderBuffer;
	private SKColor _color = new();

	public override void SetColor(SKColor color, IVector size = default, int imageId = 0)
	{
		_color = color;
		_renderBuffer = new(size.X, size.Y, LayerConfig.DefaultColorType, LayerConfig.DefaultAlphaType);
		_dirty = true;
	}

	public override void Render(SKCanvas canvas, bool force)
	{
		if (Status == LayerStatus.Unvisable || _renderBuffer is null)
			return;
		if (_dirty || force)
		{
			_renderBuffer = new(Size.X, Size.Y, LayerConfig.DefaultColorType, LayerConfig.DefaultAlphaType);
			using SKCanvas tempCanvas = new(_renderBuffer);
			tempCanvas.DrawRect(
				new SKRect(0, 0, _renderBuffer.Width, _renderBuffer.Height),
				new SKPaint { Color = _color }
			);
			tempCanvas.Flush();
			_dirty = false;
		}
		canvas.DrawBitmap(_renderBuffer, Position);
	}

	public override void ExecuteAction(EventArgs eventArgs) { }
}