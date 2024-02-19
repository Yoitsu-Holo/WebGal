using System.Reflection;
using SkiaSharp;
using WebGal.Event;
using WebGal.Global;
using WebGal.Types;

namespace WebGal.Controller;

/// <summary>
/// 包含最基本的控制组件方法.
/// 其行为是一个可以被拖动的简单方块
/// 
/// 1. 多图层（惰性叠加）
/// 2. 基础属性设置
/// 	- 文本
/// 	- 位置
/// 	- 大小
/// 	- 可见性
/// 3. 控制
/// 	- 跟踪事件
/// 	- 传入事件自动处理
/// </summary>
class ControllerBase : IController
{
	protected SKBitmap _renderBitmap = new();
	protected List<SKBitmap> _image = [];
	protected IVector _position = new(100, 100);
	protected IVector _size = new(15, 30);
	protected string _text = "ControllerBase";
	protected string _name = "ControllerBase object";
	protected SKTypeface _typeface = SKTypeface.Default;
	protected bool _enable = false;
	protected bool _visible = false;
	protected IVector _mouseDelta = new(0, 0); // 界面移动量，相对值
	protected IVector _formDelta = new(0, 0); // 界面移动量，相对值
	protected bool _triger = false;

	public ControllerBase()
	{
		SKBitmap bitmap = new(_size.X, _size.Y, LayerConfig.DefaultColorType, LayerConfig.DefaultAlphaType);
		using SKCanvas canvas = new(bitmap);
		canvas.DrawRect(
			new SKRect(0, 0, _size.X, _size.Y),
			new SKPaint { Color = new SKColor(97, 154, 195, 128) }
		);
		canvas.Flush();
		_image.Add(bitmap);
		_renderBitmap = _image[0];
	}

	// 处理事件
	public virtual void ProcessMouseEvent(MouseEvent mouseEvent)
	{
		// 触发界面外
		if (RangeComp.OutRange(GetWindow(), mouseEvent.Position) && !_triger)
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
			_formDelta = mouseEvent.Position - _position - _mouseDelta;
			_triger = true;
		}
		else
		{
			_mouseDelta = new(0, 0);
			_triger = false;
		}
	}
	public virtual void ProcessKeyboardEvent(KeyboardEvent keyboardEvent) => throw new NotImplementedException();

	// 显示图像
	public virtual SKBitmap Draw() => _renderBitmap;

	// 设置位置属性
	public virtual void SetPostion(IVector postion) => _position = postion;
	public virtual void SetSize(IVector size) => _size = size;
	public virtual IVector GetPositon() => _position + _formDelta;
	public virtual IVector GetSize() => _size;
	public virtual IRect GetWindow() => new(_position + _formDelta, _size);


	// 设置文本属性
	public virtual void SetText(string s) => _text = s;
	public virtual string GetText() => _text;

	public void SetTypeface(SKTypeface typeface) => _typeface = typeface;
	public SKTypeface GetTypeface() => _typeface;


	public virtual void SetEnable(bool enable) => _enable = enable;
	public bool IsEnable() => _enable;


	// 可见性属性
	public virtual void SetVisible(bool visible) => _visible = visible;
	public bool IsVisible() => _visible;


	// 名字属性
	public virtual void SetName(string controllerName) => _name = controllerName;
	public virtual string GetName() => _name;
}