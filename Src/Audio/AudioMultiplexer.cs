using KristofferStrube.Blazor.WebAudio;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace WebGal.Audio;

public class AudioMutiplexer : IAudioBaseNode
{
	private IJSRuntime _jsRuntime;
	private AudioContext? _context;

	private ChannelMergerNode? _merger;
	private ChannelSplitterNode? _splitter;

	private ulong _inputChannels = 6;
	private ulong _outputChannels = 6;

	public AudioMutiplexer(JSRuntime jsRuntime) => _jsRuntime = jsRuntime;

	public async Task SetInputNumber(ulong inputChannels = 6uL)
	{
		if (_context is null)
			throw new Exception("Without any context");

		_inputChannels = inputChannels;
		_merger = await ChannelMergerNode.CreateAsync(_jsRuntime, _context, new() { NumberOfInputs = _inputChannels });

		await _merger!.ConnectAsync(_splitter!);
	}

	public async Task SetOutputNumber(ulong outputChannels = 6uL)
	{
		if (_context is null)
			throw new Exception("Without any context");

		_outputChannels = outputChannels;
		_splitter = await ChannelSplitterNode.CreateAsync(_jsRuntime, _context, new() { NumberOfInputs = _outputChannels });

		await _merger!.ConnectAsync(_splitter!);
	}

	// Interface
	public async Task SetContextAsync(AudioContext context)
	{
		_context = context;
		// 默认设置 6 in 6 out
		_merger = await ChannelMergerNode.CreateAsync(_jsRuntime, _context);
		_splitter = await ChannelSplitterNode.CreateAsync(_jsRuntime, _context);
		// 内部连接
		await _merger.ConnectAsync(_splitter);
	}

	public async Task ConnectToAsync(IAudioBaseNode target, AudioWire wire)
	{
		if (_context is null)
			throw new Exception("Without any context");

		var inputChannels = target.InputChannels();
		var outputChannels = OutputChannels();

		if (wire.Input >= inputChannels)
			throw new Exception("input out of range");
		if (wire.Output >= outputChannels)
			throw new Exception("output out of range");

		await _splitter!.ConnectAsync(target.GetSocketAsync(), wire.Output, wire.Input);
	}

	public AudioNode GetSocketAsync()
	{
		if (_context is null)
			throw new Exception("Without any context");
		return _merger!;
	}

	public ulong InputChannels() => _inputChannels;
	public ulong OutputChannels() => _outputChannels;
}