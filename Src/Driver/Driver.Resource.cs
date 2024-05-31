using System.Resources;
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

	[JSInvokable]
	public static string GetScriptAsync(string json)
	{
		var info = JsonSerializer.Deserialize<FileInfo>(json, JsonConfig.Options);
		return JsonSerializer.Serialize(GetScriptAsync(info), JsonConfig.Options);
	}
	#endregion

	#region Driect
	public static async Task<Response> PullFileAsync(FileInfo info)
	{
		Response response = CheckInit();

		if (response.Type != ResponseType.Success) return response;
		if (_resourceManager!.CheckFile(info)) return response;

		try
		{
			await _resourceManager.PullFileAsnc(info);
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

		if (_resourceManager!.CheckFile(info) == false)
			response.Type = ResponseType.Fail;

		return response;
	}


	public static Response RemoveFile(FileInfo info)
	{
		Response response = CheckInit();
		if (response.Type != ResponseType.Success) return response;

		_resourceManager!.RemoveFile(info);
		return response;
	}

	public static Response GetScriptAsync(FileInfo info)
	{
		Response response = CheckInit();

		if (response.Type != ResponseType.Success) return response;

		if (info.Type != FileType.Script)
		{
			response.Type = ResponseType.Fail;
			response.Message = "Interpreter cannot fetch non-script type files";
			return response;
		}

		try
		{
			response.Message = _resourceManager!.GetScript(info.Name);
		}
		catch (Exception exception)
		{
			response.Type = ResponseType.Fail;
			response.Message = exception.Message;
		}

		return response;
	}
	#endregion
}