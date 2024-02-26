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
/// 
/// 目前没有区分 visiable 和 disable。
/// 原则上，visiable 控制是否显示。
/// 而 diasble 会显示，但是不能操作。
/// </summary>
class ControllerBase : IController
{
	protected List<SKBitmap> _image = [];
	protected IVector _position = new(100, 100);
	protected IVector _size = new(15, 30);
	protected string _text = "";
	protected string _name = "ControllerBase object";
	protected SKTypeface _typeface = SKTypeface.Default;

	public ControllerStatus Status = ControllerStatus.Normal;
	protected bool _attributeChange = true;

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
	}

	// 处理事件
	public virtual void ProcessMouseEvent(MouseEvent mouseEvent) => throw new NotImplementedException();
	public virtual void ProcessKeyboardEvent(KeyboardEvent keyboardEvent) => throw new NotImplementedException();

	// 渲染图像
	public virtual void Render(SKCanvas canvas) => canvas.DrawBitmap(_image[0], GetPositon());

	// 设置位置属性
	public virtual void SetPostion(IVector postion) => _position = postion;
	public virtual void SetSize(IVector size) => _size = size;
	public virtual IVector GetPositon() => _position;
	public virtual IVector GetSize() => _size;
	public virtual IRect GetWindow() => new(_position, _size);


	// 设置文本属性
	public virtual void SetText(string text) => _text = text;
	public virtual string GetText() => _text;
	public virtual void SetTypeface(SKTypeface typeface) => _typeface = typeface;
	public virtual SKTypeface GetTypeface() => _typeface;

	// 是否启用
	public virtual void SetEnable(bool enable) => Status = enable ? ControllerStatus.Normal : ControllerStatus.Disable;
	public virtual bool IsEnable() => Status != ControllerStatus.Disable;

	// 是否可见
	public virtual void SetVisible(bool visible) => Status = visible ? ControllerStatus.Normal : ControllerStatus.Unvisable;
	public virtual bool IsVisible() => Status != ControllerStatus.Unvisable;

	// 名字属性
	public virtual void SetName(string controllerName) => _name = controllerName;
	public virtual string GetName() => _name;

	public virtual void SetValue(float value) { }
	public virtual float GetValue() => 0;
}