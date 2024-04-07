using KristofferStrube.Blazor.WebAudio;
using Microsoft.JSInterop;

namespace WebGal.Audio;

public class AudioSpeeker(IJSRuntime jsRuntime) : IAudio
{
	private IJSRuntime _jsRuntime = jsRuntime;
	private AudioContext? _context;

	private AudioDestinationNode? _destination;

	// Interface
	public Task ConnectToAsync(IAudio target, AudioWire wire) => throw new NotImplementedException();

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