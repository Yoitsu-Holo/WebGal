namespace WebGal.Libs.Base;

public record struct LayerStructure
(
	// 图层名称
	string? Name,
	// 图层属性
	string? LayerType,
	LayerAtrribute Attribute,
	IVector Position,
	WinSizeStructure WinSize,
	// 图层动画
	int Time,
	string? Animation,
	IVector BeginPosition,
	IVector EndPosition,
	// 背景图片
	string? Image,
	IVector CutPosition,
	WinSizeStructure CutWinSize,
	// 简单图形
	ColorStructure ShapeColor,
	// 文字
	List<TextStructure>? Text
);

public record struct LayerAtrribute
(
	IVector Offset,
	bool IsHide
);

public record struct TextStructure
(
	IVector Offset,
	PaintStructure Paint,
	string? Text
);

