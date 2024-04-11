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
	// 获取文件，根据文件名自动分类
	[JSInvokable]
	public static async Task<string> PullFileAsync(string json)
	{
		Response respone = new();
		var fileInfo = JsonSerializer.Deserialize<FileInfo>(json, JsonConfig.Options);

		var (flag, ret) = CheckInit();
		if (flag == false) return ret;

		try
		{
			switch (fileInfo.Type)
			{
				case FileType.Script:
					await _resourceManager!.PullScriptAsync(fileInfo.Name, fileInfo.URL);
					break;
				case FileType.Audio:
					await _resourceManager!.PullAudioAsync(fileInfo.Name, fileInfo.URL);
					break;
				case FileType.Bin:
					await _resourceManager!.PullBlobAsync(fileInfo.Name, fileInfo.URL);
					break;
				case FileType.Image:
					await _resourceManager!.PullImageAsync(fileInfo.Name, fileInfo.URL);
					break;
				case FileType.Font:
					await _resourceManager!.PullFontAsync(fileInfo.Name, fileInfo.URL);
					break;
				default:
					break;
			}
		}
		catch (Exception exception)
		{
			respone.Type = ResponseType.Fail;
			respone.Message = exception.Message;
		}

		return JsonSerializer.Serialize(respone, JsonConfig.Options);
	}


	[JSInvokable]
	public static string CheckFile(string json)
	{
		Response respone = new();
		var fileInfo = JsonSerializer.Deserialize<FileInfo>(json, JsonConfig.Options);

		var (flag, ret) = CheckInit();
		if (flag == false) return ret;

		respone.Type = ResponseType.Success;
		try
		{
			switch (fileInfo.Type)
			{
				case FileType.Script:
					_resourceManager!.GetScript(fileInfo.Name);
					break;
				case FileType.Audio:
					_resourceManager!.GetAudio(fileInfo.Name);
					break;
				case FileType.Bin:
					_resourceManager!.GetBlob(fileInfo.Name);
					break;
				case FileType.Image:
					_resourceManager!.GetImage(fileInfo.Name);
					break;
				case FileType.Font:
					_resourceManager!.GetFont(fileInfo.Name);
					break;
				default:
					break;
			}
			respone.Message = $"File already exists: {fileInfo.Name}";
		}
		catch (Exception exception)
		{ respone.Message = exception.Message; }

		return JsonSerializer.Serialize(respone, JsonConfig.Options);
	}


	[JSInvokable]
	public static string RemoveFile(string json)
	{
		Response respone = new();
		var fileInfo = JsonSerializer.Deserialize<FileInfo>(json, JsonConfig.Options);

		var (flag, ret) = CheckInit();
		if (flag == false) return ret;

		switch (fileInfo.Type)
		{
			case FileType.Script:
				_resourceManager!.RemoveScript(fileInfo.Name);
				break;
			case FileType.Audio:
				_resourceManager!.RemoveAudio(fileInfo.Name);
				break;
			case FileType.Bin:
				_resourceManager!.RemoveBlob(fileInfo.Name);
				break;
			case FileType.Image:
				_resourceManager!.RemoveImage(fileInfo.Name);
				break;
			case FileType.Font:
				_resourceManager!.RemoveFont(fileInfo.Name);
				break;
			default:
				break;
		}

		return JsonSerializer.Serialize(respone, JsonConfig.Options);
	}
}