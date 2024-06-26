using KristofferStrube.Blazor.WebAudio;

namespace WebGal.Audio;

public interface IAudio : IAsyncDisposable
{
	public Task ConnectToAsync(IAudio target, AudioWire wire);
	public AudioNode GetSocketAsync();
	public Task SetContextAsync(AudioContext context);
	public ulong OutputChannels();
	public ulong InputChannels();
}

public record struct AudioWire
{
	public ulong Src;
	public ulong Dst;
}