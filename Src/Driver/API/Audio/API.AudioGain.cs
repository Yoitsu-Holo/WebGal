namespace WebGal.API.Data;

public record struct AudioGainInfo
{
	public AudioIdInfo ID { get; set; }

	/// <summary> Range : [0-1000] </summary>
	public int Volume { get; set; }
}