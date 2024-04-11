namespace WebGal.API.Data;

public record struct AudioMutiplexerInfo
{
	public AudioIdInfo ID { get; set; }

	public ulong InputChannels { get; set; }
	public ulong OutputChannels { get; set; }
}