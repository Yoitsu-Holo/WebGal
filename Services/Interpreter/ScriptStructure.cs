namespace WebGal.Services.Data;

#region game structure

public record struct NodeStructure
(
	bool IsLeaf,
	List<UrlStructure>? ResouresPackURL,
	List<UrlStructure>? NodeURL,
	List<UrlStructure>? SceneURL
);

public record struct SceneStructure
(
	List<UrlStructure>? ResouresPackURL,
	List<LayerStructure>? Layer
);
#endregion

#region layer structure
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
	string? Text,
	PositonStructure Offset,
	PaintStructure Paint
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

public record struct ColorStructure(int R, int G, int B, int A);

#endregion

#region resource structure
public record struct ResouresStructure
(
	List<UrlStructure>? ImageURL,
	List<UrlStructure>? AudioURL,
	List<UrlStructure>? TextURL
);
#endregion

public record struct UrlStructure(string Name = "", string URL = "");

#region test structure
public record struct TestStructure
(
	// public List<string>? Test 
	List<UrlStructure>? Test
);
#endregion