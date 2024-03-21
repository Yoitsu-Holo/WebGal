using SkiaSharp;
using WebGal.Handler.Event;
using WebGal.Types;

namespace WebGal.Layer;

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
public class LayerBase : ILayer
{
	protected bool _dirty = true;
	public LayerStatus Status = LayerStatus.Normal;
	protected bool _attributeChange = true;

	#region 外界交互
	//! 基类不能实现任何渲染和交互功能，只能对值进行设置
	// 处理事件
	public virtual void ProcessMouseEvent(MouseTrigger mouseEvent) => throw new NotImplementedException();
	public virtual void ProcessKeyboardEvent(KeyboardEvent keyboardEvent) => throw new NotImplementedException();

	// 渲染图像
	public virtual void Render(SKCanvas canvas) => throw new NotImplementedException();
	#endregion


	#region 自身设置
	// 设置图片属性 (不作为基本属性设置)
	public virtual void SetImage(SKBitmap image, int imageId = 0) => throw new NotImplementedException();
	public virtual void SetColor(SKColor color, IVector size = new(), int imageId = 0) => throw new NotImplementedException();


	// 设置位置属性
	public virtual void SetPostion(IVector postion) => (Position, _dirty) = (postion, true);
	public virtual IVector GetPositon() => Position;
	public IVector Position;
	public virtual void SetSize(IVector size) => (Size, _dirty) = (size, true);
	public virtual IVector GetSize() => Size;
	public IVector Size;
	public virtual IRect GetWindow() => new(Position, Size);
	public IRect Window { get { return GetWindow(); } }


	// 设置文本属性
	public virtual void SetText(string text) => (Text, _dirty) = (text, true);
	public virtual string GetText() => Text;
	public string Text = "";
	public void SerTextSize(int size) => (TextSize, _dirty) = (size, true);
	public int GetTextSize() => TextSize;
	public int TextSize;
	public virtual void SetTypeface(SKTypeface typeface) => (Typeface, _dirty) = (typeface, true);
	public virtual SKTypeface GetTypeface() => Typeface;
	public SKTypeface Typeface = SKTypeface.Default;


	// 是否启用
	public virtual void SetEnable(bool enable) => (Status, _dirty) = (enable ? LayerStatus.Normal : LayerStatus.Disable, true);
	public virtual bool IsEnable() => Status != LayerStatus.Disable;
	public bool Enable { get { return IsEnable(); } set { SetEnable(value); } }


	// 是否可见
	public virtual void SetVisible(bool visible) => (Status, _dirty) = (visible ? LayerStatus.Normal : LayerStatus.Unvisable, true);
	public virtual bool IsVisible() => Status != LayerStatus.Unvisable;
	public bool Visible { get { return IsVisible(); } set { SetVisible(value); } }


	// 名字属性
	public virtual void SetName(string controllerName) => (Name, _dirty) = (controllerName, true);
	public virtual string GetName() => Name;
	public string Name = "";


	// 值属性
	public virtual void SetValue(int value) => (Value, _dirty) = (value, true);
	public virtual int GetValue() => Value;
	public int Value;
	#endregion
}