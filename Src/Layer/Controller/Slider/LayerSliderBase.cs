using SkiaSharp;
using WebGal.Handler.Event;
using WebGal.Global;
using WebGal.Types;

namespace WebGal.Layer.Controller;

public abstract class ControllerSliderBase : LayerBase
{
	protected SortedDictionary<int, SKBitmap> _image = [];


	protected SKBitmap _trackImage = new();
	protected IVector _mouseDelta = new(0, 0);
	protected new float _value = 0;
	protected IVector _thumbSize = new();
	protected IVector _thumbDelta;


	// 滑轨元素：滑块
	public bool ThumbVisiable { get; set; } = true;

	// 通用属性
	// 大小,位置 设置
	public IVector ThumbPosition { get { return Position + _thumbDelta; } set { _thumbDelta = value - Position; } }
	public IVector ThumbSize { get { return _thumbSize; } set { _thumbSize = value; } }
	public IRect ThumbWindow { get { return new(ThumbPosition, _thumbSize); } }


	public void InitBase()
	{
		Visible = true;
		Enable = true;

		Name = "slider";
		Text = "new slider";
	}

	public void InitAttribute(IRect position, IVector thumbSize)
	{
		Position = new(position.X, position.Y);
		Size = new(position.W, position.H);

		_thumbSize = thumbSize;

		_attributeChange = true;
	}

	public void InitImage()
	{
		_trackImage = new(Size.Width, Size.Height, LayerConfig.DefaultColorType, LayerConfig.DefaultAlphaType); // 导轨

		_image.Clear();
		_image[0] = new(_thumbSize.Width, _thumbSize.Height, LayerConfig.DefaultColorType, LayerConfig.DefaultAlphaType); // 滑块未触发
		_image[1] = new(_thumbSize.Width, _thumbSize.Height, LayerConfig.DefaultColorType, LayerConfig.DefaultAlphaType); // 滑块鼠标悬浮
		_image[2] = new(_thumbSize.Width, _thumbSize.Height, LayerConfig.DefaultColorType, LayerConfig.DefaultAlphaType); // 滑块鼠标点击
		_image[3] = new(_thumbSize.Width, _thumbSize.Height, LayerConfig.DefaultColorType, LayerConfig.DefaultAlphaType); // 滑块鼠标点击

		using SKCanvas trackCanvas = new(_trackImage);
		using SKCanvas thumbNormalCanvas = new(_image[(int)LayerStatus.Normal]);
		using SKCanvas thumbHoverCanvas = new(_image[(int)LayerStatus.Hover]);
		using SKCanvas thumbPressedCanvas = new(_image[(int)LayerStatus.Pressed]);
		using SKCanvas thumbFocusedCanvas = new(_image[(int)LayerStatus.Focused]);

		trackCanvas.DrawRect(
			new(0, 0, Size.Width, Size.Height),
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

		_image[(int)LayerStatus.Normal] = sliderNormal;
		_image[(int)LayerStatus.Hover] = sliderHover;
		_image[(int)LayerStatus.Pressed] = sliderPressed;
		_image[(int)LayerStatus.Focused] = sliderPressed;
	}

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

	public ControllerSliderBase()
	{
		_attributeChange = true;

		InitBase();
		InitAttribute(new(new IVector(100, 300), new IVector(200, 20)), new IVector(10, 20));
		InitImage();
	}

	public override void ExecuteAction(EventArgs eventArgs)
	{
		if (eventArgs is not MouseEventData)
			return;
		if (Status == LayerStatus.Disable)
			return;

		MouseEventData mouseEvent = (MouseEventData)eventArgs;

		if (Status != LayerStatus.Focused && !(Status == LayerStatus.Pressed && mouseEvent.Status == MouseStatus.Hold))
		{
			if (RangeComp.OutRange(ThumbWindow, mouseEvent.Position))
				Status = LayerStatus.Normal;
			else if (mouseEvent.Button == MouseButton.Empty && mouseEvent.Status == MouseStatus.Release)
				Status = LayerStatus.Hover;
			else if (mouseEvent.Button == MouseButton.LButton && mouseEvent.Status == MouseStatus.Hold)
				Status = LayerStatus.Pressed;
		}

		// 触发界面内，触发
		if (Status == LayerStatus.Pressed)
		{
			if (_mouseDelta.X == 0 && _mouseDelta.Y == 0)
				_mouseDelta = mouseEvent.Position - ThumbPosition;
			ThumbLimitSet(mouseEvent.Position - Position - _mouseDelta);
		}
		else
		{
			_mouseDelta = new(0, 0);
		}
	}

	public override void Render(SKCanvas canvas, bool force)
	{
		if (Status == LayerStatus.Unvisable)
			return;

		// 优先更改图层属性
		if (_attributeChange)
			InitImage();

		// 重新渲染
		canvas.DrawBitmap(_trackImage, Position);
		switch (Status)
		{
			case LayerStatus.Normal:
				canvas.DrawBitmap(_image[(int)LayerStatus.Normal], ThumbPosition);
				break;
			case LayerStatus.Hover:
				canvas.DrawBitmap(_image[(int)LayerStatus.Hover], ThumbPosition);
				break;
			case LayerStatus.Pressed:
				canvas.DrawBitmap(_image[(int)LayerStatus.Pressed], ThumbPosition);
				break;
			case LayerStatus.Focused:
				canvas.DrawBitmap(_image[(int)LayerStatus.Focused], ThumbPosition);
				break;
		}
	}

	// range [0,1]
	public override object Value
	{
		get => base.Value;
		set
		{
			if (value is double)
			{
				base.Value = value;
				ThumbLimitSet((IVector)((FVector)(Size - ThumbSize) * (double)value));
			}
		}
	}

	protected virtual void ThumbLimitSet(IVector thumbDelta) => _thumbDelta = new(0, 0);
}