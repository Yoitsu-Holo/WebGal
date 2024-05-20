using System.Text.Json;
using Microsoft.JSInterop;
using SkiaSharp;
using WebGal.Animations;
using WebGal.API.Data;
using WebGal.Global;
using WebGal.Handler;
using WebGal.Handler.Event;
using WebGal.Layer.Controller;
using WebGal.Layer.Widget;
using WebGal.Types;
using FileInfo = WebGal.API.Data.FileInfo;

namespace WebGal.API;


/// <summary>
/// 文本管理接口：包括处理游戏中的文本显示、对话框管理、文本样式和格式化等功能。这个接口应该允许开发人员轻松地添加、编辑和显示游戏中的文本内容。
/// </summary>
public partial class Driver
{
	/*
	//测试画面
	DotNet.invokeMethodAsync('WebGal', 'TestAsync', '')
		.then(result => {console.log(result);});

	// 拉取图片资源
	DotNet.invokeMethodAsync('WebGal', 'PullFileAsync', '{"Type":1,"Name":"st-aoi","URL":"/Image/st-aoi_a101.png"}')
		.then(result => {console.log(result);});

	// 注册图层 1
	DotNet.invokeMethodAsync('WebGal', 'RegisterLayer', '{"Request":"set","Attribute":{"Type":1,"Position":{"X":0,"Y":0},"Size":{"X":464,"Y":763},"ID":{"LayoutID":10,"LayerID":1}}}')
		.then(result => {console.log(result);});

	// 设置图层 1
	DotNet.invokeMethodAsync('WebGal', 'SetImageBoxInfo', '{"ID":{"LayoutID":10,"LayerID":1},"Image":{"ImageName":"st-aoi","SubRect":{"X":0,"Y":0,"W":0,"H":0}}}')
		.then(result => {console.log(result);});

	// 裁切背景图片
	DotNet.invokeMethodAsync('WebGal', 'SetImageBoxInfo', '{"ID":{"LayoutID":10,"LayerID":0},"Image":{"ImageName":"bg010a","SubRect":{"X":0,"Y":0,"W":1280,"H":720}}}')
		.then(result => {console.log(result);});

	// 音频测试
	DotNet.invokeMethodAsync('WebGal', 'AudioTestAsync', '')
		.then(result => {console.log(result);});
	
	DotNet.invokeMethodAsync('WebGal', 'DirectTestAsync', '')
		.then(result => {console.log(result);});
	*/

	private const int _menuLayout = 0;
	private const int _gameLayout = 10;

	[JSInvokable]
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
			LayoutInfo layoutInfo = new()
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

	[JSInvokable]
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

			string result = await RegisterAudioContextAsync(JsonSerializer.Serialize(info, JsonConfig.Options));
			if (JsonSerializer.Deserialize<Response>(result, JsonConfig.Options).Type != ResponseType.Success)
				return result;
		}

		//! 注册处理节点0 源节点
		{
			Console.WriteLine("Register AudioNode:0 ...");
			AudioNodeInfo info = new()
			{
				Request = RequestType.Set,
				ID = new() { ContextID = 0, NodeID = 0, },

				Type = AudioNodeType.Source,
			};

			string result = await RegisterAudioNodeAsync(JsonSerializer.Serialize(info, JsonConfig.Options));
			if (JsonSerializer.Deserialize<Response>(result, JsonConfig.Options).Type != ResponseType.Success)
				return result;
		}

		//! 注册处理节点1 增益节点
		{
			Console.WriteLine("Register AudioNode:1 ...");
			AudioNodeInfo info = new()
			{
				Request = RequestType.Set,
				ID = new() { ContextID = 0, NodeID = 1, },

				Type = AudioNodeType.Gain,
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

				Type = AudioNodeType.Speeker,
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

	[JSInvokable]
	public static async Task DirectTestAsync()
	{
		// await Task.Run(() => { });
		if (!_layoutManager!.Layouts.ContainsKey(_gameLayout))
			_layoutManager.Layouts[_gameLayout] = new();
		await _resourceManager!.PullImageAsync("title", "/Image/title01_chip.png");

		//!
		WidgetImageBox imageLayer1 = new()
		{
			Size = new(40, 40),
			Position = new(400, 400),
			Offset = new(-20, -20),
			Animation = AnimationRegister.GetAnimation("AnimationRotate"),
		};
		imageLayer1.Animation.SetParama(new AnimationRotateData() { Z = 2.5, });

		SKBitmap bitmap = new(40, 40);
		using (var canvas = new SKCanvas(bitmap))
		{
			canvas.DrawRect(0, 0, 40, 40, new SKPaint() { Color = SKColors.Bisque, }); canvas.Flush();
			imageLayer1.SetImage(bitmap);
		}

		_layoutManager.Layouts[_gameLayout].Layers[10] = imageLayer1;

		//!
		WidgetImageBox imageLayer2 = new()
		{
			Size = new(1280, 720),
			Position = new(0, 0),
		};
		imageLayer2.SetImage(_resourceManager!.GetImage("title"), new IRect(0, 0, 1280, 720));

		if (!_layoutManager!.Layouts.ContainsKey(_menuLayout))
			_layoutManager.Layouts[_menuLayout] = new();

		//! 
		ControllerButtom buttom = new()
		{
			Size = new(316, 45),
			Position = new(1280 / 2, 720 / 2),
			Offset = new(-316 / 2, -45 / 2),
		};
		buttom.SetImage(_resourceManager!.GetImage("title"), new IRect(1, 723, 316, 45), 0);
		buttom.SetImage(_resourceManager!.GetImage("title"), new IRect(321, 723, 316, 45), 1);
		buttom.SetImage(_resourceManager!.GetImage("title"), new IRect(641, 723, 316, 45), 2);
		_layoutManager.Layouts[_menuLayout].Layers[1] = buttom;

		_layoutManager.ActiveLayout = _menuLayout;

		HandlerBase handler = new()
		{
			HandlerAction = (value) =>
			{
				Console.WriteLine("Trigger!");
				if (value is MouseEventData mouse)
				{
					if (mouse.Status == MouseStatus.Hold)
						_layoutManager.ActiveLayout = _gameLayout;
				}
			}
		};
		handler.RegistEvent(_layoutManager.Layouts[_menuLayout].Layers[1]);

		HandlerBase layoutHandle = new()
		{
			HandlerAction = (value) =>
			{
				if (value is MouseEventData mouse)
				{
					if (mouse.Status == MouseStatus.Down)
						Console.WriteLine("Out Trigger!");
				}
			}
		};
		layoutHandle.RegistEvent(_layoutManager.Layouts[_menuLayout]);
	}
}