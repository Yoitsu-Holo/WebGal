using SkiaSharp;
using WebGal.Event;
using WebGal.Global;
using WebGal.Types;

namespace WebGal.Controller;

class ControllerButtom : ControllerBase
{
	// 大小,位置 设置
	public IVector Size { get { return GetSize(); } set { SetSize(value); } }
	public IVector Position { get { return GetPositon(); } set { SetPostion(value); } }
	public IRect Window { get { return GetWindow(); } }


	// 文本设置
	public string Name { get { return GetName(); } set { SetName(value); } }
	public string Text { get { return GetText(); } set { SetText(value); } }

	public SKTypeface Typeface { get { return GetTypeface(); } set { SetTypeface(value); } }


	// 设置可见性、功能性
	public bool Visible { get { return IsVisible(); } set { SetVisible(value); } }
	public bool Enable { get { return IsEnable(); } set { SetEnable(value); } }


	public void InitBase()
	{
		Visible = true;
		Enable = true;

		Name = "buttom";
		Text = "new buttom";
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

		_image.Add(new(_size.X, _size.Y, LayerConfig.DefaultColorType, LayerConfig.DefaultAlphaType));
		_image.Add(new(_size.X, _size.Y, LayerConfig.DefaultColorType, LayerConfig.DefaultAlphaType));
		_image.Add(new(_size.X, _size.Y, LayerConfig.DefaultColorType, LayerConfig.DefaultAlphaType));
		_image.Add(new(_size.X, _size.Y, LayerConfig.DefaultColorType, LayerConfig.DefaultAlphaType));

		using SKCanvas buttomNormalCanvas = new(_image[0]);
		using SKCanvas buttomHoverCanvas = new(_image[1]);
		using SKCanvas buttomPressedCanvas = new(_image[2]);
		using SKCanvas buttomFocusedCanvas = new(_image[2]);

		buttomNormalCanvas.DrawRect(
			new SKRect(0, 0, _size.X, _size.Y),
			new SKPaint { Color = new SKColor(97, 154, 195, 255) }
		);

		buttomHoverCanvas.DrawRect(
			new SKRect(0, 0, _size.X, _size.Y),
			new SKPaint { Color = new SKColor(137, 182, 162, 255) }
		);

		buttomPressedCanvas.DrawRect(
			new SKRect(0, 0, _size.X, _size.Y),
			new SKPaint { Color = new SKColor(176, 206, 192, 255) }
		);

		buttomFocusedCanvas.DrawRect(
			new SKRect(0, 0, _size.X, _size.Y),
			new SKPaint { Color = new SKColor(176, 206, 192, 255) }
		);

		buttomNormalCanvas.Flush();
		buttomHoverCanvas.Flush();
		buttomPressedCanvas.Flush();
		buttomFocusedCanvas.Flush();
	}

	public void InitImage(SKBitmap normal, SKBitmap hover, SKBitmap pressed, SKBitmap focused) => (_image[0], _image[1], _image[2], _image[4]) = (normal, hover, pressed, focused);


	public ControllerButtom() { InitBase(); InitPosition(new IRect(50, 20, 0, 0)); }
	public ControllerButtom(IRect position) { InitBase(); InitPosition(position); }
	public ControllerButtom(IVector position, IVector size) { InitBase(); InitPosition(new IRect(position, size)); }


	public override IVector GetPositon() => _position;
	public override IRect GetWindow() => new(_position, _size);


	public override void ProcessMouseEvent(MouseEvent mouseEvent)
	{
		if (Status == ControllerStatus.Disable || Status == ControllerStatus.Focused)
			return;

		if (RangeComp.OutRange(GetWindow(), mouseEvent.Position))
			Status = ControllerStatus.Normal;
		// _renderBitmap = _image[0];
		else if (mouseEvent.Button == MouseButton.Empty && mouseEvent.Status == MouseStatus.Release)
			Status = ControllerStatus.Hover;
		// _renderBitmap = _image[1];
		else if (mouseEvent.Button == MouseButton.LButton && mouseEvent.Status == MouseStatus.Hold)
			Status = ControllerStatus.Pressed;
		// _renderBitmap = _image[2];
	}

	public override void Render(SKCanvas canvas)
	{
		if (Status == ControllerStatus.Disable)
			return;
		canvas.DrawBitmap(_image[(int)Status], Position);
	}


	public sealed override void ProcessKeyboardEvent(KeyboardEvent keyboardEvent) => throw new InvalidOperationException("该类不包含此方法");
}