namespace WebGal.Libs.Base;

public record struct PositonStructure(int X, int Y);
public record struct WinSizeStructure(int Width, int Height);
public record struct UrlStructure(string Name = "", string URL = "");
public record struct ColorStructure(byte R, byte G, byte B, byte A);
public record struct PaintStructure
(
	ColorStructure Color,
	int TextSize,
	bool Blod,
	bool Antialias
);
