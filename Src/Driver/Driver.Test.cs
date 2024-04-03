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
	[JSInvokable]
	public static async Task<string> Test(string json)
	{
		// DotNet.invokeMethodAsync('WebGal', 'Test', '');
		/*
		DotNet.invokeMethodAsync('WebGal', 'Test', '')
			.then(result => {
				console.log(result);
			});
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
				Type = FileType.Image,
				URL = "/Image/bg010a.png",
				Name = "bg010a",
			};

			string result = await PullFileAsync(JsonSerializer.Serialize(fileInfo));
			if (JsonSerializer.Deserialize<ResponseHeader>(result).Type != ResponseType.Success)
				return result;
		}

		{
			Console.WriteLine("Pull Font ...");
			FileInfo fileInfo = new()
			{
				Request = new()
				{
					Type = RequestType.Set,
				},
				Type = FileType.Font,
				URL = "/simhei.ttf",
				Name = "simhei",
			};

			string result = await PullFileAsync(JsonSerializer.Serialize(fileInfo));
			if (JsonSerializer.Deserialize<ResponseHeader>(result).Type != ResponseType.Success)
				return result;
		}

		//! 注册Layout
		{
			Console.WriteLine("Register Layout:0 ...");
			LayoutInfo layoutInfo = new()
			{
				Request = new()
				{
					Type = RequestType.Set,
				},
				LayoutId = 0,
			};

			string result = RegisterLayout(JsonSerializer.Serialize(layoutInfo));
			if (JsonSerializer.Deserialize<ResponseHeader>(result).Type != ResponseType.Success)
				return result;
		}
		//! 注册背景图片 layer0
		{
			Console.WriteLine("Register Layer:0 ...");
			LayerBox imageBox = new()
			{
				Request = new()
				{
					Type = RequestType.Set,
				},
				Attribute = new()
				{
					Type = LayerType.ImageBox,
					Position = new(0, 0),
					Size = new(1280, 720),

					LayoutID = 0,
					LayerID = 0,
				},
			};

			string result = RegisterLayer(JsonSerializer.Serialize(imageBox));
			if (JsonSerializer.Deserialize<ResponseHeader>(result).Type != ResponseType.Success)
				return result;
		}

		//! 注册文本底色框 layer2
		{
			Console.WriteLine("Register Layer:2 ...");

			LayerBox colorBox = new()
			{
				Request = new()
				{
					Type = RequestType.Set,
				},
				Attribute = new()
				{
					Type = LayerType.ColorBox,
					Position = new(20, 530),
					Size = new(1240, 170),

					LayoutID = 0,
					LayerID = 2,
				}
			};

			string result = RegisterLayer(JsonSerializer.Serialize(colorBox));
			if (JsonSerializer.Deserialize<ResponseHeader>(result).Type != ResponseType.Success)
				return result;
		}

		//! 注册文本框 layer3
		{
			Console.WriteLine("Register Layer:3 ...");

			LayerBox textBox = new()
			{
				Request = new()
				{
					Type = RequestType.Set,
				},
				Attribute = new()
				{
					Type = LayerType.TextBox,
					Position = new(40, 550),
					Size = new(1200, 150),

					LayoutID = 0,
					LayerID = 3,
				}
			};

			string result = RegisterLayer(JsonSerializer.Serialize(textBox));
			if (JsonSerializer.Deserialize<ResponseHeader>(result).Type != ResponseType.Success)
				return result;
		}


		//! 设置图片
		{
			Console.WriteLine("Set Image Layer:0 ...");

			ImageBoxInfo image = new()
			{
				LayoutID = 0,
				LayerID = 0,
				ImageName = "bg010a",
			};
			string result = SetImageBoxInfo(JsonSerializer.Serialize(image));
			if (JsonSerializer.Deserialize<ResponseHeader>(result).Type != ResponseType.Success)
				return result;
		}

		//! 设置图片
		{
			Console.WriteLine("Set Color Layer:2 ...");

			ColorBoxInfo image = new()
			{
				LayoutID = 0,
				LayerID = 2,
				R = 112,
				G = 146,
				B = 190,
				A = 160,
			};
			string result = SetColorBoxInfo(JsonSerializer.Serialize(image));
			if (JsonSerializer.Deserialize<ResponseHeader>(result).Type != ResponseType.Success)
				return result;
		}


		//! 设置文字
		{
			Console.WriteLine("Set Text Layer:3 ...");
			TextBoxInfo text = new()
			{
				LayoutID = 0,
				LayerID = 3,
				Text = "Hello Wrold, 你好世界",
				Font = "simhei",
				FontSize = 30,
			};
			string result = SetTextBoxInfo(JsonSerializer.Serialize(text));
			if (JsonSerializer.Deserialize<ResponseHeader>(result).Type != ResponseType.Success)
				return result;
		}

		ResponseHeader response = new()
		{
			Type = ResponseType.Success,
			Message = "Hello WebGal"
		};

		Console.WriteLine(JsonSerializer.Serialize(response, JsonConfig.Options));
		return JsonSerializer.Serialize(response);
	}
}