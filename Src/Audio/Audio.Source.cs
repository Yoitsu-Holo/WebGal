using KristofferStrube.Blazor.WebAudio;
using Microsoft.JSInterop;

namespace WebGal.Audio;

public class AudioSource(IJSRuntime jsRuntime) : AudioBase(jsRuntime)
{
	private AudioBufferSourceNode? _audioBuffer;

	public async Task PlayAsync()
	{
		if (_audioBuffer is null)
			throw new Exception("without any auiod buffer");
		await _audioBuffer.StartAsync();
	}

	public async Task StopAsync()
	{
		if (_audioBuffer is null)
			throw new Exception("without any auiod buffer");
		await _audioBuffer.StopAsync();
	}

	public async Task SetAudioBuffer(byte[] audioBytes)
	{
		AudioBuffer currentAudioBuffer = default!;
		if (_context is null)
			throw new Exception("Without any context");

		await _context.DecodeAudioDataAsync(audioBytes, (audioBuffer) => { currentAudioBuffer = audioBuffer; return Task.CompletedTask; });
		_audioBuffer = await _context.CreateBufferSourceAsync();
		await _audioBuffer.SetBufferAsync(currentAudioBuffer);
	}

	public async Task SetAudioLoop(bool loop)
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

		if (wire.Input >= inputChannels)
			throw new Exception("input out of range");

		if (_audioBuffer is null)
			throw new Exception("without any auiod buffer");
		await _audioBuffer.ConnectAsync(target.GetSocketAsync(), 0, wire.Input);
	}

	public override ulong InputChannels() => 0;
	public override ulong OutputChannels() => 1;
}