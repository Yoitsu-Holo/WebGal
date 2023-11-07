using KristofferStrube.Blazor.WebAudio;

namespace WebGal.Audio;

public interface IAudioBaseNode
{
	public Task ConnectToAsync(AudioWire wire, IAudioBaseNode target);
	public AudioNode GetSocketAsync();
	public Task SetContextAsync(AudioContext context);
	public ulong OutputChannels();
	public ulong InputChannels();
}

public record struct AudioWire(ulong Input = 0, ulong Output = 0);