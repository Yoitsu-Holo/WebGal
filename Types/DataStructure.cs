using SkiaSharp;

namespace WebGal.Types;

public record struct IVector(int X, int Y)
{
	public static IVector operator *(IVector v1, int off) => new(v1.X * off, v1.Y * off);
	public static IVector operator *(IVector v1, IVector v2) => new(v1.X * v2.X, v1.Y * v2.Y);
	public static IVector operator +(IVector v1, IVector v2) => new(v1.X + v2.X, v1.Y + v2.Y);
	public static IVector operator -(IVector v1, IVector v2) => new(v1.X - v2.X, v1.Y - v2.Y);

	public static implicit operator SKPointI(IVector p) => new(p.X, p.Y);
	public static implicit operator SKPoint(IVector p) => new(p.X, p.Y);
	public static explicit operator (int, int)(IVector p) => (p.X, p.Y);
	public static explicit operator (float, float)(IVector p) => (p.X, p.Y);

	public static explicit operator FVector(IVector p) => new(p.X, p.Y);

	public int Width = X;
	public int Height = Y;
}

public record struct FVector(float X, float Y)
{
	public static FVector operator *(FVector v1, float off) => new(v1.X * off, v1.Y * off);
	public static FVector operator *(FVector v1, FVector v2) => new(v1.X * v2.X, v1.Y * v2.Y);
	public static FVector operator +(FVector v1, FVector v2) => new(v1.X + v2.X, v1.Y + v2.Y);
	public static FVector operator -(FVector v1, FVector v2) => new(v1.X - v2.X, v1.Y - v2.Y);

	public static implicit operator SKPointI(FVector p) => new((int)p.X, (int)p.Y);
	public static implicit operator SKPoint(FVector p) => new(p.X, p.Y);
	public static explicit operator (int, int)(FVector p) => ((int)p.X, (int)p.Y);
	public static explicit operator (float, float)(FVector p) => (p.X, p.Y);

	public static explicit operator IVector(FVector p) => new((int)p.X, (int)p.Y);

	public float Width = X;
	public float Height = Y;
}

public record struct WinSizeStructure(int Width, int Height)
{
	public static implicit operator SKSizeI(WinSizeStructure winSize) => new SKSizeI(winSize.Width, winSize.Height);
	public static implicit operator SKSize(WinSizeStructure winSize) => new SKSize(winSize.Width, winSize.Height);
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