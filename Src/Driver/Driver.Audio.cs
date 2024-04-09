using System.Text.Json;
using Microsoft.JSInterop;
using WebGal.API.Data;
using WebGal.Audio;
using FileInfo = WebGal.API.Data.FileInfo;

namespace WebGal.API;


/// <summary>
/// 音频处理接口，用于管理游戏音频
/// </summary>
public partial class Driver
{
	[JSInvokable]
	public static async Task<string> AudioTestAsync()
	{
		/*
		DotNet.invokeMethodAsync('WebGal', 'AudioTestAsync', '')
			.then(result => {console.log(result);});
		*/
		//! 拉取文件
		{
			Console.WriteLine("Pull background image ...");
			FileInfo fileInfo = new()
			{
				Request = new()
				{
					Type = RequestType.Set,
				},
				Type = FileType.Audio,
				URL = "/pack/sound/bgm/bgm04_b.ogg",
				Name = "bgm04",
			};

			string result = await PullFileAsync(JsonSerializer.Serialize(fileInfo));
			if (JsonSerializer.Deserialize<ResponseHeader>(result).Type != ResponseType.Success)
				return result;
		}

		{
			// _audioTest = new(jsRuntime);
			// _audioGain = new(jsRuntime);
			// _audioSource = new(jsRuntime);
			// _audioSpeeker = new(jsRuntime);
			_audioManager!.AudioNodes[0] = new AudioGain(_audioManager.JSRuntime);
		}

		ResponseHeader response = new()
		{
			Type = ResponseType.Success,
			Message = "Hello WebGal.Audio"
		};

		return JsonSerializer.Serialize(response);
	}
}