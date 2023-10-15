namespace WebGal.Libs.Base;

public record struct NodeStructure
(
	bool IsLeaf,
	List<string>? LoopAudio,
	List<string>? OneShotAudio,
	List<UrlStructure>? ResouresPackURL,
	List<UrlStructure>? NodeURL
);

public record struct SceneStructure
(
	bool IsStatic,
	List<string>? LoopAudio,
	List<string>? OneShotAudio,
	List<LayerStructure>? Layer
);

public record struct LayerStructure
(
	// 图层名称
	string? Name,
	// 图层属性
	bool IsImageLayer,
	bool IsShapeLayer,
	bool IsTextLayer,
	PositonStructure Position,
	WinSizeStructure WinSize,
	// 图层动画
	int Time,
	string? Animation,
	PositonStructure BeginPosition,
	PositonStructure EndPosition,
	// 背景图片
	string? Image,
	// 简单图形
	ColorStructure ShapeColor,
	// 文字
	List<TextStructure>? Text
);

public record struct TextStructure
(
	PositonStructure Offset,
	PaintStructure Paint,
	string Text = ""
);

public record struct PositonStructure(int X, int Y);
public record struct WinSizeStructure(int Width, int Height);
public record struct PaintStructure
(
	ColorStructure Color,
	int TextSize,
	bool Blod,
	bool Antialias
);

public record struct ColorStructure(byte R, byte G, byte B, byte A);

public record struct ResouresStructure
(
	List<UrlStructure>? ImageURL,
	List<UrlStructure>? AudioURL,
	List<UrlStructure>? TextURL
);

public record struct UrlStructure(string Name = "", string URL = "");

public record struct TestStructure
(
	List<UrlStructure>? Test
);
