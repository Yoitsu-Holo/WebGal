using KristofferStrube.Blazor.WebAudio;
using Microsoft.JSInterop;

namespace WebGal.Audio;

public class AudioGain(IJSRuntime jsRuntime) : AudioBase(jsRuntime)
{
	private GainNode? _gain;

	public async Task SetGainASync(float gain)
	{
		if (_context is null)
			throw new Exception("Without any context");
		await (await _gain!.GetGainAsync()).SetValueAsync(gain);
	}

	// Interface
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

		await _gain!.ConnectAsync(target.GetSocketAsync(), wire.Src, wire.Dst);
	}

	public override AudioNode GetSocketAsync()
	{
		if (_context is null)
			throw new Exception("Without any context");
		return _gain!;
	}

	public override async Task SetContextAsync(AudioContext context)
	{
		await base.SetContextAsync(context);
		if (_context is null)
			throw new Exception("Without any context");
		_gain = await _context.CreateGainAsync();
	}

	public override ulong InputChannels() => 1;
	public override ulong OutputChannels() => 1;

	public override async Task DisposeAsync()
	{
		if (_gain is not null)
		{
			await _gain.DisconnectAsync();
			await _gain.DisposeAsync();
		}

		_gain = null;
	}
}