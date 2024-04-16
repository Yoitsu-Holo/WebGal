using SkiaSharp;
using WebGal.Global;
using WebGal.Types;

namespace WebGal.Layer.Widget;

public class WidgetColorBox : LayerBase
{
	// private SKBitmap? _renderBuffer;
	private SKColor _color = new();

	public override void SetColor(SKColor color, int imageId = 0)
	{
		_color = color;
		_dirty = true;
	}

	public override void Render(SKCanvas canvas, bool force)
	{
		if (Status == LayerStatus.Unvisable)
			return;
		// if (_dirty || force || _renderBuffer is null)
		// {
		// 	_renderBuffer = new(Size.X, Size.Y, RenderConfig.DefaultColorType, RenderConfig.DefaultAlphaType);
		// 	using SKCanvas tempCanvas = new(_renderBuffer);
		// 	tempCanvas.DrawRect(
		// 		new SKRect(0, 0, _renderBuffer.Width, _renderBuffer.Height),
		// 		new SKPaint { Color = _color }
		// 	);
		// 	tempCanvas.Flush();
		// 	_dirty = false;
		// }
		_dirty = false;
		canvas.DrawRect(new SKRect(Position.X, Position.Y, Position.X + Size.Width, Position.Y + Size.Height), new SKPaint() { Color = _color, IsAntialias = true, });
		// canvas.DrawBitmap(_renderBuffer, Position);
	}

	public override void ExecuteAction(EventArgs eventArgs) { }
}