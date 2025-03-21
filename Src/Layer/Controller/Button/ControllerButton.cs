using SkiaSharp;
using WebGal.Handler.Event;
using WebGal.Global;
using WebGal.Types;
using WebGal.Extend;

namespace WebGal.Layer.Controller;

public class ControllerButton : LayerBase
{
	protected List<SKBitmap> _imageBuffer = [];
	protected List<SKBitmap> _renderBuffer = [];

	public ControllerButton()
	{
		for (int i = 1; i <= 4; i++)
		{
			_imageBuffer.Add(new());
			_renderBuffer.Add(new());
		}
	}

	public override void SetImage(SKBitmap image, int imageId = 0)
	{
		if (imageId >= _imageBuffer.Count)
			return;
		_imageBuffer[imageId] = image;
		_dirty = true;
	}

	public override void SetImage(SKBitmap image, IRect imageWindow, int imageId = 0)
	{
		if (imageId >= _imageBuffer.Count)
			return;
		if (imageWindow == default)
			SetImage(image, imageId);
		else
			_imageBuffer[imageId] = image.SubBitmap(imageWindow);
		_dirty = true;
	}

	public override void SetColor(SKColor color, int imageId = 0)
	{
		if (imageId > _imageBuffer.Count)
			return;
		SKBitmap bitmap = new(Size.X, Size.Y, RenderConfig.DefaultColorType, RenderConfig.DefaultAlphaType);
		using SKCanvas canvas = new(bitmap);
		canvas.DrawRect(
			new SKRect(0, 0, Size.X, Size.Y),
			new SKPaint { Color = color }
		);
		canvas.Flush();
		_imageBuffer[imageId] = bitmap;
	}

	public override bool DoAction(EventArgs eventArgs)
	{
		if (eventArgs is not MouseEventData) return false;
		if (Status == LayerStatus.Disable) return false;

		MouseEventData mouseEvent = (MouseEventData)eventArgs;

		LayerStatus nowStatus = new();
		if (Status != LayerStatus.Focused)
		{
			if (RangeComp.OutRange(Window, mouseEvent.Position))
				nowStatus = LayerStatus.Normal;
			else if (mouseEvent.Button == MouseButton.Empty && mouseEvent.Status == MouseStatus.Release)
				nowStatus = LayerStatus.Hover;
			else if (mouseEvent.Button == MouseButton.LButton && mouseEvent.Status == MouseStatus.Hold)
				nowStatus = LayerStatus.Pressed;
		}

		if (nowStatus != Status)
		{
			Status = nowStatus;
			TriggerEvent(mouseEvent);
			return true;
		}

		return false;
	}

	public override void Render(SKCanvas canvas, bool force)
	{
		if (Status == LayerStatus.Unvisable || _imageBuffer is null || _imageBuffer[0].IsNull)
			return;

		if (_dirty || force || _renderBuffer[(int)Status].IsNull)
		{
			for (int i = 1; i < 4; i++)
				if (_imageBuffer[i].IsNull)
					_imageBuffer[i] = _imageBuffer[i - 1];

			for (int i = 0; i < 4; i++)
				_renderBuffer[i] = _imageBuffer[i].Resize(Size, SKSamplingOptions.Default);
			_dirty = false;

		}

		SKMatrix matrix = SKMatrix.Identity;
		FVector pos = (FVector)Position + _animationData.PosOff;

		matrix = SKMatrix.Concat(matrix, SKMatrix.CreateScale(canvas.TotalMatrix.ScaleX, canvas.TotalMatrix.ScaleY));
		matrix = SKMatrix.Concat(matrix, SKMatrix.CreateTranslation(pos.X, pos.Y));
		matrix = SKMatrix.Concat(matrix, _animationData.Transform); // 应用变化
		matrix = SKMatrix.Concat(matrix, SKMatrix.CreateTranslation(_offset.X, _offset.Y));

		canvas.Save();
		canvas.SetMatrix(matrix);
		canvas.DrawBitmap(_renderBuffer[(int)Status], new SKPoint(0, 0), _animationData.Paint);
		canvas.Restore();
	}
}