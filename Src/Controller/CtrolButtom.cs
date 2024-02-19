using SkiaSharp;
using WebGal.Event;
using WebGal.Global;
using WebGal.Types;

namespace WebGal.Controller;

class ControllerButtom : ControllerBase
{
	protected new List<SKBitmap> _image = [];


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

		_image.Add(new(Size.X, Size.Y, LayerConfig.DefaultColorType, LayerConfig.DefaultAlphaType));
		_image.Add(new(Size.X, Size.Y, LayerConfig.DefaultColorType, LayerConfig.DefaultAlphaType));
		_image.Add(new(Size.X, Size.Y, LayerConfig.DefaultColorType, LayerConfig.DefaultAlphaType));

		InitImage();
	}

	public void InitPosition(IRect position)
	{
		Position = new(position.X, position.Y);
		Size = new(position.W, position.H);
	}

	public void InitImage()
	{
		using SKCanvas buttomBackCanvas = new(_image[0]);
		using SKCanvas buttomActiveCanvas = new(_image[1]);
		using SKCanvas buttomTrigerCanvas = new(_image[2]);

		buttomBackCanvas.DrawRect(
			new SKRect(0, 0, _size.X, _size.Y),
			new SKPaint { Color = new SKColor(97, 154, 195, 255) }
		);

		buttomActiveCanvas.DrawRect(
			new SKRect(0, 0, _size.X, _size.Y),
			new SKPaint { Color = new SKColor(137, 182, 162, 255) }
		);

		buttomTrigerCanvas.DrawRect(
			new SKRect(0, 0, _size.X, _size.Y),
			new SKPaint { Color = new SKColor(176, 206, 192, 255) }
		);

		buttomBackCanvas.Flush();
		buttomActiveCanvas.Flush();
		buttomTrigerCanvas.Flush();
	}

	public void InitImage(SKBitmap back, SKBitmap active, SKBitmap triger) => (_image[0], _image[1], _image[2]) = (back, active, triger);


	public ControllerButtom() { InitPosition(new IRect(50, 20, 0, 0)); InitBase(); }
	public ControllerButtom(IRect position) { InitPosition(position); InitBase(); }
	public ControllerButtom(IVector position, IVector size) { InitPosition(new IRect(position, size)); InitBase(); }


	public override IVector GetPositon() => _position;
	public override IRect GetWindow() => new(_position, _size);


	public override void ProcessMouseEvent(MouseEvent mouseEvent)
	{
		if (RangeComp.OutRange(GetWindow(), mouseEvent.Position))
			_renderBitmap = _image[0];
		else if (mouseEvent.Button == MouseButton.Empty && mouseEvent.Status == MouseStatus.Release)
			_renderBitmap = _image[1];
		else if (mouseEvent.Button == MouseButton.LButton && mouseEvent.Status == MouseStatus.Hold)
			_renderBitmap = _image[2];
	}


	public sealed override void ProcessKeyboardEvent(KeyboardEvent keyboardEvent) => throw new InvalidOperationException("该类不包含此方法");
}