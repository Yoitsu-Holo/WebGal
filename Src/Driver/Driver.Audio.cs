using System.Text.Json;
using KristofferStrube.Blazor.WebAudio;
using Microsoft.JSInterop;
using WebGal.API.Data;
using WebGal.Audio;
using WebGal.Global;

namespace WebGal.API;


/// <summary>
/// 音频处理接口，用于管理游戏音频
/// </summary>
public partial class Driver
{
	#region API
	[JSInvokable]
	public static async Task<string> RegisterAudioContextAsync(string json)
	{
		var info = JsonSerializer.Deserialize<AudioIdInfo>(json, JsonConfig.Options);
		return JsonSerializer.Serialize(await RegisterAudioContextAsync(info), JsonConfig.Options);
	}

	[JSInvokable]
	public static async Task<string> RegisterAudioNodeAsync(string json)
	{
		var info = JsonSerializer.Deserialize<AudioNodeInfo>(json, JsonConfig.Options);
		return JsonSerializer.Serialize(await RegisterAudioNodeAsync(info), JsonConfig.Options);
	}

	[JSInvokable]
	public static async Task<string> ConnectAudioNode(string json)
	{
		var info = JsonSerializer.Deserialize<AudioWireInfo>(json, JsonConfig.Options);
		return JsonSerializer.Serialize(await ConnectAudioNode(info), JsonConfig.Options);
	}
	#endregion

	public static async Task<Response> RegisterAudioContextAsync(AudioIdInfo info)
	{
		Response response = CheckInit();
		if (response.Type != ResponseType.Success) return response;

		if (_audioManager!.AudioContexts.ContainsKey(info.ContextID) == false)
			_audioManager.AudioContexts[info.ContextID] = await AudioContext.CreateAsync(_audioManager.JSRuntime);

		return response;
	}

	public static async Task<Response> RegisterAudioNodeAsync(AudioNodeInfo info)
	{
		Response response = CheckInit();
		if (response.Type != ResponseType.Success) return response;


		if (_audioManager!.AudioContexts.TryGetValue(info.ID.ContextID, out AudioContext? context))
		{
			if (_audioManager.AudioNodes.TryGetValue(info.ID.NodeID, out IAudio? node))
			{
				_audioManager.AudioNodes.Remove(info.ID.NodeID);
				await node.DisposeAsync();
			}

			_audioManager.AudioNodes[info.ID.NodeID] = AudioNodeRegister.GetAudioNode(info.Type);
			await _audioManager.AudioNodes[info.ID.NodeID].SetContextAsync(context);
			return response;
		}
		else
			return new($"Audiocontext {info.ID.ContextID} not registed");
	}

	#region Driect
	public static async Task<Response> ConnectAudioNode(AudioWireInfo info)
	{
		Response response;
		response = CheckInit(); if (response.Type != ResponseType.Success) return response;
		response = CheckAudioNodeOutput(info.SrcID); if (response.Type != ResponseType.Success) return response;
		response = CheckAudioNodeInput(info.DstID); if (response.Type != ResponseType.Success) return response;

		IAudio outputNode = _audioManager!.AudioNodes[info.SrcID.NodeID];
		IAudio inputNode = _audioManager.AudioNodes[info.DstID.NodeID];
		AudioWire wire = new() { Src = info.SrcID.SocketID, Dst = info.DstID.SocketID, };

		await outputNode.ConnectToAsync(inputNode, wire);

		return response;
	}

	public static Response CheckAudioContext(AudioIdInfo info)
	{
		if (_audioManager is null)
			return new("AudioManager is NULL");
		if (!_audioManager.AudioContexts.ContainsKey(info.ContextID))
			return new($"AudioContext {info.ContextID} not find");
		return new();
	}

	public static Response CheckAudioNode(AudioIdInfo info)
	{
		Response response;
		response = CheckAudioContext(info); if (response.Type != ResponseType.Success) return response;
		if (!_audioManager!.AudioNodes.ContainsKey(info.NodeID))
			return new($"AudioContext {info.ContextID}:{info.NodeID} not find");
		return response;
	}

	public static Response CheckAudioNodeInput(AudioIdInfo info)
	{
		Response response;
		response = CheckAudioNode(info); if (response.Type != ResponseType.Success) return response;
		if (_audioManager!.AudioNodes[info.NodeID].InputChannels() <= info.SocketID)
			return new($"Error InputChannelID");
		return response;
	}

	public static Response CheckAudioNodeOutput(AudioIdInfo info)
	{
		Response response;
		response = CheckAudioNode(info); if (response.Type != ResponseType.Success) return response;
		if (_audioManager!.AudioNodes[info.NodeID].OutputChannels() <= info.SocketID)
			return new($"Error OutputChannelID");
		return response;
	}
	#endregion
}