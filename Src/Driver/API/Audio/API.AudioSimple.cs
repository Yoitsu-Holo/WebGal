namespace WebGal.API.Data;

public record struct AudioSimpleInfo
{
	public AudioIdInfo ID { get; set; }

	public string AudioName { get; set; }
	public bool Start { get; set; }
}