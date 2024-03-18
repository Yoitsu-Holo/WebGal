using WebGal.Types;

namespace WebGal.API;

/// <summary>
/// 基本请求类型
/// </summary>
public enum RequestType
{
	// 获取，设置，删除
	Get, Set, Delete,

	// 任意类型，空类型，错误
	Any, Void, Error,
}

/// <summary>
/// 基本返回类型
/// </summary>
public enum ResponseType
{
	Success, Fail,

	// 任意类型，空类型，错误
	Any, Void, Error,
}


/// <summary>
/// 图层类型
/// </summary>
public enum LayerType
{
	TextBox, ImageBox, ControllerBox,

	Any, Void, Error,
}

/// <summary>
/// 请求结构头
/// </summary>
public record struct RequestHeader
{
	public RequestType Type;
	public string Message;
}

/// <summary>
/// 响应结构头
/// </summary>
public record struct ResponseHeader
{
	public ResponseType Type;
	public string Message;
}

/// <summary>
/// 图层信息
/// </summary>
public record struct LayerInfo
{
	public LayerType Type;
	public IVector Position;
	public IVector Size;

	public int LayerID; // 图层编号，数字越大表示越在上方
}

/// <summary>
/// 文本框设置，根据 RequestHeader.RequestType 字段确定行为: 
/// Get: 获取基本信息;
/// Set: 设置信息（未存在则新建，存在则覆盖）;
/// Del: 删除;
/// </summary>
public record struct TextBox
{
	public RequestHeader Request;
	public ResponseHeader Response;

	public LayerInfo Attribute;

	public string Name;
	public string Text;
	public string Font;
	public int FontSize;
}

public record struct TextBoxInfo
{
	public RequestHeader Request;
	public ResponseHeader Response;

	public string Name;
	public string Text;
	public string Font;
	public int FontSize;
}

public record struct SetTextBoxText(string Name, string Text);
public record struct SetTextBoxFont(string Name, string Font);
public record struct SetTextBoxFontSize(string Name, int FontSize);



/// <summary>
/// 图片图层设置
/// </summary>
public record struct ImageBox
{
	public RequestHeader Request;
	public ResponseHeader Response;

	public LayerInfo Attribute;

	public string Name;
	public string ImageName;
	public IVector Offset;
}

public record struct ImageBoxInfo
{
	public RequestHeader Request;
	public ResponseHeader Response;

	public string Name;
	public string ImageName;
	public IVector Offset;
}

public record struct SetImageBoxImage(string Name, string ImageName);
public record struct SetImageBoxOffset(string Name, IVector Offset);


/// <summary>
/// 单颜色色彩快设置
/// </summary>
public record struct ColorBox
{
	public RequestHeader Request;
	public ResponseHeader Response;

	public LayerInfo Attribute;

	public string Name;
	public byte R, G, B, A;
	public IVector Offset;
}

public record struct ColorBoxInfo
{
	public RequestHeader Request;
	public ResponseHeader Response;

	public string Name;
	public byte R, G, B, A;
	public IVector Offset;
}

public record struct ColorBoxInfoColor(string Name, byte R, byte G, byte B, byte A);
public record struct ColorBoxInfoOffset(string Name, IVector Offset);


public record struct Audio