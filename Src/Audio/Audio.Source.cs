using KristofferStrube.Blazor.WebAudio;
using Microsoft.JSInterop;
using WebGal.Global;

namespace WebGal.Audio;

public class AudioSource(IJSRuntime jsRuntime) : AudioBase(jsRuntime)
{
	private AudioBufferSourceNode? _audioBuffer;
	private bool _started = false;

	public async Task StartAsync(bool start = true)
	{
		if (_context is null) { Logger.LogInfo("未设置音频上下文", Global.LogLevel.Warning); return; }
		if (_audioBuffer is null) { Logger.LogInfo("无音频缓冲区", Global.LogLevel.Warning); return; }

		if (start)
		{
			await _audioBuffer.StartAsync();
			_started = true;
		}
		else if (_started)
			await _audioBuffer.StopAsync();
	}

	public async Task SetAudioBufferAsync(byte[] audioBytes)
	{
		AudioBuffer currentAudioBuffer = default!;

		if (_context is null) { Logger.LogInfo("未设置音频上下文", Global.LogLevel.Warning); return; }

		await _context.DecodeAudioDataAsync(audioBytes, (audioBuffer) => { currentAudioBuffer = audioBuffer; return Task.CompletedTask; });
		_audioBuffer = await _context.CreateBufferSourceAsync();
		await _audioBuffer.SetBufferAsync(currentAudioBuffer);
	}

	public async Task SetLoopAsync(bool loop)
	{
		if (_audioBuffer is null) { Logger.LogInfo("无音频缓冲区", Global.LogLevel.Warning); return; }

		await _audioBuffer.SetLoopAsync(loop);
	}

	// Interface
	public override async Task SetContextAsync(AudioContext context) => await base.SetContextAsync(context);

	public override async Task ConnectToAsync(IAudio target, AudioWire wire)
	{
		if (_context is null) { Logger.LogInfo("未设置音频上下文", Global.LogLevel.Warning); return; }
		if (_audioBuffer is null) { Logger.LogInfo("无音频缓冲区", Global.LogLevel.Warning); return; }
		if (wire.Src >= OutputChannels()) { Logger.LogInfo("源接口超过限制", Global.LogLevel.Warning); return; }
		if (wire.Dst >= target.InputChannels()) { Logger.LogInfo("目标接口超过限制", Global.LogLevel.Warning); return; }

		await _audioBuffer.ConnectAsync(target.GetSocketAsync(), 0, wire.Dst);
	}

	public override ulong OutputChannels() => 1;

	public override async ValueTask DisposeAsync()
	{
		if (_audioBuffer is not null)
		{
			await _audioBuffer.StopAsync();
			await _audioBuffer.DisconnectAsync();
			await _audioBuffer.DisposeAsync();
		}

		GC.SuppressFinalize(this);
	}
}