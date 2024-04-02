namespace WebGal.API.Data;

public record struct LayerBox
{
	public RequestHeader Request { get; set; }
	public LayerInfo Attribute { get; set; }
}