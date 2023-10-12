using SkiaSharp;

namespace WebGal.Libs.Base;

public class Layer
{
	#region  Position
	public SKPoint Pos { get; set; }
	public SKPoint PosAt(long timeoff) { var (OffX, OffY) = Anim.GetOffset(timeoff); return new SKPoint(Pos.X + OffX, Pos.Y + OffY); }
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
	public void GenNextFrame(long timeoff, bool force = false)
	{
		if (BackGroundSKBitmap is null)
			return;
		if (force || FrameBuffer is null)
		{
			FramePosition = PosAt(timeoff);
			FrameBuffer = BackGroundSKBitmap.Resize(WinSize, SKFilterQuality.High);
		}
	}
	#endregion


	#region Text
	public List<LayerText> Text { get; private set; } = new();
	#endregion


	#region Image
	public SKBitmap? BackGroundSKBitmap { get; set; }
	#endregion


	#region Animation
	public long BeginTime { get => Anim.BeginTime; set => Anim.BeginTime = value; }
	public Animation Anim = new();
	public bool HasAnimation(long timeoff) => Anim is not null && Anim.HasAnimation(timeoff);
	#endregion


	#region Style
	public double Transparency { set; get; }
	#endregion
}