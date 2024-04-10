namespace WebGal.API.Data;

public enum AudioNodeType
{
	Simple, Source, Speeker, Gain, Multiplexer,
	Void, Any, Auto, Error,
}

public record struct AudioContextInfo
{
	public int ContextID { get; set; }
}