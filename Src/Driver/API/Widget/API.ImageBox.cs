namespace WebGal.API.Data;


/// <summary>
/// 图片图层设置
/// </summary>
public record struct ImageBoxInfo
{
	public LayerIdInfo ID { get; set; }
	public ImageInfo Image { get; set; }
}

public record struct ImageBoxImage
{
	public LayerIdInfo ID { get; set; }
	public ImageInfo Image { get; set; }
}