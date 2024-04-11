using System.Text.Json;
using Microsoft.JSInterop;
using WebGal.API.Data;
using WebGal.Audio;
using WebGal.Global;

namespace WebGal.API;

public partial class Driver
{
	[JSInvokable]
	public static async Task<string> SetAudioSourceInfoAsync(string json)
	{
		Response respone = new();
		var info = JsonSerializer.Deserialize<AudioSourceInfo>(json, JsonConfig.Options);

		var (flag, ret) = CheckInit();
		if (flag == false) return ret;

		if (CheckAudioContext(info.ID) == false)
		{
			respone.Type = ResponseType.Fail;
			respone.Message = $"AudioContext {info.ID.ContextID}:{info.ID.NodeID} not find";
			return JsonSerializer.Serialize(respone, JsonConfig.Options);
		}

		IAudio audio = _audioManager!.AudioNodes[info.ID.NodeID];

		if (audio is AudioSource audioSource)
		{
			await audioSource.SetAudioBufferAsync(_resourceManager!.GetAudio(info.AudioName));
			await audioSource.StartAsync(info.Start);
			await audioSource.SetLoopAsync(info.Loop);
		}
		else
		{
			respone.Type = ResponseType.Fail;
			respone.Message = $"AudioNode:{info.ID.NodeID} not AudioSource";
			return JsonSerializer.Serialize(respone, JsonConfig.Options);
		}

		return JsonSerializer.Serialize(respone, JsonConfig.Options);
	}
}
