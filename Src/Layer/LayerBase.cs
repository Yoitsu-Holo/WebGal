using SkiaSharp;
using WebGal.Animations;
using WebGal.Handler;
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
public class LayerBase : ILayer, IAction
{
	protected bool _dirty = true;
	public LayerStatus Status = LayerStatus.Normal;
	protected bool _attributeChange = true;

	#region 外界交互
	//! 基类不能实现任何渲染和交互功能，只能对值进行设置
	// 处理事件
	public virtual void ExecuteAction(EventArgs eventArgs) => throw new NotImplementedException();

	// 渲染图像
	public virtual void Render(SKCanvas canvas) => throw new NotImplementedException();
	#endregion


	#region 自身设置
	// 设置图片属性 (不作为基本属性设置)
	public virtual void SetImage(SKBitmap image, int imageId = 0) => throw new NotImplementedException();
	public virtual void SetColor(SKColor color, IVector size = new(), int imageId = 0) => throw new NotImplementedException();

	public IAnimation Animation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	public void DoAnimation(long timeOff) { }



	// 设置位置属性
	public virtual IVector Position { get => Position; set => (Position, _dirty) = (value, true); }
	public virtual IVector Size { get => Size; set => (Size, _dirty) = (value, true); }
	public virtual IRect Window { get { return new(Position, Size); } }


	// 设置文本属性
	public virtual string Text { get => Text; set => (Text, _dirty) = (value, true); }
	public virtual int TextSize { get => TextSize; set => (TextSize, _dirty) = (value, true); }
	public virtual SKTypeface Typeface { get => Typeface; set => (Typeface, _dirty) = (value, true); }


	public virtual bool Enable { get => Status != LayerStatus.Disable; set => (Status, _dirty) = (value ? LayerStatus.Normal : LayerStatus.Disable, true); }
	public virtual bool Visible { get => Status != LayerStatus.Unvisable; set => (Status, _dirty) = (value ? LayerStatus.Normal : LayerStatus.Unvisable, true); }
	public virtual string Name { get => Name; set => (Name, _dirty) = (value, true); }
	public virtual object Value { get => Value; set => (Value, _dirty) = (value, true); }
	#endregion
}