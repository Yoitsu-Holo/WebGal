using KristofferStrube.Blazor.WebAudio;
using Microsoft.JSInterop;

namespace WebGal.Libs.Base;

/*
? 基本音频分为四类
		名称	描述			数量	作用				属性
^		audio:	最基本的音乐	无限量	用于播放任何音频	需要设置属性，默认单次，无淡入淡出，100%音量
todo:	music:	音乐播放通道	数量1	用于播放背景音乐	循环
todo:	vioce:	语音播放通道	配置	用于播放人物语音	单次
todo:	effect:	音效播放通道	数量1	用于播放场景音效	单次
*/

public class Audio
{
	private IJSRuntime _jsRuntime = null!;
	public string Name { get; set; } = "";
	public byte[]? AudioBytes { get; set; }
	public AudioSetting Setting;

	private AudioContext _context = default!;
	private GainNode _gain = default!;
	private AudioDestinationNode _destination = default!;
	private AudioBufferSourceNode _currentAudioBufferNode = default!;

	public Audio(IJSRuntime jsRuntime)
	{
		_jsRuntime = jsRuntime;
	}

	public async Task LoadAudioAsync()
	{
		if (AudioBytes is null)
			throw new Exception("Empty Audio Bytes");

		// 创建音频上下文
		_context = await AudioContext.CreateAsync(_jsRuntime);
		_destination = await _context.GetDestinationAsync();

		// 创建音频缓冲区
		AudioBuffer currentAudioBuffer = default!;
		await _context.DecodeAudioDataAsync(AudioBytes, (audioBuffer) => { currentAudioBuffer = audioBuffer; return Task.CompletedTask; });

		// 连接音频缓冲区和上下文
		_currentAudioBufferNode = await _context.CreateBufferSourceAsync();
		await _currentAudioBufferNode.SetBufferAsync(currentAudioBuffer);
		await _currentAudioBufferNode.ConnectAsync(_destination);

		// 设置属性
		await _currentAudioBufferNode.SetLoopAsync(Setting.Loop);

		// 获取音频持续时间
		// await currentAudioBuffer.GetDurationAsync();
	}

	public async Task StartAsync()
	{
		await _currentAudioBufferNode.StartAsync();
	}

	public async Task StopAsunc()
	{
		await _currentAudioBufferNode.StopAsync();
	}

	public async Task SetVolume(float volume)
	{
		_gain = await GainNode.CreateAsync(_jsRuntime, _context, new() { Gain = volume });
		await _gain.ConnectAsync(_destination);
	}
}