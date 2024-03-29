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
	public static Task<string> Test()
	{
		// 注册Layout


		// 注册背景图片 layer0
		{
			ImageBox imageBox = new()
			{
				Request = new()
				{
					Type = RequestType.Set,
				},
				Attribute = new()
				{
					Type = LayerType.TextBox,
					Position = new(0, 0),
					Size = new(1280, 720),

					LayerID = 0,
				},
				Name = "MainBgBox",
				Offset = new(0, 0),
			};

			var task = RegisteTextBox(JsonSerializer.Serialize(imageBox, JsonConfig.Options));
			task.Wait();
			if (JsonSerializer.Deserialize<ResponseHeader>(task.Result).Type != ResponseType.Success)
				return Task.FromResult(task.Result);
		}

		// 注册文本框 layer3
		{
			TextBox textbox = new()
			{
				Request = new()
				{
					Type = RequestType.Set,
				},
				Attribute = new()
				{
					Type = LayerType.TextBox,
					Position = new(20, 550),
					Size = new(1240, 150),

					LayerID = 3,
				},
				FontSize = 30,
				Font = "agaveMono",
				Name = "MainTextBox"
			};

			var task = RegisteTextBox(JsonSerializer.Serialize(textbox, JsonConfig.Options));
			task.Wait();
			if (JsonSerializer.Deserialize<ResponseHeader>(task.Result).Type != ResponseType.Success)
				return Task.FromResult(task.Result);
		}

		ResponseHeader respone = new()
		{
			Type = ResponseType.Success,
			Message = "",
		};
		return Task.FromResult(JsonSerializer.Serialize(respone, JsonConfig.Options));
	}
}