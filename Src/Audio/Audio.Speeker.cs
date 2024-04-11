using KristofferStrube.Blazor.WebAudio;
using Microsoft.JSInterop;

namespace WebGal.Audio;

public class AudioSpeeker(IJSRuntime jsRuntime) : AudioBase(jsRuntime)
{
	private AudioDestinationNode? _destination;

	// Interface
	public override AudioNode GetSocketAsync()
	{
		if (_context is null)
			throw new Exception("Without any context");
		return _destination!;
	}

	public override ulong InputChannels() => 1;

	public override async Task SetContextAsync(AudioContext context)
	{
		await base.SetContextAsync(context);

		_destination = await context.GetDestinationAsync();
	}

	public override async Task DisposeAsync()
	{
		if (_destination is not null)
		{
			await _destination.DisconnectAsync();
			await _destination.DisposeAsync();
		}

		_destination = null;
	}
}