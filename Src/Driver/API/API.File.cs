namespace WebGal.API.Data;

/// <summary> 文件类型 </summary>
public enum FileType
{
	Script, Image, Audio, Bin, Font,


	Void, Any, Auto, Error,
}

public record struct FileInfo
{
	public FileType Type { get; set; }
	public string Name { get; set; }
	public string URL { get; set; }
}