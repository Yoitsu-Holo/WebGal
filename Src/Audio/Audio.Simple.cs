using KristofferStrube.Blazor.WebAudio;
using Microsoft.JSInterop;
using WebGal.Audio;

namespace WebGal.Libs.Base;

/*
? 基本音频分为四类
		名称	描述			数量	作用				属性
^		audio:	最基本的音乐	无限量	用于播放任何音频	需要设置属性，默认单次，无淡入淡出，100%音量
todo:	music:	音乐播放通道	数量1	用于播放背景音乐	循环
todo:	vioce:	语音播放通道	配置	用于播放人物语音	单次
todo:	effect:	音效播放通道	数量1	用于播放场景音效	单次
*/

public class AudioSimple(IJSRuntime jsRuntime) : AudioBase(jsRuntime)
{
	private AudioDestinationNode? _destination;
	private GainNode? _gain;
	private AudioBufferSourceNode? _currentAudioBufferNode;

	public async Task SetAudioAsync(byte[] audioBytes)
	{
		if (_context is null)
			throw new Exception("Without any context");
		if (audioBytes is null)
			throw new Exception("Empty Audio Bytes");

		// 创建音频缓冲区
		AudioBuffer currentAudioBuffer = default!;
		await _context.DecodeAudioDataAsync(audioBytes, (audioBuffer) => { currentAudioBuffer = audioBuffer; return Task.CompletedTask; });
		await _currentAudioBufferNode!.SetBufferAsync(currentAudioBuffer);
	}

	public async Task SetLoop(bool loop)
	{
		if (_context is null)
			throw new Exception("Without any context");

		// 设置循环属性
		await _currentAudioBufferNode!.SetLoopAsync(loop);
	}

	public async Task StartAsync()
	{
		if (_context is null)
			throw new Exception("Without any context");
		await _currentAudioBufferNode!.StartAsync();
	}

	public async Task StopAsunc()
	{
		if (_context is null)
			throw new Exception("Without any context");
		await _currentAudioBufferNode!.StopAsync();
	}

	public async Task SetVolume(float volume)
	{
		if (_context is null)
			throw new Exception("Without any context");
		await (await _gain!.GetGainAsync()).SetValueAsync(volume);
	}

	// Interface
	public override async Task SetContextAsync(AudioContext context)
	{
		await base.SetContextAsync(context);
		if (_context is null)
			throw new Exception("Without any context");

		_currentAudioBufferNode = await _context.CreateBufferSourceAsync();
		_gain = await GainNode.CreateAsync(_jsRuntime, _context, new() { Gain = 1.0f });
		_destination = await context.GetDestinationAsync();

		await _currentAudioBufferNode.ConnectAsync(_gain);
		await _gain.ConnectAsync(_destination);
	}
}