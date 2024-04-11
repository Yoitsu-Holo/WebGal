using KristofferStrube.Blazor.WebAudio;
using Microsoft.JSInterop;

namespace WebGal.Audio;

public class AudioBase(IJSRuntime jsRuntime) : IAudio
{
	protected readonly IJSRuntime _jsRuntime = jsRuntime;
	protected AudioContext? _context;

	// Interface
	public virtual Task ConnectToAsync(IAudio target, AudioWire wire) => throw new NotImplementedException();
	public virtual AudioNode GetSocketAsync() => throw new NotImplementedException();
	public virtual ulong InputChannels() => 0;
	public virtual ulong OutputChannels() => 0;
	public virtual async Task SetContextAsync(AudioContext context)
	{
		await Task.Run(() => { });
		_context = context;
	}
}