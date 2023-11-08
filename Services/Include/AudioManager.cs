using WebGal.Audio;

namespace WebGal.Services.Include;

public class AudioManager
{
	public Dictionary<string, IAudioBaseNode> audioNode = new();
}