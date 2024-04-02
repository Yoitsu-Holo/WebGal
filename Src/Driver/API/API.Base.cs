using WebGal.Types;

namespace WebGal.API.Data;

/// <summary> 基本请求类型 </summary>
public enum RequestType
{
	// 获取，设置，删除
	Get, Set, Delete,

	// 任意类型，空类型，自动解析，错误
	Void, Any, Auto, Error,
}

/// <summary> 基本返回类型 </summary>
public enum ResponseType
{
	Success, Fail,

	// 任意类型，空类型，自动解析，错误
	Void, Any, Auto, Error,
}

/// <summary> 图层类型 </summary>
public enum LayerType
{
	TextBox, ImageBox, ColorBox,    // widget
	ControllerBox, // controller

	// 任意类型，空类型，自动解析，错误
	Void, Any, Auto, Error,
}

/// <summary> 文件类型 </summary>
public enum FileType
{
	Script,
	Image,
	Audio,
	Bin,
	Font,


	Void, Any, Auto, Error,
}

/// <summary> 请求结构头 </summary>
public record struct RequestHeader
{
	public RequestType Type { get; set; }
	public string Message { get; set; }
}

/// <summary> 响应结构头 </summary>
public record struct ResponseHeader
{
	public ResponseType Type { get; set; }
	public string Message { get; set; }
}

/// <summary> 布局信息 </summary>
public record struct LayoutInfo
{
	public RequestHeader Request { get; set; }

	public int LayoutId { get; set; }
}

/// <summary> 图层信息 </summary>
public record struct LayerInfo
{
	public LayerType Type { get; set; }
	public IVector Position { get; set; }
	public IVector Size { get; set; }

	public int LayoutID { get; set; }
	public int LayerID { get; set; } // 图层编号，数字越大表示越在上方
}

/// <summary>  游戏设置  </summary>
public record struct GameInfo
{
	public RequestHeader Request { get; set; }

	public string Name { get; set; }
	public IVector Resolution { get; set; }
}