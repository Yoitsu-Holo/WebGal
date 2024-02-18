using System.Runtime.InteropServices;
using SkiaSharp;
using WebGal.Event;
using WebGal.Global;
using WebGal.Types;

namespace WebGal.Controller;


/// <summary>
/// 包含最基本的控制组件方法
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
	protected List<SKBitmap> image = [];
	protected IVector _positon = new(100, 100);
	protected IVector _size = new(10, 10);
	protected string _text = "ControllerBase";
	protected bool _enable = false;
	protected bool _visible = false;
	private string _name = "ControllerBase object";
	protected IVector delta = new(0, 0); // 界面移动量，相对值
	protected bool _triger = false;

	public ControllerBase()
	{
		SKBitmap bitmap = new(10, 20, LayerConfig.DefaultColorType, LayerConfig.DefaultAlphaType);
		using SKCanvas canvas = new(bitmap);
		canvas.DrawRect(
			new SKRect(0, 0, 10, 20),
			new SKPaint
			{
				Color = new SKColor(97, 154, 195, 128)
			}
		);
		canvas.Flush();
		image.Add(bitmap);
	}

	// 处理事件
	public virtual void ProcessMouseEvent(MouseEvent mouseEvent)
	{
		// 触发界面外
		if (RangeComp.OutRange(GetWindow(), mouseEvent.Position) && !_triger)
			return;

		Console.WriteLine($"{mouseEvent.Position.X}:{mouseEvent.Position.Y} => {GetWindow().X}:{GetWindow().Y}");
		// 触发界面内，触发
		if (mouseEvent.Button == MouseButton.LButton && mouseEvent.Status == MouseStatus.Hold)
		{
			delta = mouseEvent.Position - _positon;
			_triger = true;
		}
		else
			_triger = false;
	}
	public virtual void ProcessKeyboardEvent(KeyboardEvent keyboardEvent) => throw new NotImplementedException();

	// 显示图像
	public virtual SKBitmap Draw()
	{
		return image[0];
	}


	// 设置位置属性
	public virtual void SetPostion(IVector postion) => _positon = postion;
	public virtual void SetSize(IVector size) => _size = size;
	public virtual IVector GetPositon() => _positon + delta;
	public virtual IVector GetSize() => _size;
	public virtual IRect GetWindow() => new(_positon + delta, _size);


	// 设置文本属性
	public virtual void SetText(string s) => _text = s;
	public virtual string GetText() => _text;


	public virtual void SetEnable(bool enable)
	{
		if (enable == true)
			throw new Exception("Can not enable ControllerBase object");
	}
	public bool IsEnable() => _enable;


	// 可见性属性
	public virtual void SetVisible(bool visible) => _visible = visible;
	public bool IsVisible() => _visible;


	// 名字属性
	public virtual void SetName(string controllerName) => _name = controllerName;
	public virtual string GetName() => _name;
}