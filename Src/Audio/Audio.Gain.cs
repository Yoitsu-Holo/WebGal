using KristofferStrube.Blazor.WebAudio;
using Microsoft.JSInterop;

namespace WebGal.Audio;

public class AudioGain(IJSRuntime jsRumtine) : IAudio
{
	private readonly IJSRuntime _jsRuntime = jsRumtine;
	private AudioContext? _context;

	private GainNode? _gain;

	public async Task SetGainASync(float gain)
	{
		if (_context is null)
			throw new Exception("Without any context");
		await (await _gain!.GetGainAsync()).SetValueAsync(gain);
	}

	// Interface
	public async Task ConnectToAsync(IAudio target, AudioWire wire)
	{
		if (_context is null)
			throw new Exception("Without any context");

		var inputChannels = target.InputChannels();
		var outputChannels = OutputChannels();

		if (wire.Input >= inputChannels)
			throw new Exception("input out of range");
		if (wire.Output >= outputChannels)
			throw new Exception("output out of range");

		await _gain!.ConnectAsync(target.GetSocketAsync(), wire.Output, wire.Input);
	}

	public AudioNode GetSocketAsync()
	{
		if (_context is null)
			throw new Exception("Without any context");
		return _gain!;
	}

	public async Task SetContextAsync(AudioContext context)
	{
		_context = context;
		_gain = await GainNode.CreateAsync(_jsRuntime, _context, new() { Gain = 1.0f });
	}

	public ulong InputChannels() => 1;
	public ulong OutputChannels() => 1;
}