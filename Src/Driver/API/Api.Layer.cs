namespace WebGal.API.Data;

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