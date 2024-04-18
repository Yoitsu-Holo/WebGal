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
	public static async Task<string> SetAudioSourceInfoAsync(string json)
	{
		var info = JsonSerializer.Deserialize<AudioSourceInfo>(json, JsonConfig.Options);
		return JsonSerializer.Serialize(await SetAudioSourceInfoAsync(info), JsonConfig.Options);
	}
	#endregion


	public static async Task<Response> SetAudioSourceInfoAsync(AudioSourceInfo info)
	{
		Response response;
		response = CheckInit(); if (response.Type != ResponseType.Success) return response;
		response = CheckAudioContext(info.ID); if (response.Type != ResponseType.Success) return response;


		IAudio audio = _audioManager!.AudioNodes[info.ID.NodeID];

		if (audio is AudioSource audioSource)
		{
			await audioSource.SetAudioBufferAsync(_resourceManager!.GetAudio(info.AudioName));
			await audioSource.StartAsync(info.Start);
			await audioSource.SetLoopAsync(info.Loop);
		}
		else
		{
			response.Type = ResponseType.Fail;
			response.Message = $"AudioNode:{info.ID.NodeID} not AudioSource";
			return response;
		}

		return response;
	}
}
