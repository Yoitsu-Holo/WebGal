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
	public static async Task<string> SetAudioSimpleInfoAsync(string json)
	{
		var info = JsonSerializer.Deserialize<AudioSimpleInfo>(json, JsonConfig.Options);
		return JsonSerializer.Serialize(await SetAudioSimpleInfoAsync(info), JsonConfig.Options);
	}
	#endregion


	public static async Task<Response> SetAudioSimpleInfoAsync(AudioSimpleInfo info)
	{
		Response response;
		response = CheckInit(); if (response.Type != ResponseType.Success) return response;
		response = CheckAudioContext(info.ID); if (response.Type != ResponseType.Success) return response;


		IAudio audio = _audioManager!.AudioNodes[info.ID.NodeID];

		if (audio is AudioSimple audioSimple)
		{
			await audioSimple.SetAudioBufferAsync(_resourceManager!.GetAudio(info.AudioName));
			await audioSimple.StartAsync(info.Start);
			await audioSimple.SetLoopAsync(info.Loop);
		}
		else
		{
			response.Type = ResponseType.Fail;
			response.Message = $"AudioNode:{info.ID.NodeID} not AudioSimple";
			return response;
		}

		return response;
	}
}
