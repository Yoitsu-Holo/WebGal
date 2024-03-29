using WebGal.Types;

namespace WebGal.API;


/// <summary>
/// 图片图层设置
/// </summary>
public record struct ImageBox
{
	public RequestHeader Request;

	public LayerInfo Attribute;

	public string Name;
	public string ImageName;
	public IVector Offset;
}

public record struct ImageBoxInfo
{
	public RequestHeader Request;

	public string Name;
	public string ImageName;
	public IVector Offset;
}

public record struct SetImageBoxImage(string Name, string ImageName);
public record struct SetImageBoxOffset(string Name, IVector Offset);

//^ 特化图像标签，设置背景、角色、立绘操作
public record struct BackGround(string ImageName, IVector Offset);
public record struct Character(string ImageName, IVector Offset);
public record struct Stand(string ImageName, IVector Offset);
