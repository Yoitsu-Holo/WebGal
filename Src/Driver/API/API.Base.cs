using WebGal.Types;

namespace WebGal.API;

/// <summary>
/// 基本请求类型
/// </summary>
public enum RequestType
{
	// 获取，设置，删除
	Get, Set, Delete,

	// 任意类型，空类型，自动解析，错误
	Any, Void, Auto, Error,
}

/// <summary>
/// 基本返回类型
/// </summary>
public enum ResponseType
{
	Success, Fail,

	// 任意类型，空类型，自动解析，错误
	Any, Void, Auto, Error,
}

/// <summary>
/// 图层类型
/// </summary>
public enum LayerType
{
	TextBox, ImageBox, ColorBox,    // widget
	ControllerBox, // controller

	// 任意类型，空类型，自动解析，错误
	Any, Void, Auto, Error,
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
/// 游戏设置
/// </summary>
public record struct GameInfo
{
	public RequestHeader Request;

	public string Name;
	public IVector Resolution;
}