using System.Text.Json;
using Microsoft.JSInterop;
using WebGal.API.Data;
using WebGal.Layer;
using WebGal.Layer.Controller;
using WebGal.Layer.Widget;
using WebGal.Libs.Base;

namespace WebGal.API;


/// <summary>
/// 文本管理接口：包括处理游戏中的文本显示、对话框管理、文本样式和格式化等功能。这个接口应该允许开发人员轻松地添加、编辑和显示游戏中的文本内容。
/// </summary>
public partial class Driver
{
	[JSInvokable]
	public static string RegisterLayout(string json)
	{
		var layoutInfo = JsonSerializer.Deserialize<LayoutInfo>(json);
		ResponseHeader respone = new()
		{
			Type = ResponseType.Success,
			Message = "",
		};

		if (_layoutManager is not null)
		{
			if (_layoutManager.Layouts.ContainsKey(layoutInfo.LayoutId) == false)
				_layoutManager.Layouts[layoutInfo.LayoutId] = new();
		}
		else
			respone = new()
			{
				Type = ResponseType.Fail,
				Message = "LayoutManager not set OR Game not loading",
			};
		return JsonSerializer.Serialize(respone);
	}

	[JSInvokable]
	public static string RegisterLayer(string json)
	{
		var layerInfo = JsonSerializer.Deserialize<LayerBox>(json);
		ResponseHeader respone = new()
		{
			Type = ResponseType.Success,
			Message = "",
		};

		if (_layoutManager is not null)
		{
			if (_layoutManager.Layouts.TryGetValue(layerInfo.Attribute.LayoutID, out Layout? value))
			{
				Layout layout = value;
				layout.Layers[layerInfo.Attribute.LayerID] = layerInfo.Attribute.Type switch
				{
					LayerType.TextBox => new WidgetTextBox(),
					LayerType.ImageBox => new WidgetImageBox(),
					LayerType.ColorBox => new WidgetColorBox(),
					LayerType.ButtomBox => new ControllerButtom(),
					LayerType.ControllerBox => throw new Exception("控制组件未完善: todo"),
					_ => throw new Exception("未标识的控件类型: todo"),
				};
				ILayer layer = layout.Layers[layerInfo.Attribute.LayerID];
				layer.Size = layerInfo.Attribute.Size;
				layer.Position = layerInfo.Attribute.Position;
			}
			else
			{
				respone = new()
				{
					Type = ResponseType.Fail,
					Message = $"Layout {layerInfo.Attribute.LayoutID} not registed",
				};
			}
		}
		else
			respone = new()
			{
				Type = ResponseType.Fail,
				Message = "LayoutManager not set OR Game not loading",
			};
		return JsonSerializer.Serialize(respone);
	}

	public static string CheckLayout(LayerIdInfo info)
	{
		ResponseHeader respone = new()
		{
			Type = ResponseType.Success,
			Message = "",
		};

		if (_layoutManager is null)
		{
			respone.Type = ResponseType.Fail;
			respone.Message = "LayoutManager not set OR Game not loading";
			return JsonSerializer.Serialize(respone);
		}

		if (_layoutManager.Layouts.ContainsKey(info.LayoutID) == false)
		{
			respone.Type = ResponseType.Fail;
			respone.Message = $"Layout:{info.LayoutID} not registered";
			return JsonSerializer.Serialize(respone);
		}

		respone.Type = ResponseType.Success;
		return JsonSerializer.Serialize(respone);
	}

	public static string CheckLayer(LayerIdInfo info)
	{
		ResponseHeader respone = new()
		{
			Type = ResponseType.Success,
			Message = "",
		};

		if (_layoutManager is null)
		{
			respone.Type = ResponseType.Fail;
			respone.Message = "LayoutManager not set OR Game not loading";
			return JsonSerializer.Serialize(respone);
		}

		string responeString = CheckLayout(info);
		respone = JsonSerializer.Deserialize<ResponseHeader>(responeString);
		if (respone.Type != ResponseType.Success)
			return responeString;

		if (_layoutManager.Layouts[info.LayoutID].Layers.ContainsKey(info.LayerID) == false)
		{
			respone.Type = ResponseType.Fail;
			respone.Message = $"Layer:{info.LayerID} not registered";
		}

		return JsonSerializer.Serialize(respone);
	}
}
