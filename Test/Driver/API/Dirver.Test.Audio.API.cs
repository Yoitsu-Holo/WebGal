using System.Text.Json;
using WebGal.API.Data;
using WebGal.Global;
using FileInfo = WebGal.API.Data.FileInfo;

namespace WebGal.API;

public partial class Driver
{
	public partial class Test
	{
		public static async Task<string> AudioTestAsync()
		{
			Response response = new();
			if (_audioManager is null || _resourceManager is null)
			{
				response.Type = ResponseType.Fail;
				response.Message = "AudioManager not set OR Game not loading";
				return JsonSerializer.Serialize(response, JsonConfig.Options);
			}

			//! 拉取文件
			{
				Console.WriteLine("Pull BackgroundMusic ...");
				FileInfo fileInfo = new()
				{
					Type = FileType.Audio,

					URL = "/pack/sound/bgm/bgm04_b.ogg",
					Name = "bgm04",
				};

				string result = await PullFileAsync(JsonSerializer.Serialize(fileInfo, JsonConfig.Options));
				if (JsonSerializer.Deserialize<Response>(result, JsonConfig.Options).Type != ResponseType.Success)
					return result;
			}

			//! 注册上下文
			{
				Console.WriteLine("Register AudioContext:0 ...");
				AudioIdInfo info = new() { ContextID = 0, };

				Response result = await RegisterAudioContextAsync(info);
				if (result.Type != ResponseType.Success)
					return result.Message;
			}

			//! 注册处理节点0 源节点
			{
				Console.WriteLine("Register AudioNode:0 ...");
				AudioNodeInfo info = new()
				{
					Request = RequestType.Set,
					ID = new() { ContextID = 0, NodeID = 0, },

					Type = "AudioSource",
				};

				Response result = await RegisterAudioNodeAsync(info);
				if (result.Type != ResponseType.Success)
					return result.Message;
			}

			//! 注册处理节点1 增益节点
			{
				Console.WriteLine("Register AudioNode:1 ...");
				AudioNodeInfo info = new()
				{
					Request = RequestType.Set,
					ID = new() { ContextID = 0, NodeID = 1, },

					Type = "AudioGain",
				};

				string result = await RegisterAudioNodeAsync(JsonSerializer.Serialize(info, JsonConfig.Options));
				if (JsonSerializer.Deserialize<Response>(result, JsonConfig.Options).Type != ResponseType.Success)
					return result;
			}

			//! 注册处理节点2 输出节点
			{
				Console.WriteLine("Register AudioNode:2 ...");
				AudioNodeInfo info = new()
				{
					Request = RequestType.Set,
					ID = new() { ContextID = 0, NodeID = 2, },

					Type = "AudioSpeeker",
				};

				string result = await RegisterAudioNodeAsync(JsonSerializer.Serialize(info, JsonConfig.Options));
				if (JsonSerializer.Deserialize<Response>(result, JsonConfig.Options).Type != ResponseType.Success)
					return result;
			}

			//! 设置节点0
			{
				Console.WriteLine("Set Source AudioNode:0 ...");
				AudioSourceInfo info = new()
				{
					ID = new() { ContextID = 0, NodeID = 0, },
					AudioName = "bgm04",
					Start = true,
					Loop = true,
				};

				string result = await SetAudioSourceInfoAsync(JsonSerializer.Serialize(info, JsonConfig.Options));
				if (JsonSerializer.Deserialize<Response>(result, JsonConfig.Options).Type != ResponseType.Success)
					return result;
			}

			//! 设置节点1
			{
				Console.WriteLine("Set Gain AudioNode:1 ...");
				AudioGainInfo info = new()
				{
					ID = new() { ContextID = 0, NodeID = 1, },
					Gain = 800,
				};

				string result = await SetAudioGainInfoAsync(JsonSerializer.Serialize(info, JsonConfig.Options));
				if (JsonSerializer.Deserialize<Response>(result, JsonConfig.Options).Type != ResponseType.Success)
					return result;
			}

			//! 连接节点 0 -> 1
			{
				Console.WriteLine("Coneect AudioNode: 0 -> 1 ...");
				AudioWireInfo info = new()
				{
					Request = RequestType.Set,
					SrcID = new() { ContextID = 0, NodeID = 0, SocketID = 0, },
					DstID = new() { ContextID = 0, NodeID = 1, SocketID = 0, },
				};

				string result = await ConnectAudioNode(JsonSerializer.Serialize(info, JsonConfig.Options));
				if (JsonSerializer.Deserialize<Response>(result, JsonConfig.Options).Type != ResponseType.Success)
					return result;
			}

			//! 连接节点 1 -> 2
			{
				Console.WriteLine("Coneect AudioNode: 1 -> 2 ...");
				AudioWireInfo info = new()
				{
					Request = RequestType.Set,
					SrcID = new() { ContextID = 0, NodeID = 1, SocketID = 0, },
					// SrcID = new() { ContextID = 0, NodeID = 0, SocketID = 0, },
					DstID = new() { ContextID = 0, NodeID = 2, SocketID = 0, },
				};

				string result = await ConnectAudioNode(JsonSerializer.Serialize(info, JsonConfig.Options));
				if (JsonSerializer.Deserialize<Response>(result, JsonConfig.Options).Type != ResponseType.Success)
					return result;
			}

			response.Type = ResponseType.Success;
			response.Message = "Hello WebGal.Audio";
			return JsonSerializer.Serialize(response, JsonConfig.Options);
		}
	}
}