using System.ComponentModel.DataAnnotations;
using KristofferStrube.Blazor.WebAudio;
using SkiaSharp;
using WebGal.Event;
using WebGal.Global;
using WebGal.Types;

namespace WebGal.Controller;

class ControllerSliderBox : ControllerBase
{
	protected SKBitmap _trackImage = new();

	// 滑轨元素：滑块
	private IVector _thumbSize = new();
	public bool ThumbVisiable { get; set; } = true;

	// 通用属性
	// 大小,位置 设置
	public IVector Position { get { return GetPositon(); } set { SetPostion(value); } }
	public IVector Size { get { return GetSize(); } set { SetSize(value); } }
	public IRect Window { get { return GetWindow(); } }

	public IVector ThumbPosition { get { return _position + ThumbDelta; } set { ThumbDelta = value - _position; } }
	public IVector ThumbDelta;
	public IVector ThumbSize { get { return _thumbSize; } set { _thumbSize = value; } }
	public IRect ThumbWindow { get { return new(ThumbPosition, _thumbSize); } }

	public int Value { get; set; } = 0;


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
		_statusChange = true;
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
			new SKPaint { Color = new SKColor(164, 156, 91, 255) }
		);

		thumbHoverCanvas.DrawRect(
			new SKRect(0, 0, _thumbSize.Width, _thumbSize.Height),
			new SKPaint { Color = new SKColor(176, 170, 117, 255) }
		);

		thumbPressedCanvas.DrawRect(
			new SKRect(0, 0, _thumbSize.Width, _thumbSize.Height),
			new SKPaint { Color = new SKColor(198, 193, 153, 255) }
		);

		thumbFocusedCanvas.DrawRect(
			new SKRect(0, 0, _thumbSize.Width, _thumbSize.Height),
			new SKPaint { Color = new SKColor(198, 193, 153, 255) }
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

	public ControllerSliderBox()
	{
		_statusChange = true;
		_attributeChange = true;

		InitBase();
		InitAttribute(new(new IVector(100, 300), new IVector(200, 20)), new IVector(10, 20));
		InitImage();
	}


	public override void ProcessMouseEvent(MouseEvent mouseEvent)
	{
		// 触发界面外
		if (RangeComp.OutRange(ThumbWindow, mouseEvent.Position) && !_statusChange)
		{
			_mouseDelta = new(0, 0);
			return;
		}

		// 触发界面内，触发
		if (mouseEvent.Button == MouseButton.LButton && mouseEvent.Status == MouseStatus.Hold)
		{
			if (_mouseDelta.X == 0 && _mouseDelta.Y == 0)
				_mouseDelta = mouseEvent.Position - GetPositon();
			// _formDelta = mouseEvent.Position - _positon;
			ThumbDelta = mouseEvent.Position - _position - _mouseDelta;
			ThumbDelta.Y = 0;
			ThumbDelta.X = Math.Max(0, Math.Min(_size.Width - _thumbSize.Width, ThumbDelta.X));

			_statusChange = true;
		}
		else
		{
			_mouseDelta = new(0, 0);
			_statusChange = false;
		}
	}

	public sealed override void ProcessKeyboardEvent(KeyboardEvent keyboardEvent) => throw new InvalidOperationException("该类不包含此方法");

	public override void Render(SKCanvas canvas)
	{
		if (Status == ControllerStatus.Disable)
			return;

		// 优先更改图层属性
		if (_attributeChange)
			InitImage();

		// 重新渲染
		canvas.DrawBitmap(_trackImage, Position);
		Console.WriteLine($"{_size.Width} {_size.Y}");
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
}