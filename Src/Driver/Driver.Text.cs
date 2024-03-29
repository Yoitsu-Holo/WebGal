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
	[JSInvokable]
	public static Task<string> RegisteTextBox(string json)
	{
		ResponseHeader respone = new();
		var textBox = JsonSerializer.Deserialize<TextBox>(json);


		return Task.FromResult(JsonSerializer.Serialize(respone));
	}


	[JSInvokable]
	public static Task<string> Say(string json)
	{
		ResponseHeader respone = new();



		return Task.FromResult(JsonSerializer.Serialize(respone));
	}
}