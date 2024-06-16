using KristofferStrube.Blazor.WebAudio;

namespace WebGal.Audio;

public class AudioSpeeker : AudioBase
{
	private AudioDestinationNode? _destination;

	// Interface
	public override AudioNode GetSocketAsync()
	{
		if (_context is null) throw new Exception("未设置音频上下文");

		return _destination!;
	}

	public override ulong InputChannels() => 1;

	public override async Task SetContextAsync(AudioContext context)
	{
		await base.SetContextAsync(context);

		_destination = await context.GetDestinationAsync();
	}

	public override async ValueTask DisposeAsync()
	{
		if (_destination is not null)
		{
			await _destination.DisconnectAsync();
			await _destination.DisposeAsync();
		}

		GC.SuppressFinalize(this);
	}
}