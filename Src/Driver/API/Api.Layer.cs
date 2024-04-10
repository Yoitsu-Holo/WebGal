using WebGal.Types;

namespace WebGal.API.Data;

/// <summary> 图层类型 </summary>
public enum LayerType
{
	TextBox, ImageBox, ColorBox,    // widget
	ButtomBox,
	ControllerBox, // controller

	// 任意类型，空类型，自动解析，错误
	Void, Any, Auto, Error,
}

/// <summary> 布局信息 </summary>
public record struct LayoutInfo
{
	public RequestHeader Request { get; set; }

	public int LayoutId { get; set; }
}

/// <summary> 图层信息 </summary>
public record struct LayerInfo
{
	public LayerType Type { get; set; }
	public IVector Position { get; set; }
	public IVector Size { get; set; }

	public int LayoutID { get; set; }
	public int LayerID { get; set; } // 图层编号，数字越大表示越在上方
}

public record struct LayerBox
{
	public RequestHeader Request { get; set; }
	public LayerInfo Attribute { get; set; }
}

public record struct LayerIdInfo
{
	public int LayoutID { get; set; }
	public int LayerID { get; set; }
}