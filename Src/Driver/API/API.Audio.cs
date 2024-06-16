namespace WebGal.API.Data;

// public enum AudioNodeType
// {
// 	Simple, Source, Speeker, Gain, Multiplexer, Pan,

// 	Void, Any, Auto, Error,
// }

public record struct AudioIdInfo
{
	public int ContextID { get; set; }
	public int NodeID { get; set; }
	public ulong SocketID { get; set; }
}

public record struct AudioNodeInfo
{
	public RequestType Request { get; set; }
	public AudioIdInfo ID { get; set; }

	public string Type { get; set; }
}

public record struct AudioWireInfo
{
	public RequestType Request { get; set; }

	public AudioIdInfo SrcID { get; set; }
	public AudioIdInfo DstID { get; set; }
}