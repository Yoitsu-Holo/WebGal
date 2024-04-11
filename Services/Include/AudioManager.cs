using KristofferStrube.Blazor.WebAudio;
using Microsoft.JSInterop;
using WebGal.Audio;

namespace WebGal.Services.Include;


/// <summary>
/// AudioManager 接收来自驱动层的请求，根据需要播放相应的音频。
/// </summary>
public class AudioManager(IJSRuntime jSRuntime)
{
	public readonly IJSRuntime JSRuntime = jSRuntime;
	public Dictionary<int, IAudio> AudioNodes = [];

	public Dictionary<int, AudioContext> AudioContexts = [];

	public async Task Clear()
	{
		foreach (var (_, audioContext) in AudioContexts)
		{
			await audioContext.CloseAsync();
			await audioContext.DisposeAsync();
		}

		AudioNodes.Clear();
		AudioContexts.Clear();
	}

	~AudioManager()
	{
		Clear().GetAwaiter().GetResult();
	}
}