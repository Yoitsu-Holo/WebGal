using SkiaSharp;
using WebGal.Handler.Event;
using WebGal.Global;
using WebGal.Types;
using WebGal.Extend;
using WebGal.Pages;

namespace WebGal.Layer.Controller;

public class ControllerButtom : LayerBase
{
	protected List<SKBitmap> _imageBuffer = [];
	protected List<SKBitmap> _renderBuffer = [];

	public ControllerButtom()
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
		_imageBuffer[imageId] = image.SubBitmap(imageWindow);
		_dirty = true;
	}

	public override void SetColor(SKColor color, int imageId = 0)
	{
		if (imageId > _imageBuffer.Count)
			return;
		SKBitmap bitmap = new(Size.X, Size.Y, LayerConfig.DefaultColorType, LayerConfig.DefaultAlphaType);
		using SKCanvas canvas = new(bitmap);
		canvas.DrawRect(
			new SKRect(0, 0, Size.X, Size.Y),
			new SKPaint { Color = color }
		);
		canvas.Flush();
		_imageBuffer[imageId] = bitmap;
	}

	public override void ExecuteAction(EventArgs eventArgs)
	{
		if (eventArgs is not MouseEventData)
			return;
		if (Status == LayerStatus.Disable)
			return;

		MouseEventData mouseEvent = (MouseEventData)eventArgs;

		if (Status != LayerStatus.Focused)
		{
			if (RangeComp.OutRange(Window, mouseEvent.Position))
				Status = LayerStatus.Normal;
			else if (mouseEvent.Button == MouseButton.Empty && mouseEvent.Status == MouseStatus.Release)
				Status = LayerStatus.Hover;
			else if (mouseEvent.Button == MouseButton.LButton && mouseEvent.Status == MouseStatus.Hold)
				Status = LayerStatus.Pressed;
		}
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
				_renderBuffer[i] = _imageBuffer[i].Resize(Size, SKFilterQuality.High);
			_dirty = false;

		}
		canvas.DrawBitmap(_renderBuffer[(int)Status], Position);
	}
}