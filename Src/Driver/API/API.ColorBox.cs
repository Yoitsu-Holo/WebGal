using WebGal.Types;

namespace WebGal.API;

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


