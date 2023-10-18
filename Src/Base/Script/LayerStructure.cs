namespace WebGal.Libs.Base;

public record struct LayerStructure
(
	// 图层名称
	string? Name,
	// 图层属性
	string? LayerType,
	LayerAtrribute Atrribute,
	PositonStructure Position,
	WinSizeStructure WinSize,
	// 图层动画
	int Time,
	string? Animation,
	PositonStructure BeginPosition,
	PositonStructure EndPosition,
	// 背景图片
	string? Image,
	PositonStructure CutPosition,
	WinSizeStructure CutWinSize,
	// 简单图形
	ColorStructure ShapeColor,
	// 文字
	List<TextStructure>? Text
);

public record struct LayerAtrribute
(
	bool IsHide
);

public record struct TextStructure
(
	PositonStructure Offset,
	PaintStructure Paint,
	string? Text
);

