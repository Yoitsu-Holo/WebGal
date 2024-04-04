using System.Text.Json.Serialization;
using SkiaSharp;

namespace WebGal.Types;

public record class IRect
{
	public IRect()
	{
		(X, Y) = (0, 0);
		(W, H) = (0, 0);
	}

	public IRect(int x, int y, int w, int h)
	{
		(X, Y) = (x, y);
		(W, H) = (w, h);
	}

	public IRect(IVector pos, IVector size)
	{
		(X, Y) = pos;
		(W, H) = size;
	}

	public static implicit operator SKRectI(IRect rect) => new(rect.Left, rect.Top, rect.Right, rect.Bottom);
	public static implicit operator SKRect(IRect rect) => new(rect.Left, rect.Top, rect.Right, rect.Bottom);
	public static explicit operator (IVector, IVector)(IRect rect) => (new(rect.X, rect.Y), new(rect.H, rect.W));
	public static explicit operator (FVector, FVector)(IRect rect) => (new(rect.X, rect.Y), new(rect.H, rect.W));

	public static explicit operator FRect(IRect rect) => new(rect.X, rect.Y, rect.W, rect.H);


	public int X { get; set; } = 0;
	public int Y { get; set; } = 0;
	public int W { get; set; } = 0;
	public int H { get; set; } = 0;

	[JsonIgnore] public int Left => X;
	[JsonIgnore] public int Right => X + W;
	[JsonIgnore] public int Top => Y;
	[JsonIgnore] public int Bottom => Y + H;

	[JsonIgnore] public IVector MidPoint => new(X + W / 2, Y + H / 2);
}


public record class FRect
{
	public FRect(int x, int y, int w, int h)
	{
		(X, Y) = (x, y);
		(W, H) = (w, h);
	}

	public FRect(FVector pos, FVector size)
	{
		(X, Y) = pos;
		(W, H) = size;
	}

	public static implicit operator SKRectI(FRect rect) => new((int)rect.Left, (int)rect.Top, (int)rect.Right, (int)rect.Bottom);
	public static implicit operator SKRect(FRect rect) => new((float)rect.Left, (float)rect.Top, (float)rect.Right, (float)rect.Bottom);
	public static explicit operator (IVector, IVector)(FRect rect) => (new((int)rect.X, (int)rect.Y), new((int)rect.H, (int)rect.W));
	public static explicit operator (FVector, FVector)(FRect rect) => (new(rect.X, rect.Y), new(rect.H, rect.W));

	public static explicit operator IRect(FRect rect) => new((int)rect.X, (int)rect.Y, (int)rect.W, (int)rect.H);


	public double X { get; set; } = 0;
	public double Y { get; set; } = 0;
	public double W { get; set; } = 0;
	public double H { get; set; } = 0;

	[JsonIgnore] public double Left => X;
	[JsonIgnore] public double Right => X + W;
	[JsonIgnore] public double Top => Y;
	[JsonIgnore] public double Bottom => Y + H;

	[JsonIgnore] public FVector MidPoint => new(X + W / 2, Y + H / 2);
}

public record struct IVector(int X, int Y)
{
	public static IVector operator *(IVector v1, int off) => new(v1.X * off, v1.Y * off);
	public static IVector operator /(IVector v1, int off) => new(v1.X / off, v1.Y / off);
	public static IVector operator *(IVector v1, IVector v2) => new(v1.X * v2.X, v1.Y * v2.Y);
	public static IVector operator +(IVector v1, IVector v2) => new(v1.X + v2.X, v1.Y + v2.Y);
	public static IVector operator -(IVector v1, IVector v2) => new(v1.X - v2.X, v1.Y - v2.Y);

	public static implicit operator SKPointI(IVector p) => new(p.X, p.Y);
	public static implicit operator SKPoint(IVector p) => new(p.X, p.Y);

	public static implicit operator SKSizeI(IVector p) => new(p.X, p.Y);
	public static implicit operator SKSize(IVector p) => new(p.X, p.Y);

	public static explicit operator (int, int)(IVector p) => (p.X, p.Y);
	public static explicit operator (double, double)(IVector p) => (p.X, p.Y);

	public static explicit operator FVector(IVector p) => new(p.X, p.Y);

	[JsonIgnore] public readonly int Width => X;
	[JsonIgnore] public readonly int Height => Y;
}

public record struct FVector(double X, double Y)
{
	public static FVector operator *(FVector v1, double off) => new(v1.X * off, v1.Y * off);
	public static FVector operator /(FVector v1, double off) => new(v1.X / off, v1.Y / off);
	public static FVector operator *(FVector v1, FVector v2) => new(v1.X * v2.X, v1.Y * v2.Y);
	public static FVector operator +(FVector v1, FVector v2) => new(v1.X + v2.X, v1.Y + v2.Y);
	public static FVector operator -(FVector v1, FVector v2) => new(v1.X - v2.X, v1.Y - v2.Y);

	public static implicit operator SKPointI(FVector p) => new((int)p.X, (int)p.Y);
	public static implicit operator SKPoint(FVector p) => new((float)p.X, (float)p.Y);

	public static implicit operator SKSizeI(FVector p) => new((int)p.X, (int)p.Y);
	public static implicit operator SKSize(FVector p) => new((float)p.X, (float)p.Y);

	public static explicit operator (int, int)(FVector p) => ((int)p.X, (int)p.Y);
	public static explicit operator (double, double)(FVector p) => (p.X, p.Y);

	public static explicit operator IVector(FVector p) => new((int)p.X, (int)p.Y);

	[JsonIgnore] public readonly double Width => X;
	[JsonIgnore] public readonly double Height => Y;
}

public record struct WinSizeStructure(int Width, int Height)
{
	public static implicit operator SKSizeI(WinSizeStructure winSize) => new(winSize.Width, winSize.Height);
	public static implicit operator SKSize(WinSizeStructure winSize) => new(winSize.Width, winSize.Height);
	public static implicit operator (int, int)(WinSizeStructure win) => (win.Width, win.Height);
}

public record struct UrlStructure(string Name = "", string URL = "");
public record struct ColorStructure(byte R, byte G, byte B, byte A);
public record struct PaintStructure
(
	ColorStructure Color,
	int TextSize,
	bool Blod,
	bool Antialias
);


// csv Register
public record struct IVectorRegister(int C_iX, int C_iY);
public record struct FVectorRegister(int C_fX, int C_fY);
public record struct WinSizeStructureRegister(int C_iWidth, int C_iHeight);
public record struct UrlStructureRegister(int C_sName, int C_sURL);
public record struct ColorStructureRegister(int C_cR, int C_cG, int C_cB, int C_cA);
public record struct PaintStructureRegister(
	ColorStructureRegister O_Color,
	int C_iTextSize,
	int C_bBlod,
	int C_bAntialias
);