using KristofferStrube.Blazor.WebAudio;
using Microsoft.JSInterop;

namespace WebGal.Audio;

public class AudioSource(IJSRuntime jsRuntime) : AudioBase(jsRuntime)
{
	private AudioBufferSourceNode? _audioBuffer;

	public async Task StartAsync(bool start = true)
	{
		if (_audioBuffer is null)
			throw new Exception("without any auiod buffer");

		if (start)
			await _audioBuffer.StartAsync();
		else
			await _audioBuffer.StopAsync();
	}

	public async Task SetAudioBufferAsync(byte[] audioBytes)
	{
		AudioBuffer currentAudioBuffer = default!;
		if (_context is null)
			throw new Exception("Without any context");

		await _context.DecodeAudioDataAsync(audioBytes, (audioBuffer) => { currentAudioBuffer = audioBuffer; return Task.CompletedTask; });
		_audioBuffer = await _context.CreateBufferSourceAsync();
		await _audioBuffer.SetBufferAsync(currentAudioBuffer);
	}

	public async Task SetLoopAsync(bool loop)
	{
		if (_audioBuffer is null)
			throw new Exception("without any auiod buffer");
		await _audioBuffer.SetLoopAsync(loop);
	}

	// Interface
	public override async Task SetContextAsync(AudioContext context) => await base.SetContextAsync(context);

	public override async Task ConnectToAsync(IAudio target, AudioWire wire)
	{
		var inputChannels = target.InputChannels();

		if (wire.Dst >= inputChannels)
			throw new Exception("input out of range");

		if (_audioBuffer is null)
			throw new Exception("without any auiod buffer");
		await _audioBuffer.ConnectAsync(target.GetSocketAsync(), 0, wire.Dst);
	}

	public override ulong InputChannels() => 0;
	public override ulong OutputChannels() => 1;

	public override async Task DisposeAsync()
	{
		if (_audioBuffer is not null)
		{
			await _audioBuffer.DisconnectAsync();
			await _audioBuffer.StopAsync();
			await _audioBuffer.DisposeAsync();
		}

		_audioBuffer = null;
	}
}