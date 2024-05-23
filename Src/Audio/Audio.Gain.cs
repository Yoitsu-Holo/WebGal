using KristofferStrube.Blazor.WebAudio;
using Microsoft.JSInterop;
using WebGal.Global;

namespace WebGal.Audio;

public class AudioGain(IJSRuntime jsRuntime) : AudioBase(jsRuntime)
{
	private GainNode? _gain;

	public async Task SetGainASync(float gain)
	{
		if (_context is null) { Logger.LogInfo("未设置音频上下文", Global.LogLevel.Warning); return; }

		await (await _gain!.GetGainAsync()).SetValueAsync(gain);
	}

	// Interface
	public override async Task ConnectToAsync(IAudio target, AudioWire wire)
	{
		if (_context is null) { Logger.LogInfo("未设置音频上下文", Global.LogLevel.Warning); return; }
		if (wire.Src >= OutputChannels()) { Logger.LogInfo("源接口超过限制", Global.LogLevel.Warning); return; }
		if (wire.Dst >= target.InputChannels()) { Logger.LogInfo("目标接口超过限制", Global.LogLevel.Warning); return; }

		await _gain!.ConnectAsync(target.GetSocketAsync(), wire.Src, wire.Dst);
	}

	public override AudioNode GetSocketAsync()
	{
		if (_context is null) throw new Exception("未设置音频上下文");

		return _gain!;
	}

	public override async Task SetContextAsync(AudioContext context)
	{
		await base.SetContextAsync(context);
		if (_context is null) { Logger.LogInfo("未设置音频上下文", Global.LogLevel.Warning); return; }

		_gain = await _context.CreateGainAsync();
	}

	public override ulong InputChannels() => 1;
	public override ulong OutputChannels() => 1;

	public override async ValueTask DisposeAsync()
	{
		if (_gain is not null)
		{
			await _gain.DisconnectAsync();
			await _gain.DisposeAsync();
		}

		GC.SuppressFinalize(this);
	}
}