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
	public static async Task<string> SetAudioGainInfoAsync(string json)
	{
		var info = JsonSerializer.Deserialize<AudioGainInfo>(json, JsonConfig.Options);
		return JsonSerializer.Serialize(await SetAudioGainInfoAsync(info), JsonConfig.Options);
	}
	#endregion

	public static async Task<Response> SetAudioGainInfoAsync(AudioGainInfo info)
	{
		Response response;
		response = CheckInit(); if (response.Type != ResponseType.Success) return response;
		response = CheckAudioContext(info.ID); if (response.Type != ResponseType.Success) return response;

		IAudio audio = _audioManager!.AudioNodes[info.ID.NodeID];

		if (audio is AudioGain audioGain)
		{
			float value = Math.Min(1000, Math.Max(0, info.Gain));
			await audioGain.SetGainASync(value / 1000.0f);
		}
		else
		{
			response.Type = ResponseType.Fail;
			response.Message = $"AudioNode:{info.ID.NodeID} not AudioGain";
			return response;
		}

		return response;
	}
}
