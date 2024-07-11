using System.Text.Json;
using Microsoft.JSInterop;
using WebGal.API.Data;
using WebGal.Audio;
using WebGal.Global;

namespace WebGal.API;

public partial class Driver
{
	#region API
	[JSInvokable]
	public static async Task<string> SetAudioMultiplexerInfoAsync(string json)
	{
		var info = JsonSerializer.Deserialize<AudioMutiplexerInfo>(json, JsonConfig.Options);
		return JsonSerializer.Serialize(await SetAudioMultiplexerInfoAsync(info), JsonConfig.Options);
	}
	#endregion

	#region Driect
	public static async Task<Response> SetAudioMultiplexerInfoAsync(AudioMutiplexerInfo info)
	{
		Response response;
		response = CheckInit(); if (response.Type != ResponseType.Success) return response;
		response = CheckAudioContext(info.ID); if (response.Type != ResponseType.Success) return response;

		IAudio audio = _audioManager!.AudioNodes[info.ID.NodeID];

		if (audio is AudioMutiplexer audioMultiplexer)
		{
			await audioMultiplexer.SetInputNumberAsync(info.InputChannels);
			await audioMultiplexer.SetOutputNumberAsync(info.OutputChannels);
			return new();
		}
		else
			return new($"AudioNode:{info.ID.NodeID} not AudioMutiplexer");
	}
	#endregion
}
