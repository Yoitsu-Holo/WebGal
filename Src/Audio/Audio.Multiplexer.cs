using KristofferStrube.Blazor.WebAudio;
using Microsoft.JSInterop;
using WebGal.Global;

namespace WebGal.Audio;

public class AudioMutiplexer(IJSRuntime jsRuntime) : AudioBase(jsRuntime)
{
	private ChannelMergerNode? _merger;
	private ChannelSplitterNode? _splitter;

	private ulong _inputChannels = 6;
	private ulong _outputChannels = 6;

	public async Task SetInputNumberAsync(ulong inputChannels = 6uL)
	{
		if (_context is null) { Logger.LogInfo("未设置音频上下文", Global.LogLevel.Warning); return; }

		_inputChannels = inputChannels;
		_merger = await ChannelMergerNode.CreateAsync(_jsRuntime, _context, new() { NumberOfInputs = _inputChannels });

		await _merger!.ConnectAsync(_splitter!);
	}

	public async Task SetOutputNumberAsync(ulong outputChannels = 6uL)
	{
		if (_context is null) { Logger.LogInfo("未设置音频上下文", Global.LogLevel.Warning); return; }

		_outputChannels = outputChannels;
		_splitter = await ChannelSplitterNode.CreateAsync(_jsRuntime, _context, new() { NumberOfInputs = _outputChannels });

		await _merger!.ConnectAsync(_splitter!);
	}

	// Interface
	public override async Task SetContextAsync(AudioContext context)
	{
		await base.SetContextAsync(context);
		if (_context is null) { Logger.LogInfo("未设置音频上下文", Global.LogLevel.Warning); return; }

		// 默认设置 6 in 6 out
		_merger = await _context.CreateChannelMergerAsync(_inputChannels);
		_splitter = await _context.CreateChannelSplitterAsync(_inputChannels);
		// 内部连接
		await _merger.ConnectAsync(_splitter);
	}

	public override async Task ConnectToAsync(IAudio target, AudioWire wire)
	{
		if (_context is null) { Logger.LogInfo("未设置音频上下文", Global.LogLevel.Warning); return; }
		if (wire.Src >= OutputChannels()) { Logger.LogInfo("源接口超过限制", Global.LogLevel.Warning); return; }
		if (wire.Dst >= target.InputChannels()) { Logger.LogInfo("目标接口超过限制", Global.LogLevel.Warning); return; }

		await _splitter!.ConnectAsync(target.GetSocketAsync(), wire.Src, wire.Dst);
	}

	public override AudioNode GetSocketAsync()
	{
		if (_context is null) throw new Exception("未设置音频上下文");

		return _merger!;
	}

	public override ulong InputChannels() => _inputChannels;
	public override ulong OutputChannels() => _outputChannels;

	public override async ValueTask DisposeAsync()
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
		// _merger = null;
		// _splitter = null;

		GC.SuppressFinalize(this);
	}
}