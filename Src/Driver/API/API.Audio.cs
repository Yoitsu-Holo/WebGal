namespace WebGal.API.Data;

public enum AudioNodeType
{
	Simple, Source, Speeker, Gain, Multiplexer, Pan,

	Void, Any, Auto, Error,
}

public record struct AudioIdInfo
{
	public int ContextID { get; set; }
	public int NodeID { get; set; }
	public int SocketID { get; set; }
}

public record struct AudioInfo
{
	public RequestType Request { get; set; }
	public AudioIdInfo ID { get; set; }

	public AudioNodeType Type { get; set; }
}

public record struct AudioWire
{
	public RequestType Request { get; set; }

	public AudioIdInfo SrcID { get; set; }
	public AudioIdInfo DstID { get; set; }
}