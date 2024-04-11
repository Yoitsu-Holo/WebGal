namespace WebGal.API.Data;

public record struct AudioSimpleInfo
{
	public AudioIdInfo ID { get; set; }

	public string AudioName { get; set; }
	public bool Start { get; set; }
	public bool Loop { get; set; }

	/// <summary> Range : [0-1000] </summary>
	public int Volume { get; set; }
}