using SkiaSharp;
using WebGal.Event;
using WebGal.Global;
using WebGal.Types;

namespace WebGal.Controller;

abstract class ControllerSliderBase : ControllerBase
{
	protected SKBitmap _trackImage = new();
	protected IVector _mouseDelta = new(0, 0);
	protected float _value = 0;
	protected IVector _thumbSize = new();
	protected IVector _thumbDelta;


	// 滑轨元素：滑块
	public bool ThumbVisiable { get; set; } = true;

	// 通用属性
	// 大小,位置 设置
	public IVector Position { get { return GetPositon(); } set { SetPostion(value); } }
	public IVector Size { get { return GetSize(); } set { SetSize(value); } }
	public IRect Window { get { return GetWindow(); } }

	public IVector ThumbPosition { get { return _position + _thumbDelta; } set { _thumbDelta = value - _position; } }
	public IVector ThumbSize { get { return _thumbSize; } set { _thumbSize = value; } }
	public IRect ThumbWindow { get { return new(ThumbPosition, _thumbSize); } }



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

		Name = "slider";
		Text = "new slider";
	}

	public void InitAttribute(IRect position, IVector thumbSize)
	{
		_position = new(position.X, position.Y);
		_size = new(position.W, position.H);

		_thumbSize = thumbSize;

		_attributeChange = true;
	}

	public void InitImage()
	{
		_trackImage = new(_size.Width, _size.Height, LayerConfig.DefaultColorType, LayerConfig.DefaultAlphaType); // 导轨

		_image.Clear();
		_image.Add(new(_thumbSize.Width, _thumbSize.Height, LayerConfig.DefaultColorType, LayerConfig.DefaultAlphaType)); // 滑块未触发
		_image.Add(new(_thumbSize.Width, _thumbSize.Height, LayerConfig.DefaultColorType, LayerConfig.DefaultAlphaType)); // 滑块鼠标悬浮
		_image.Add(new(_thumbSize.Width, _thumbSize.Height, LayerConfig.DefaultColorType, LayerConfig.DefaultAlphaType)); // 滑块鼠标点击
		_image.Add(new(_thumbSize.Width, _thumbSize.Height, LayerConfig.DefaultColorType, LayerConfig.DefaultAlphaType)); // 滑块鼠标点击

		using SKCanvas trackCanvas = new(_trackImage);
		using SKCanvas thumbNormalCanvas = new(_image[(int)ControllerStatus.Normal]);
		using SKCanvas thumbHoverCanvas = new(_image[(int)ControllerStatus.Hover]);
		using SKCanvas thumbPressedCanvas = new(_image[(int)ControllerStatus.Pressed]);
		using SKCanvas thumbFocusedCanvas = new(_image[(int)ControllerStatus.Focused]);

		trackCanvas.DrawRect(
			new(0, 0, _size.Width, _size.Height),
			new SKPaint { Color = new SKColor(200, 200, 200, 255) }
		);

		thumbNormalCanvas.DrawRect(
			new SKRect(0, 0, _thumbSize.Width, _thumbSize.Height),
			new SKPaint { Color = new SKColor(89, 166, 97, 255) }
		);

		thumbHoverCanvas.DrawRect(
			new SKRect(0, 0, _thumbSize.Width, _thumbSize.Height),
			new SKPaint { Color = new SKColor(166, 206, 170, 255) }
		);

		thumbPressedCanvas.DrawRect(
			new SKRect(0, 0, _thumbSize.Width, _thumbSize.Height),
			new SKPaint { Color = new SKColor(217, 234, 219, 255) }
		);

		thumbFocusedCanvas.DrawRect(
			new SKRect(0, 0, _thumbSize.Width, _thumbSize.Height),
			new SKPaint { Color = new SKColor(217, 234, 219, 255) }
		);

		trackCanvas.Flush();
		thumbNormalCanvas.Flush();
		thumbHoverCanvas.Flush();
		thumbPressedCanvas.Flush();
		thumbFocusedCanvas.Flush();
	}

	public void InitImage(SKBitmap trackImage, SKBitmap sliderNormal, SKBitmap sliderHover, SKBitmap sliderPressed)
	{
		_trackImage = trackImage;

		_image[(int)ControllerStatus.Normal] = sliderNormal;
		_image[(int)ControllerStatus.Hover] = sliderHover;
		_image[(int)ControllerStatus.Pressed] = sliderPressed;
		_image[(int)ControllerStatus.Focused] = sliderPressed;
	}

	public ControllerSliderBase()
	{
		_attributeChange = true;

		InitBase();
		InitAttribute(new(new IVector(100, 300), new IVector(200, 20)), new IVector(10, 20));
		InitImage();
	}


	public override void ProcessMouseEvent(MouseEvent mouseEvent)
	{
		if (Status == ControllerStatus.Disable)
			return;

		if (Status != ControllerStatus.Focused && !(Status == ControllerStatus.Pressed && mouseEvent.Status == MouseStatus.Hold))
		{
			if (RangeComp.OutRange(ThumbWindow, mouseEvent.Position))
				Status = ControllerStatus.Normal;
			else if (mouseEvent.Button == MouseButton.Empty && mouseEvent.Status == MouseStatus.Release)
				Status = ControllerStatus.Hover;
			else if (mouseEvent.Button == MouseButton.LButton && mouseEvent.Status == MouseStatus.Hold)
				Status = ControllerStatus.Pressed;
		}

		// 触发界面内，触发
		if (Status == ControllerStatus.Pressed)
		{
			if (_mouseDelta.X == 0 && _mouseDelta.Y == 0)
				_mouseDelta = mouseEvent.Position - ThumbPosition;
			_thumbDelta = ThumbLimiter(mouseEvent.Position - _position - _mouseDelta);
		}
		else
		{
			_mouseDelta = new(0, 0);
		}
	}

	public override void Render(SKCanvas canvas)
	{
		if (Status == ControllerStatus.Unvisable)
			return;

		// 优先更改图层属性
		if (_attributeChange)
			InitImage();

		// 重新渲染
		canvas.DrawBitmap(_trackImage, Position);
		Console.WriteLine(Status);
		switch (Status)
		{
			case ControllerStatus.Normal:
				canvas.DrawBitmap(_image[(int)ControllerStatus.Normal], ThumbPosition);
				break;
			case ControllerStatus.Hover:
				canvas.DrawBitmap(_image[(int)ControllerStatus.Hover], ThumbPosition);
				break;
			case ControllerStatus.Pressed:
				canvas.DrawBitmap(_image[(int)ControllerStatus.Pressed], ThumbPosition);
				break;
			case ControllerStatus.Focused:
				canvas.DrawBitmap(_image[(int)ControllerStatus.Focused], ThumbPosition);
				break;
		}
	}
	protected virtual IVector ThumbLimiter(IVector thumbDelta) => new(0, 0);


	// range [0,1]
	public override void SetValue(float value)
	{
		_value = value;
		_thumbDelta = ThumbLimiter((IVector)((FVector)(Size - ThumbSize) * value));
	}

	public override float GetValue() => _value;
}