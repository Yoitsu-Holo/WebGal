using WebGal.Types;

namespace WebGal.API.Data;

/// <summary> 布局信息 </summary>
// public record struct LayoutInfo
// {
// 	public RequestType Request { get; set; }

// 	public int LayoutID { get; set; }
// }

/// <summary> 图层信息 </summary>
public record struct LayerInfo
{
	public string Type { get; set; }
	public IVector Position { get; set; }
	public IVector Size { get; set; }

	public LayerIdInfo ID { get; set; }
}

public record struct LayerBox
{
	public RequestType Request { get; set; }
	public LayerInfo Attribute { get; set; }
}

public record struct LayerIdInfo
{
	public RequestType Request { get; set; }
	public int LayoutID { get; set; }
	public int LayerID { get; set; }
}