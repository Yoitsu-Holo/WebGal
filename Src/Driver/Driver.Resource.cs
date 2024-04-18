using System.Text.Json;
using Microsoft.JSInterop;
using WebGal.API.Data;
using WebGal.Global;
using FileInfo = WebGal.API.Data.FileInfo;

namespace WebGal.API;


/// <summary>
/// 文本管理接口：包括处理游戏中的文本显示、对话框管理、文本样式和格式化等功能。这个接口应该允许开发人员轻松地添加、编辑和显示游戏中的文本内容。
/// </summary>
public partial class Driver
{
	#region API
	// 获取文件，根据文件名自动分类
	[JSInvokable]
	public static async Task<string> PullFileAsync(string json)
	{
		var info = JsonSerializer.Deserialize<FileInfo>(json, JsonConfig.Options);
		return JsonSerializer.Serialize(await PullFileAsync(info), JsonConfig.Options);
	}

	[JSInvokable]
	public static string CheckFile(string json)
	{
		var info = JsonSerializer.Deserialize<FileInfo>(json, JsonConfig.Options);
		return JsonSerializer.Serialize(CheckFile(info), JsonConfig.Options);
	}

	[JSInvokable]
	public static string RemoveFile(string json)
	{
		var info = JsonSerializer.Deserialize<FileInfo>(json, JsonConfig.Options);
		return JsonSerializer.Serialize(RemoveFile(info), JsonConfig.Options);
	}
	#endregion

	public static async Task<Response> PullFileAsync(FileInfo info)
	{
		Response response = CheckInit();

		if (response.Type != ResponseType.Success) return response;

		try
		{
			switch (info.Type)
			{
				case FileType.Script:
					await _resourceManager!.PullScriptAsync(info.Name, info.URL);
					break;
				case FileType.Audio:
					await _resourceManager!.PullAudioAsync(info.Name, info.URL);
					break;
				case FileType.Bin:
					await _resourceManager!.PullBlobAsync(info.Name, info.URL);
					break;
				case FileType.Image:
					await _resourceManager!.PullImageAsync(info.Name, info.URL);
					break;
				case FileType.Font:
					await _resourceManager!.PullFontAsync(info.Name, info.URL);
					break;
				default:
					break;
			}
		}
		catch (Exception exception)
		{
			response.Type = ResponseType.Fail;
			response.Message = exception.Message;
		}

		return response;
	}


	public static Response CheckFile(FileInfo info)
	{
		Response response = CheckInit();

		if (response.Type != ResponseType.Success) return response;

		response.Type = ResponseType.Success;
		try
		{
			switch (info.Type)
			{
				case FileType.Script:
					_resourceManager!.GetScript(info.Name);
					break;
				case FileType.Audio:
					_resourceManager!.GetAudio(info.Name);
					break;
				case FileType.Bin:
					_resourceManager!.GetBlob(info.Name);
					break;
				case FileType.Image:
					_resourceManager!.GetImage(info.Name);
					break;
				case FileType.Font:
					_resourceManager!.GetFont(info.Name);
					break;
				default:
					break;
			}
			response.Message = $"File already exists: {info.Name}";
		}
		catch (Exception exception)
		{
			response.Type = ResponseType.Fail;
			response.Message = exception.Message;
		}

		return response;
	}


	public static Response RemoveFile(FileInfo info)
	{
		Response response = CheckInit();
		if (response.Type != ResponseType.Success) return response;


		switch (info.Type)
		{
			case FileType.Script:
				_resourceManager!.RemoveScript(info.Name);
				break;
			case FileType.Audio:
				_resourceManager!.RemoveAudio(info.Name);
				break;
			case FileType.Bin:
				_resourceManager!.RemoveBlob(info.Name);
				break;
			case FileType.Image:
				_resourceManager!.RemoveImage(info.Name);
				break;
			case FileType.Font:
				_resourceManager!.RemoveFont(info.Name);
				break;
			default:
				break;
		}

		return response;
	}
}