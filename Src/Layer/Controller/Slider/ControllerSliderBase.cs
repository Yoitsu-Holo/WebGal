using SkiaSharp;
using WebGal.Handler.Event;
using WebGal.Global;
using WebGal.Types;
using WebGal.Extend;

namespace WebGal.Layer.Controller;

public abstract class ControllerSliderBase : LayerBase
{
	protected SortedDictionary<int, SKBitmap> _image = [];


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

	public void InitAttribute(IVector trackSize, IVector thumbSize)
	{
		Size = trackSize;
		_thumbSize = thumbSize;

		_attributeChange = true;
	}

	public void InitImage(SKBitmap trackImage, SKBitmap sliderNormal, SKBitmap sliderHover, SKBitmap sliderPressed)
	{
		// _trackImage = trackImage;

		_image[-1] = trackImage;
		_image[(int)LayerStatus.Normal] = sliderNormal;
		_image[(int)LayerStatus.Hover] = sliderHover;
		_image[(int)LayerStatus.Pressed] = sliderPressed;
		_image[(int)LayerStatus.Focused] = sliderPressed;
	}

	public override void SetImage(SKBitmap image, int imageId = 0)
	{
		_image[imageId] = image;
		_dirty = true;
	}

	public override void SetImage(SKBitmap image, IRect imageWindow, int imageId = 0)
	{
		if (imageWindow == default)
			SetImage(image, imageId);
		else
			_image[imageId] = image.SubBitmap(imageWindow);

		_dirty = true;
	}

	public override void SetColor(SKColor color, int imageId = 0)
	{
		SKBitmap bitmap = new(Size.X, Size.Y, RenderConfig.DefaultColorType, RenderConfig.DefaultAlphaType);
		using SKCanvas canvas = new(bitmap);
		canvas.DrawRect(
			new SKRect(0, 0, Size.X, Size.Y),
			new SKPaint { Color = color }
		);
		canvas.Flush();
		_image[imageId] = bitmap;
	}

	public ControllerSliderBase()
	{
		Position = new IVector(100, 300);

		InitBase();
		InitAttribute(new IVector(200, 20), new IVector(10, 20));

		for (int i = -1; i <= 4; i++)
			_image[i] = new();
	}

	public override bool DoAction(EventArgs eventArgs)
	{
		if (eventArgs is not MouseEventData) return false;
		if (Status == LayerStatus.Disable) return false;

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
			return true;
		}
		else
		{
			_mouseDelta = new(0, 0);
			if (RangeComp.InRange(Window, mouseEvent.Position))
				return true;
		}
		return false;
	}

	public override void Render(SKCanvas canvas, bool force)
	{
		if (Status == LayerStatus.Unvisable || _image[(int)Status].IsNull || _image[-1].IsNull)
			return;

		// 重新渲染
		SKMatrix matrix = SKMatrix.Identity;
		FVector pos = (FVector)Position + _animationData.PosOff;

		matrix = SKMatrix.Concat(matrix, SKMatrix.CreateScale(canvas.TotalMatrix.ScaleX, canvas.TotalMatrix.ScaleY));
		matrix = SKMatrix.Concat(matrix, SKMatrix.CreateTranslation((float)pos.X, (float)pos.Y));
		matrix = SKMatrix.Concat(matrix, _animationData.Transform); // 应用变化
		matrix = SKMatrix.Concat(matrix, SKMatrix.CreateTranslation(_offset.X, _offset.Y));

		canvas.Save();
		canvas.SetMatrix(matrix);
		canvas.DrawBitmap(_image[-1], new SKPoint(0, 0), RenderConfig.DefaultPaint);
		canvas.Restore();

		matrix = SKMatrix.Identity;
		pos = (FVector)ThumbPosition + _animationData.PosOff;

		matrix = SKMatrix.Concat(matrix, SKMatrix.CreateScale(canvas.TotalMatrix.ScaleX, canvas.TotalMatrix.ScaleY));
		matrix = SKMatrix.Concat(matrix, SKMatrix.CreateTranslation((float)pos.X, (float)pos.Y));
		matrix = SKMatrix.Concat(matrix, _animationData.Transform); // 应用变化
		matrix = SKMatrix.Concat(matrix, SKMatrix.CreateTranslation(_offset.X, _offset.Y));

		canvas.Save();
		canvas.SetMatrix(matrix);
		canvas.DrawBitmap(_image[(int)Status], new SKPoint(0, 0), _animationData.Paint);
		canvas.Restore();
	}

	// range [0,1]
	public override object Value
	{
		get => base.Value;
		set
		{
			if (value is float v)
			{
				base.Value = value;
				ThumbLimitSet((IVector)((FVector)(Size - ThumbSize) * v));
			}
		}
	}

	protected virtual void ThumbLimitSet(IVector thumbDelta) => _thumbDelta = new(0, 0);
}