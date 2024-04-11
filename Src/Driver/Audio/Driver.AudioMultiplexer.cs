using System.Text.Json;
using Microsoft.JSInterop;
using WebGal.API.Data;
using WebGal.Audio;
using WebGal.Global;

namespace WebGal.API;

public partial class Driver
{
	[JSInvokable]
	public static async Task<string> SetAudioMultiplexerInfoAsync(string json)
	{
		Response respone = new();
		var info = JsonSerializer.Deserialize<AudioMutiplexerInfo>(json, JsonConfig.Options);

		if (_resourceManager is null || _audioManager is null)
		{
			respone.Type = ResponseType.Fail;
			respone.Message = "AudioManager not set OR Game not loading";
			return JsonSerializer.Serialize(respone, JsonConfig.Options);
		}

		if (CheckAudioContext(info.ID) == false)
		{
			respone.Type = ResponseType.Fail;
			respone.Message = $"AudioContext {info.ID.ContextID}:{info.ID.NodeID} not find";
			return JsonSerializer.Serialize(respone, JsonConfig.Options);
		}

		IAudio audio = _audioManager.AudioNodes[info.ID.NodeID];

		if (audio is AudioMutiplexer audioMultiplexer)
		{
			await audioMultiplexer.SetInputNumberAsync(info.InputChannels);
			await audioMultiplexer.SetOutputNumberAsync(info.OutputChannels);
		}
		else
		{
			respone.Type = ResponseType.Fail;
			respone.Message = $"AudioNode:{info.ID.NodeID} not AudioMutiplexer";
			return JsonSerializer.Serialize(respone, JsonConfig.Options);
		}

		respone.Type = ResponseType.Success;
		respone.Message = "";
		return JsonSerializer.Serialize(respone, JsonConfig.Options);
	}
}
