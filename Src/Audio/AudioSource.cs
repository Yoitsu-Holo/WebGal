using KristofferStrube.Blazor.WebAudio;
using Microsoft.JSInterop;

namespace WebGal.Audio;

public class AudioSource : IAudioBaseNode
{
	private readonly IJSRuntime _jsRuntime = null!;
	private AudioContext _context = null!;
	private AudioBufferSourceNode? _audioBuffer;


	public AudioSource(IJSRuntime jsRuntime) => _jsRuntime = jsRuntime;

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
	public async Task SetContextAsync(AudioContext context) => await Task.Run(() => _context = context);

	public async Task ConnectToAsync(IAudioBaseNode target, AudioWire wire)
	{
		var inputChannels = target.InputChannels();

		if (wire.Input >= inputChannels)
			throw new Exception("input out of range");

		if (_audioBuffer is null)
			throw new Exception("without any auiod buffer");
		await _audioBuffer.ConnectAsync(target.GetSocketAsync(), 0, wire.Input);
	}

	public ulong InputChannels() => 0;
	public ulong OutputChannels() => 1;
	public AudioNode GetSocketAsync() => throw new NotImplementedException();
	public string NodeMeta() => throw new NotImplementedException();
}