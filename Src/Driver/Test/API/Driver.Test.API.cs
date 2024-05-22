using System.Text.Json;
using WebGal.API.Data;
using WebGal.Global;
using WebGal.Handler.Event;
using FileInfo = WebGal.API.Data.FileInfo;

namespace WebGal.API;

public partial class Driver
{
	public partial class Test
	{
		public static async Task<string> TestAsync()
		{
			//! 拉取文件
			{
				Console.WriteLine("Pull background image ...");
				FileInfo fileInfo = new()
				{
					Type = FileType.Image,

					URL = "/Image/bg010a.png",
					Name = "bg010a",
				};

				string result = await PullFileAsync(JsonSerializer.Serialize(fileInfo, JsonConfig.Options));
				if (JsonSerializer.Deserialize<Response>(result, JsonConfig.Options).Type != ResponseType.Success)
					return result;
			}

			{
				Console.WriteLine("Pull Menu image ...");
				FileInfo fileInfo = new()
				{
					Type = FileType.Image,

					URL = "/Image/title01_chip.png",
					Name = "title",
				};

				string result = await PullFileAsync(JsonSerializer.Serialize(fileInfo, JsonConfig.Options));
				if (JsonSerializer.Deserialize<Response>(result, JsonConfig.Options).Type != ResponseType.Success)
					return result;
			}

			{
				Console.WriteLine("Pull Font ...");
				FileInfo fileInfo = new()
				{
					Type = FileType.Font,

					URL = "/simhei.ttf",
					Name = "simhei",
				};

				string result = await PullFileAsync(JsonSerializer.Serialize(fileInfo, JsonConfig.Options));
				if (JsonSerializer.Deserialize<Response>(result, JsonConfig.Options).Type != ResponseType.Success)
					return result;
			}

			//! 注册Layout
			{
				Console.WriteLine("Register Layout:10 ...");
				LayerIdInfo layoutInfo = new()
				{
					Request = RequestType.Set,
					LayoutID = _gameLayout,
				};

				string result = RegisterLayout(JsonSerializer.Serialize(layoutInfo, JsonConfig.Options));
				if (JsonSerializer.Deserialize<Response>(result, JsonConfig.Options).Type != ResponseType.Success)
					return result;
			}

			//! 注册背景图片 layer0
			{
				Console.WriteLine("Register Layer:0 ...");
				LayerBox imageBox = new()
				{
					Request = RequestType.Set,
					Attribute = new()
					{
						Type = "WidgetImageBox",
						Position = new(0, 0),
						Size = new(1280, 720),
						ID = new() { LayoutID = _gameLayout, LayerID = 0, },
					},
				};

				string result = RegisterLayer(JsonSerializer.Serialize(imageBox, JsonConfig.Options));
				if (JsonSerializer.Deserialize<Response>(result, JsonConfig.Options).Type != ResponseType.Success)
					return result;
			}

			//! 注册文本底色框 layer2
			{
				Console.WriteLine("Register Layer:2 ...");

				LayerBox colorBox = new()
				{
					Request = RequestType.Set,
					Attribute = new()
					{
						Type = "WidgetColorBox",
						Position = new(20, 530),
						Size = new(1240, 170),
						ID = new() { LayoutID = _gameLayout, LayerID = 2, },
					}
				};

				string result = RegisterLayer(JsonSerializer.Serialize(colorBox, JsonConfig.Options));
				if (JsonSerializer.Deserialize<Response>(result, JsonConfig.Options).Type != ResponseType.Success)
					return result;
			}

			//! 注册文本框 layer3
			{
				Console.WriteLine("Register Layer:3 ...");

				LayerBox textBox = new()
				{
					Request = RequestType.Set,
					Attribute = new()
					{
						Type = "WidgetTextBox",
						Position = new(40, 550),
						Size = new(1200, 150),
						ID = new() { LayoutID = _gameLayout, LayerID = 3, },
					}
				};

				string result = RegisterLayer(JsonSerializer.Serialize(textBox, JsonConfig.Options));
				if (JsonSerializer.Deserialize<Response>(result, JsonConfig.Options).Type != ResponseType.Success)
					return result;
			}

			//! 注册按钮 layer4
			{
				Console.WriteLine("Register Layer:4 ...");

				LayerBox textBox = new()
				{
					Request = RequestType.Set,
					Attribute = new()
					{
						Type = "ControllerButtom",
						Position = new(870, 400),
						Size = new(316, 45),
						ID = new() { LayoutID = _gameLayout, LayerID = 4, },
					}
				};

				string result = RegisterLayer(JsonSerializer.Serialize(textBox, JsonConfig.Options));
				if (JsonSerializer.Deserialize<Response>(result, JsonConfig.Options).Type != ResponseType.Success)
					return result;
			}

			//! 设置图片
			{
				Console.WriteLine("Set Image Layer:0 ...");

				ImageBoxInfo image = new()
				{
					ID = new() { LayoutID = _gameLayout, LayerID = 0, },

					Image = new() { ImageName = "bg010a", SubRect = new(0, 0, 1820, 1024), }
				};

				string result = SetImageBoxInfo(JsonSerializer.Serialize(image, JsonConfig.Options));
				if (JsonSerializer.Deserialize<Response>(result, JsonConfig.Options).Type != ResponseType.Success)
					return result;
			}

			//! 设置颜色
			{
				Console.WriteLine("Set Color Layer:2 ...");

				ColorBoxInfo image = new()
				{
					ID = new() { LayoutID = _gameLayout, LayerID = 2, },
					R = 112,
					G = 146,
					B = 190,
					A = 160,
				};
				string result = SetColorBoxInfo(JsonSerializer.Serialize(image, JsonConfig.Options));
				if (JsonSerializer.Deserialize<Response>(result, JsonConfig.Options).Type != ResponseType.Success)
					return result;
			}

			//! 设置文字
			{
				Console.WriteLine("Set Text Layer:3 ...");
				TextBoxInfo text = new()
				{
					ID = new() { LayoutID = _gameLayout, LayerID = 3, },
					Text = "Hello Wrold, 你好世界: 中文测试: ascii可打印字符、换行、中文字体\n1234567890 ABCDEFGHIJKLMNOPQRSTUVWXYZ abcdefghijklmnopqrstuvwxyz ,.:+-=_!@#$%^&*'\"`~ <>()[]{} /|\\",
					Font = "simhei",
					FontSize = 30,
				};
				string result = SetTextBoxInfo(JsonSerializer.Serialize(text, JsonConfig.Options));
				if (JsonSerializer.Deserialize<Response>(result, JsonConfig.Options).Type != ResponseType.Success)
					return result;
			}

			//! 设置按钮
			{
				Console.WriteLine("Set Buttom Layer:4 ...");

				ButtomBoxInfo buttom = new()
				{
					ID = new() { LayoutID = _gameLayout, LayerID = 4, },
					NormalImage = new() { ImageName = "title", SubRect = new(1, 723, 316, 45), },
					HoverImage = new() { ImageName = "title", SubRect = new(321, 723, 316, 45), },
					PressedImage = new() { ImageName = "title", SubRect = new(641, 723, 316, 45), },
				};

				string result = SetButtomBoxInfo(JsonSerializer.Serialize(buttom, JsonConfig.Options));
				if (JsonSerializer.Deserialize<Response>(result, JsonConfig.Options).Type != ResponseType.Success)
					return result;

				if (_layoutManager!.Layouts[_gameLayout].Layers.TryGetValue(4, out Layer.ILayer? value))
				{
					LogEventHandler logEventHandler = new();
					logEventHandler.RegistEvent(value);
				}
			}

			//! 设置活动节点
			{
				Console.WriteLine($"Set Active Layout: {_gameLayout}");
				LayerIdInfo info = new() { LayoutID = _gameLayout, };

				string result = SetActiveLayout(JsonSerializer.Serialize(info, JsonConfig.Options));
				if (JsonSerializer.Deserialize<Response>(result, JsonConfig.Options).Type != ResponseType.Success)
					return result;
			}

			Response response = new()
			{
				Type = ResponseType.Success,
				Message = "Hello WebGal"
			};

			return JsonSerializer.Serialize(response, JsonConfig.Options);
		}
	}
}