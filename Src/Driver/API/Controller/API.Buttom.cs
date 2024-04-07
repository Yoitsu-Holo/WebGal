using WebGal.Types;

namespace WebGal.API.Data;

/// <summary>
/// 图片图层设置
/// </summary>
public record struct ButtomBoxInfo
{
	public LayerIdInfo ID { get; set; }
	public ImageInfo NormalImage { get; set; }
	public ImageInfo HoverImage { get; set; }
	public ImageInfo PressedImage { get; set; }
	public ImageInfo FocusedImage { get; set; }
}

public record struct ButtomBoxImage
{
	public LayerIdInfo ID { get; set; }
	public ControllerStatus Status { get; set; }
	public string ImageName { get; set; }
}

public record struct ButtomBoxSubImage
{
	public LayerIdInfo ID { get; set; }
	public ControllerStatus Status { get; set; }
	public IRect SubRect { get; set; }
}