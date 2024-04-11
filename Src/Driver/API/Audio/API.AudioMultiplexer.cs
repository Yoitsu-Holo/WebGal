namespace WebGal.API.Data;

public record struct AudioMutiplexerInfo
{
	public AudioIdInfo ID { get; set; }

	public int InputChannels { get; set; }
	public int OutputChannels { get; set; }
}