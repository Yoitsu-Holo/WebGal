using WebGal.Types;

namespace WebGal.API.Data;

/// <summary>
/// 图片图层设置
/// </summary>
public record struct ButtomBoxInfo
{
	public int LayoutID { get; set; }
	public int LayerID { get; set; }
	public ImageInfo NormalImage { get; set; }
	public ImageInfo HoverImage { get; set; }
	public ImageInfo PressedImage { get; set; }
	public ImageInfo FocusedImage { get; set; }
}

public record struct ButtomBoxImage
{
	public int LayoutID { get; set; }
	public int LayerID { get; set; }
	public ControllerStatus Status { get; set; }
	public string ImageName { get; set; }
}

public record struct ButtomBoxSubImage
{
	public int LayoutID { get; set; }
	public int LayerID { get; set; }
	public ControllerStatus Status { get; set; }
	public IRect SubRect { get; set; }
}