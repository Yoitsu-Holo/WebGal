using SkiaSharp;
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

		SKMatrix matrix = SKMatrix.Identity;
		FVector pos = (FVector)Position + _animationData.PosOff;

		matrix = SKMatrix.Concat(matrix, SKMatrix.CreateTranslation((float)pos.X, (float)pos.Y));
		matrix = SKMatrix.Concat(matrix, _animationData.Transform); // 应用变化
		matrix = SKMatrix.Concat(matrix, SKMatrix.CreateTranslation(-_offset.X, -_offset.Y));

		canvas.Save();
		canvas.SetMatrix(matrix);
		canvas.DrawRect(new SKRect(0, 0, Size.Width, Size.Height), new SKPaint() { Color = _color, IsAntialias = true, });
		// canvas.DrawBitmap(_renderBuffer, new SKPoint(0, 0), RenderConfig.DefaultPaint);
		canvas.Restore();

	}
}