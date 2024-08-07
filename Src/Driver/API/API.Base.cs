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

public enum ControllerStatus
{
	// 通常状态，悬停状态，按下状态，聚焦状态 (突出显示)
	Normal, Hover, Pressed, Focused,
}

/// <summary> 响应结构头 </summary>
public record struct Response
{
	public Response()
	{
		Type = ResponseType.Success;
		Message = "";
	}

	public Response(string message, ResponseType type = ResponseType.Fail)
	{
		Type = type;
		Message = message;
	}

	public ResponseType Type { get; set; }
	public string Message { get; set; }
}

/// <summary>  游戏设置  </summary>
public record struct GameInfo
{
	public RequestType Request { get; set; }

	public string Name { get; set; }
	public IVector Resolution { get; set; }
}

public record struct ImageInfo
{
	public string ImageName { get; set; }
	public IRect SubRect { get; set; }
}