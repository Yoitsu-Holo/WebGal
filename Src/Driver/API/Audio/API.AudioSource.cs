namespace WebGal.API.Data;

public record struct AudioSourceInfo
{
	public AudioIdInfo ID { get; set; }

	public string AudioName { get; set; }
	public bool Start { get; set; }
	public bool Loop { get; set; }
}