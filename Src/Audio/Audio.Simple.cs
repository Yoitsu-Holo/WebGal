using KristofferStrube.Blazor.WebAudio;
using WebGal.Global;

namespace WebGal.Audio;

/*
? 基本音频分为四类
		名称	描述			数量	作用				属性
^		audio:	最基本的音乐	无限量	用于播放任何音频	需要设置属性，默认单次，无淡入淡出，100%音量
todo:	music:	音乐播放通道	数量1	用于播放背景音乐	循环
todo:	vioce:	语音播放通道	配置	用于播放人物语音	单次
todo:	effect:	音效播放通道	数量1	用于播放场景音效	单次
*/

public class AudioSimple : AudioBase
{
	private AudioDestinationNode? _destination;
	private GainNode? _gain;
	private AudioBufferSourceNode? _audioBuffer;

	private bool _started = false;

	public async Task SetAudioBufferAsync(byte[] audioBytes)
	{
		if (_context is null) { Logger.LogInfo("未设置音频上下文", Global.LogLevel.Warning); return; }


		if (audioBytes is null)
			throw new Exception("Empty Audio Bytes");

		// 创建音频缓冲区
		AudioBuffer currentAudioBuffer = default!;
		await _context.DecodeAudioDataAsync(audioBytes, (audioBuffer) => { currentAudioBuffer = audioBuffer; return Task.CompletedTask; });
		if (_audioBuffer is not null)
		{
			await _audioBuffer.StopAsync();
			await _audioBuffer.DisconnectAsync();
			await _audioBuffer.DisposeAsync();
		}
		_audioBuffer = await _context.CreateBufferSourceAsync();
		await _audioBuffer.ConnectAsync(_gain!);
		await _audioBuffer.SetBufferAsync(currentAudioBuffer);
	}

	public async Task SetLoopAsync(bool loop)
	{
		if (_context is null) { Logger.LogInfo("未设置音频上下文", Global.LogLevel.Warning); return; }


		// 设置循环属性
		await _audioBuffer!.SetLoopAsync(loop);
	}

	public async Task StartAsync(bool start = true)
	{
		if (_context is null) { Logger.LogInfo("未设置音频上下文", Global.LogLevel.Warning); return; }
		if (_audioBuffer is null) { Logger.LogInfo("无音频缓冲区", Global.LogLevel.Warning); return; }

		if (start)
		{
			await _audioBuffer!.StartAsync();
			_started = true;
		}
		else if (_started)
			await _audioBuffer!.StopAsync();
	}

	public async Task SetGain(float Gain)
	{
		if (_context is null) { Logger.LogInfo("未设置音频上下文", Global.LogLevel.Warning); return; }

		await (await _gain!.GetGainAsync()).SetValueAsync(Gain);
	}

	// Interface
	public override async Task SetContextAsync(AudioContext context)
	{
		await base.SetContextAsync(context);
		if (_context is null) { Logger.LogInfo("未设置音频上下文", Global.LogLevel.Warning); return; }


		_gain = await _context.CreateGainAsync();
		_destination = await context.GetDestinationAsync();

		await _gain.ConnectAsync(_destination);
	}

	public override async ValueTask DisposeAsync()
	{
		if (_destination is not null)
		{
			await _destination.DisconnectAsync();
			await _destination.DisposeAsync();
		}
		if (_gain is not null)
		{
			await _gain.DisconnectAsync();
			await _gain.DisposeAsync();
		}
		if (_audioBuffer is not null)
		{
			await _audioBuffer.StopAsync();
			await _audioBuffer.DisconnectAsync();
			await _audioBuffer.DisposeAsync();
		}

		GC.SuppressFinalize(this);
	}
}