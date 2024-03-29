namespace WebGal.API;

/// <summary>
/// 文本框设置，根据 RequestHeader.RequestType 字段确定行为: 
/// Get: 获取基本信息;
/// Set: 设置信息（未存在则新建，存在则覆盖）;
/// Del: 删除;0
/// </summary>
public record struct TextBox
{
	public RequestHeader Request;

	public LayerInfo Attribute;

	public string Name;
	public string Text;
	public string Font;
	public int FontSize;
}

public record struct TextBoxInfo
{
	public RequestHeader Request;

	public string Name;
	public string Text;
	public string Font;
	public int FontSize;
}

public record struct SetTextBoxText(string Name, string Text);
public record struct SetTextBoxFont(string Name, string Font);
public record struct SetTextBoxFontSize(string Name, int FontSize);