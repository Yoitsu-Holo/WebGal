using KristofferStrube.Blazor.WebAudio;
using Microsoft.JSInterop;

namespace WebGal.Audio;

public class AudioMutiplexer(IJSRuntime jsRuntime) : AudioBase(jsRuntime)
{
	private ChannelMergerNode? _merger;
	private ChannelSplitterNode? _splitter;

	private ulong _inputChannels = 6;
	private ulong _outputChannels = 6;

	public async Task SetInputNumberAsync(ulong inputChannels = 6uL)
	{
		if (_context is null)
			throw new Exception("Without any context");

		_inputChannels = inputChannels;
		_merger = await ChannelMergerNode.CreateAsync(_jsRuntime, _context, new() { NumberOfInputs = _inputChannels });

		await _merger!.ConnectAsync(_splitter!);
	}

	public async Task SetOutputNumberAsync(ulong outputChannels = 6uL)
	{
		if (_context is null)
			throw new Exception("Without any context");

		_outputChannels = outputChannels;
		_splitter = await ChannelSplitterNode.CreateAsync(_jsRuntime, _context, new() { NumberOfInputs = _outputChannels });

		await _merger!.ConnectAsync(_splitter!);
	}

	// Interface
	public override async Task SetContextAsync(AudioContext context)
	{
		await base.SetContextAsync(context);
		if (_context is null)
			throw new Exception("Without any context");
		// 默认设置 6 in 6 out
		_merger = await _context.CreateChannelMergerAsync(_inputChannels);
		_splitter = await _context.CreateChannelSplitterAsync(_inputChannels);
		// 内部连接
		await _merger.ConnectAsync(_splitter);
	}

	public override async Task ConnectToAsync(IAudio target, AudioWire wire)
	{
		if (_context is null)
			throw new Exception("Without any context");

		var inputChannels = target.InputChannels();
		var outputChannels = OutputChannels();

		if (wire.Dst >= inputChannels)
			throw new Exception("input out of range");
		if (wire.Src >= outputChannels)
			throw new Exception("output out of range");

		await _splitter!.ConnectAsync(target.GetSocketAsync(), wire.Src, wire.Dst);
	}

	public override AudioNode GetSocketAsync()
	{
		if (_context is null)
			throw new Exception("Without any context");
		return _merger!;
	}

	public override ulong InputChannels() => _inputChannels;
	public override ulong OutputChannels() => _outputChannels;

	public override async Task DisposeAsync()
	{
		if (_merger is not null)
		{
			await _merger.DisconnectAsync();
			await _merger.DisposeAsync();
		}

		if (_splitter is not null)
		{
			await _splitter.DisconnectAsync();
			await _splitter.DisposeAsync();
		}
		_merger = null;
		_splitter = null;
	}
}