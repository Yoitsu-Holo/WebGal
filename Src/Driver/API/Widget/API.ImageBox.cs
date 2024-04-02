namespace WebGal.API.Data;


/// <summary>
/// 图片图层设置
/// </summary>
public record struct ImageBoxInfo
{
	public int LayoutID { get; set; }
	public int LayerID { get; set; }
	public string ImageName { get; set; }
}

public record struct ImageBoxImage
{
	public int LayoutID { get; set; }
	public int LayerID { get; set; }
	public string ImageName { get; set; }
}
