using WebGal.Audio;

namespace WebGal.Services.Include;


/// <summary>
/// AudioManager 接收来自 Scene 或其他模块的请求，根据需要播放相应的音频。
/// </summary>
public class AudioManager
{
	public Dictionary<string, IAudioBaseNode> audioNode = new();
}