using SkiaSharp;
using WebGal.Types;

namespace WebGal.Libs.Base;

public class Layer
{
	#region  Position
	public IVector Pos { get; set; }
	public IVector PosAt(long timeoff) { var (OffX, OffY) = Anim.GetOffset(timeoff); return new IVector(Pos.X + OffX, Pos.Y + OffY); }
	public IVector Center => new(Pos.X + WinSize.Width / 2, Pos.Y + WinSize.Height / 2);
	public IVector AbsolutePos(IVector offset) => new(Pos.X + offset.X, Pos.Y + offset.Y);
	public SKSizeI WinSize { get; set; }
	#endregion

	#region attribute
	public bool StatusHasChanged;
	public LayerAtrribute OriginalAttribute;
	public LayerAtrribute DynamicAttribute;
	#endregion

	#region Next Frame
	public SKBitmap? FrameBuffer { get; private set; }
	public IVector FramePosition { get; private set; } = new(0, 0);
	public void GenNextFrame(long timeoff, bool force = false)
	{
		if (BackGroundSKBitmap is null || DynamicAttribute.IsHide)
			return;

		force = false;

		if (force || FrameBuffer is null)
		{
			FramePosition = PosAt(timeoff);
			FrameBuffer = BackGroundSKBitmap.Resize(WinSize, SKFilterQuality.High);
		}
	}
	#endregion


	#region Text
	public LayerText? Text { get; set; }
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

