namespace WebGal.API.Data;

/// <summary>
/// 单颜色色彩快设置
/// </summary>
public record struct ColorBoxInfo
{
	public int LayoutID { get; set; }
	public int LayerID { get; set; }
	public byte R { get; set; }
	public byte G { get; set; }
	public byte B { get; set; }
	public byte A { get; set; }
}

public record struct ColorBoxColor
{
	public int LayoutID { get; set; }
	public int LayerID { get; set; }
	public byte R { get; set; }
	public byte G { get; set; }
	public byte B { get; set; }
	public byte A { get; set; }
}

