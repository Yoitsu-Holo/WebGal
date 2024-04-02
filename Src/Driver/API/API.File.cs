namespace WebGal.API.Data;

public record struct FileInfo
{
	public RequestHeader Request { get; set; }


	public FileType Type { get; set; }
	public string Name { get; set; }
	public string URL { get; set; }
}