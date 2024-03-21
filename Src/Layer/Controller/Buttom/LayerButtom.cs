using SkiaSharp;
using WebGal.Handler.Event;
using WebGal.Global;
using WebGal.Types;

namespace WebGal.Layer.Controller;

public class ControllerButtom : LayerBase
{
	protected SortedDictionary<int, SKBitmap> _image = [];

	public void InitBase()
	{
		Visible = true;
		Enable = true;

		Name = "Controller Buttom";
	}

	public void InitPosition(IRect position)
	{
		Position = new(position.X, position.Y);
		Size = new(position.W, position.H);

		InitImage();
	}

	public void InitImage()
	{
		_image.Clear();

		_image[0] = new(Size.X, Size.Y, LayerConfig.DefaultColorType, LayerConfig.DefaultAlphaType);
		_image[1] = new(Size.X, Size.Y, LayerConfig.DefaultColorType, LayerConfig.DefaultAlphaType);
		_image[2] = new(Size.X, Size.Y, LayerConfig.DefaultColorType, LayerConfig.DefaultAlphaType);
		_image[3] = new(Size.X, Size.Y, LayerConfig.DefaultColorType, LayerConfig.DefaultAlphaType);

		using SKCanvas buttomNormalCanvas = new(_image[0]);
		using SKCanvas buttomHoverCanvas = new(_image[1]);
		using SKCanvas buttomPressedCanvas = new(_image[2]);
		using SKCanvas buttomFocusedCanvas = new(_image[2]);

		buttomNormalCanvas.DrawRect(
			new SKRect(0, 0, Size.X, Size.Y),
			new SKPaint { Color = new SKColor(100, 159, 133, 255) }
		);

		buttomHoverCanvas.DrawRect(
			new SKRect(0, 0, Size.X, Size.Y),
			new SKPaint { Color = new SKColor(137, 182, 162, 255) }
		);

		buttomPressedCanvas.DrawRect(
			new SKRect(0, 0, Size.X, Size.Y),
			new SKPaint { Color = new SKColor(176, 204, 190, 255) }
		);

		buttomFocusedCanvas.DrawRect(
			new SKRect(0, 0, Size.X, Size.Y),
			new SKPaint { Color = new SKColor(176, 204, 190, 255) }
		);

		buttomNormalCanvas.Flush();
		buttomHoverCanvas.Flush();
		buttomPressedCanvas.Flush();
		buttomFocusedCanvas.Flush();
	}

	public void InitImage(SKBitmap normal, SKBitmap hover, SKBitmap pressed, SKBitmap focused) => (_image[0], _image[1], _image[2], _image[4]) = (normal, hover, pressed, focused);

	public override void SetImage(SKBitmap image, int imageId = 0) => _image[imageId] = image;
	public override void SetColor(SKColor color, IVector size = new(), int imageId = 0)
	{
		if (size.X * size.Y == 0)
			size = Size;
		SKBitmap bitmap = new(size.X, size.Y, LayerConfig.DefaultColorType, LayerConfig.DefaultAlphaType);
		using SKCanvas canvas = new(bitmap);
		canvas.DrawRect(
			new SKRect(0, 0, size.X, size.Y),
			new SKPaint { Color = color }
		);
		canvas.Flush();
		_image[imageId] = bitmap;
	}

	public ControllerButtom() { InitBase(); InitPosition(new IRect(50, 20, 0, 0)); }
	public ControllerButtom(IRect position) { InitBase(); InitPosition(position); }
	public ControllerButtom(IVector position, IVector size) { InitBase(); InitPosition(new IRect(position, size)); }

	public override void ProcessMouseEvent(MouseTrigger mouseEvent)
	{
		if (Status == LayerStatus.Disable)
			return;
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

	public override void Render(SKCanvas canvas)
	{
		if (Status == LayerStatus.Unvisable)
			return;
		canvas.DrawBitmap(_image[(int)Status], Position);
	}
}