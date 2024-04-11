using System.Text.Json;
using KristofferStrube.Blazor.WebAudio;
using Microsoft.JSInterop;
using WebGal.API.Data;
using WebGal.Audio;
using WebGal.Global;
using FileInfo = WebGal.API.Data.FileInfo;

namespace WebGal.API;


/// <summary>
/// 音频处理接口，用于管理游戏音频
/// </summary>
public partial class Driver
{
	[JSInvokable]
	public static async Task<string> RegisterAudioContextAsync(string json)
	{
		var audioContext = JsonSerializer.Deserialize<AudioIdInfo>(json, JsonConfig.Options);
		Response respone = new()
		{
			Type = ResponseType.Success,
			Message = "",
		};

		if (_audioManager is not null)
		{
			if (_audioManager.AudioContexts.ContainsKey(audioContext.ContextID) == false)
				_audioManager.AudioContexts[audioContext.ContextID] = await AudioContext.CreateAsync(_audioManager.JSRuntime);
		}

		else
			respone = new()
			{
				Type = ResponseType.Fail,
				Message = "AudioManager not set OR Game not loading",
			};
		return JsonSerializer.Serialize(respone, JsonConfig.Options);
	}

	[JSInvokable]
	public static async Task<string> RegisterAudioNodeAsync(string json)
	{
		var audioInfo = JsonSerializer.Deserialize<AudioInfo>(json, JsonConfig.Options);
		Response respone = new()
		{
			Type = ResponseType.Success,
			Message = "",
		};

		if (_audioManager is not null)
		{
			if (_audioManager.AudioContexts.TryGetValue(audioInfo.ID.ContextID, out AudioContext? value))
			{
				_audioManager.AudioNodes[audioInfo.ID.NodeID] = audioInfo.Type switch
				{
					AudioNodeType.Simple => new AudioSimple(_audioManager.JSRuntime),
					AudioNodeType.Source => new AudioSource(_audioManager.JSRuntime),
					AudioNodeType.Speeker => new AudioSpeeker(_audioManager.JSRuntime),
					AudioNodeType.Gain => new AudioGain(_audioManager.JSRuntime),
					AudioNodeType.Multiplexer => new AudioMutiplexer(_audioManager.JSRuntime),
					AudioNodeType.Pan => throw new Exception("控制组件未完善: todo"),
					_ => throw new Exception("未标识的控件类型: todo"),
				};
				await _audioManager.AudioNodes[audioInfo.ID.NodeID].SetContextAsync(_audioManager.AudioContexts[audioInfo.ID.ContextID]);
			}
			else
			{
				respone = new()
				{
					Type = ResponseType.Fail,
					Message = $"Audiocontext {audioInfo.ID.ContextID} not registed",
				};
			}
		}
		else
			respone = new()
			{
				Type = ResponseType.Fail,
				Message = "AudioManager not set OR Game not loading",
			};

		return JsonSerializer.Serialize(respone, JsonConfig.Options);
	}

	public static bool CheckAudioContext(AudioIdInfo info)
	{
		if (_audioManager is null || _audioManager.AudioContexts.ContainsKey(info.ContextID) == false)
			return false;
		return true;
	}
}