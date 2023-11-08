using KristofferStrube.Blazor.WebAudio;
using Microsoft.JSInterop;

namespace WebGal.Audio;

public class AudioSpeeker : IAudioBaseNode
{
	private IJSRuntime _jsRuntime = null!;
	private AudioContext? _context;

	private AudioDestinationNode? _destination;


	public AudioSpeeker(IJSRuntime jsRuntime) => _jsRuntime = jsRuntime;

	// Interface
	public Task ConnectToAsync(IAudioBaseNode target, AudioWire wire) => throw new NotImplementedException();

	public AudioNode GetSocketAsync()
	{
		if (_context is null)
			throw new Exception("Without any context");
		return _destination!;
	}

	public ulong InputChannels() => 1;
	public ulong OutputChannels() => throw new NotImplementedException();

	public async Task SetContextAsync(AudioContext context)
	{
		_context = context;

		_destination = await context.GetDestinationAsync();
	}
}