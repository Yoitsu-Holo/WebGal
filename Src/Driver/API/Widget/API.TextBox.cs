namespace WebGal.API.Data;

/// <summary>
/// 文本框设置，根据 RequestHeader.RequestType 字段确定行为: 
/// Get: 获取基本信息;
/// Set: 设置信息（未存在则新建，存在则覆盖）;
/// Del: 删除;0
/// </summary>
public record struct TextBoxInfo
{
	public int LayoutID { get; set; }
	public int LayerID { get; set; }

	public string Text { get; set; }
	public string Font { get; set; }
	public int FontSize { get; set; }
}

public record struct TextBoxText
{
	public int LayoutID { get; set; }
	public int LayerID { get; set; }
	public string Text { get; set; }
}

public record struct TextBoxFont
{
	public int LayoutId { get; set; }
	public int LayerId { get; set; }
	public string Font { get; set; }
}

public record struct TextBoxFontSize
{
	public int LayoutID { get; set; }
	public int LayerID { get; set; }
	public int FontSize { get; set; }
}