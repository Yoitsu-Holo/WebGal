using KristofferStrube.Blazor.WebAudio;
using Microsoft.JSInterop;
using WebGal.Global;

namespace WebGal.Audio;

public class AudioBase(IJSRuntime jsRuntime) : IAudio
{
	protected readonly IJSRuntime _jsRuntime = jsRuntime;
	protected AudioContext? _context;

	// Interface
	public virtual Task ConnectToAsync(IAudio target, AudioWire wire) => throw new NotImplementedException();
	public virtual AudioNode GetSocketAsync() => throw new NotImplementedException();
	public virtual ulong InputChannels() => 0;
	public virtual ulong OutputChannels() => 0;
	public virtual async Task SetContextAsync(AudioContext context)
	{
		await Task.Run(() => { });
		_context = context;
	}

	public virtual async ValueTask DisposeAsync()
	{
		await Task.Run(() => { Logger.LogInfo("该音频节点不包含任何内容", Global.LogLevel.Info); });
		GC.SuppressFinalize(this);
	}
}