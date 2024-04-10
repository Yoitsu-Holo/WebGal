using System.Text.Json;
using Microsoft.JSInterop;
using WebGal.API.Data;
using WebGal.Audio;

namespace WebGal.API;

public partial class Driver
{
	[JSInvokable]
	public static async Task<string> SetAudioSimpleInfoAsync(string json)
	{
		ResponseHeader respone = new();
		var info = JsonSerializer.Deserialize<AudioSimpleInfo>(json);

		if (_resourceManager is null || _audioManager is null)
		{
			respone.Type = ResponseType.Fail;
			respone.Message = "AudioManager not set OR Game not loading";
			return JsonSerializer.Serialize(respone);
		}

		if (CheckAudioContext(info.ID) == false)
		{
			respone.Type = ResponseType.Fail;
			respone.Message = $"AudioContext {info.ID.ContextID}:{info.ID.NodeID} not find";
			return JsonSerializer.Serialize(respone);
		}

		IAudio audio = _audioManager.AudioNodes[info.ID.NodeID];

		if (audio is AudioSimple audioSimple)
		{
			await audioSimple.SetAudioAsync(_resourceManager.GetAudio(info.AudioName));
			if (info.Start)
				await audioSimple.StartAsync();
		}
		else
		{
			respone.Type = ResponseType.Fail;
			respone.Message = $"AudioNode:{info.ID.NodeID} not AudioSimpleNode";
			return JsonSerializer.Serialize(respone);
		}

		respone.Type = ResponseType.Success;
		respone.Message = "";
		return JsonSerializer.Serialize(respone);
	}
}
