using SkiaSharp;

namespace WebGal.Services.Module;

/// <summary>
/// + Positon
/// 	+ 左上角坐标 & 长宽
/// + Fade
/// 	+ 整个场景图层的淡入淡出方式
/// + Text(front)
/// 	+ 文字颜色
/// 	+ 文字样式
/// 	+ 文字文本
/// 	+ 文字坐标（默认0，0）
/// + SKBitmap(back)
/// 	+ 场景图片
/// + Animation
/// 	+ 场景动画效果 （todo）
/// + Style
/// 	+ 图层透明度
/// </summary>
public class Layer
{
	#region  Position
	public SKPoint Pos { get; set; }
	public SKPoint Center => new(Pos.X + WinSize.Width / 2, Pos.Y + WinSize.Height / 2);
	public SKPoint AbsolutePos(SKPoint offset) => new(Pos.X + offset.X, Pos.Y + offset.Y);
	public float Left => Pos.X;
	public float Right => Pos.X + WinSize.Width;
	public float Top => Pos.Y;
	public float Bottom => Pos.Y + WinSize.Height;
	#endregion

	#region Window Size
	public SKSizeI WinSize { get; set; }
	public SKRect Window => new(Pos.X, Pos.Y, WinSize.Width, WinSize.Height);
	#endregion

	#region Next Frame
	public SKBitmap? FrameBuffer { get; private set; }
	public SKPoint FramePosition { get; private set; } = new(0, 0);
	public void NextFrame(int timeoff, bool force = false)
	{
		if (BackGroundSKBitmap is null)
			return;
		if (force || FrameBuffer is null)
		{
			Console.WriteLine("Rerendering");
			FramePosition = Pos;
			FrameBuffer = BackGroundSKBitmap.Resize(WinSize, SKFilterQuality.High);
		}
	}
	#endregion


	#region Fade
	private (SKBitmap In, SKBitmap Out) _fadeMask;
	private (float In, float Out) _fadeTime; //ms

	public void SetFadeIn(SKBitmap mask, float time)
	{
		(_fadeMask.In, _fadeTime.In) = (mask, time);
	}
	public void SetFadeOut(SKBitmap mask, float time)
	{
		(_fadeMask.Out, _fadeTime.Out) = (mask, time);
	}
	#endregion


	#region Text
	public List<LayerText> Text { get; private set; } = new();
	#endregion


	#region Image
	public SKBitmap? BackGroundSKBitmap { get; set; }
	#endregion


	#region Animation
	public Animation? Anim;
	#endregion


	#region Style
	public double Transparency { set; get; }
	#endregion
}