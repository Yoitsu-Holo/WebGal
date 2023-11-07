namespace WebGal.Types;

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


public record struct LayerStructureRegister
(
	// 图层名称
	int C_sName,
	// 图层属性
	int C_sLayerType,
	LayerAtrributeRegister O_Attribute,
	IVectorRegister O_Position,
	WinSizeStructureRegister O_WinSize,
	// 图层动画
	int C_iTime,
	int C_sAnimation,
	IVectorRegister O_BeginPosition,
	IVectorRegister O_EndPosition,
	// 背景图片
	int C_sImage,
	IVectorRegister O_CutPosition,
	WinSizeStructureRegister O_CutWinSize,
	// 简单图形
	ColorStructureRegister O_ShapeColor,
	// 文字
	List<TextStructureRegister>? Text
);

public record struct LayerAtrributeRegister
(
	int O_Offset,
	int C_bIsHide
);

public record struct TextStructureRegister
(
	int O_Offset,
	int O_Paint,
	int C_sText
);