using System.Text.Json;
using Microsoft.JSInterop;
using WebGal.API;
using WebGal.Global;

namespace WebGal.Driver;


/// <summary>
/// 文本管理接口：包括处理游戏中的文本显示、对话框管理、文本样式和格式化等功能。这个接口应该允许开发人员轻松地添加、编辑和显示游戏中的文本内容。
/// </summary>
public partial class Driver
{
	// 获取文件，根据文件名自动分类
	[JSInvokable]
	public static Task<string> PullFile(string json)
	{
		ResponseHeader respone = new();
		var textBox = JsonSerializer.Deserialize<TextBox>(json);


		return Task.FromResult(JsonSerializer.Serialize(respone));
	}



	[JSInvokable]
	public static Task<string> CheckFile(string json)
	{
		ResponseHeader respone = new();
		var textBox = JsonSerializer.Deserialize<TextBox>(json);


		return Task.FromResult(JsonSerializer.Serialize(respone));
	}



	[JSInvokable]
	public static Task<string> RemoveFile(string json)
	{
		ResponseHeader respone = new();
		var textBox = JsonSerializer.Deserialize<TextBox>(json);


		return Task.FromResult(JsonSerializer.Serialize(respone));
	}
}