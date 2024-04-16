using SkiaSharp;
using WebGal.Animations;
using WebGal.Handler;
using WebGal.Types;

namespace WebGal.Layer;

public interface ILayer : IAction
{
	// 渲染到指定canvas
	public void Render(SKCanvas canvas, bool force);

	// 图片处理
	public void SetImage(SKBitmap image, int imageId);
	public void SetImage(SKBitmap image, IRect imageWindow, int imageId);
	public void SetColor(SKColor color, int imageId);

	// 动画处理
	public IAnimation Animation { get; set; }
	public bool HasAnimation(long timeOff);
	public void DoAnimation(long timeOff);


	// 大小处理、位置处理（左上角）
	public IVector Size { get; set; }
	public IVector Position { get; set; }
	public IRect Window { get; }

	// 文本处理
	public string Text { get; set; }
	public int TextSize { get; set; }
	public SKTypeface Typeface { get; set; }

	public string Name { get; set; }    // 设置名字
	public bool Visible { get; set; }   // 可见性
	public bool Enable { get; set; }    // 功能性
	public object Value { get; set; }   // 值处理
}