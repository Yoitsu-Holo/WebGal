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

		SKMatrix matrix = SKMatrix.Identity;
		FVector pos = (FVector)Position + _animationData.PosOff;

		matrix = SKMatrix.Concat(matrix, SKMatrix.CreateScale(canvas.TotalMatrix.ScaleX, canvas.TotalMatrix.ScaleY));
		matrix = SKMatrix.Concat(matrix, SKMatrix.CreateTranslation((float)pos.X, (float)pos.Y));
		matrix = SKMatrix.Concat(matrix, _animationData.Transform); // 应用变化
		matrix = SKMatrix.Concat(matrix, SKMatrix.CreateTranslation(_offset.X, _offset.Y));

		canvas.Save();
		canvas.SetMatrix(matrix);
		canvas.DrawBitmap(_renderBuffer, new SKPoint(0, 0), _animationData.Paint);
		canvas.Restore();
	}
}