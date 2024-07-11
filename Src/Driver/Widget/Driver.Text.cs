using System.Text.Json;
using Microsoft.JSInterop;
using SkiaSharp;
using WebGal.API.Data;
using WebGal.Global;
using WebGal.Layer;
using WebGal.Layer.Widget;

namespace WebGal.API;


/// <summary>
/// 文本管理接口：包括处理游戏中的文本显示、对话框管理、文本样式和格式化等功能。这个接口应该允许开发人员轻松地添加、编辑和显示游戏中的文本内容。
/// </summary>
public partial class Driver
{
	#region API
	[JSInvokable]
	public static string SetTextBoxInfo(string json)
	{
		var info = JsonSerializer.Deserialize<TextBoxInfo>(json, JsonConfig.Options);
		return JsonSerializer.Serialize(SetTextBoxInfo(info), JsonConfig.Options);
	}

	[JSInvokable]
	public static string SetTextBoxText(string json)
	{
		var info = JsonSerializer.Deserialize<TextBoxText>(json, JsonConfig.Options);
		return JsonSerializer.Serialize(SetTextBoxText(info), JsonConfig.Options);
	}

	[JSInvokable]
	public static string SetTextBoxFont(string json)
	{
		var info = JsonSerializer.Deserialize<TextBoxFont>(json, JsonConfig.Options);
		return JsonSerializer.Serialize(SetTextBoxFont(info), JsonConfig.Options);
	}

	[JSInvokable]
	public static string SetTextBoxFontSize(string json)
	{
		var info = JsonSerializer.Deserialize<TextBoxFontSize>(json, JsonConfig.Options);
		return JsonSerializer.Serialize(SetTextBoxFontSize(info), JsonConfig.Options);
	}
	#endregion


	#region Driect
	public static Response SetTextBoxInfo(TextBoxInfo info)
	{
		Response response;
		response = CheckInit(); if (response.Type != ResponseType.Success) return response;
		response = CheckLayer(info.ID); if (response.Type != ResponseType.Success) return response;

		if (info.Font != "" && _resourceManager!.CheckFont(info.Font) == false)
			return new($"Font: {info.Font} is not loaded");

		ILayer layer = _layoutManager!.Layouts[info.ID.LayoutID].Layers[info.ID.LayerID];

		if (layer is WidgetTextBox textBox)
		{
			textBox.Text = info.Text;
			textBox.Style = new() { MarginBottom = 20 };
			textBox.SetColor(SKColors.Wheat);
			textBox.SetFontSize(info.FontSize);
			textBox.SetFontStyle(_resourceManager!.GetFont(info.Font));
			return new();
		}
		else
			return new($"Layout:{info.ID.LayoutID} Layer:{info.ID.LayerID} not WidgetTextBox");
	}

	public static Response SetTextBoxText(TextBoxText info)
	{
		Response response;
		response = CheckInit(); if (response.Type != ResponseType.Success) return response;
		response = CheckLayer(info.ID); if (response.Type != ResponseType.Success) return response;

		ILayer layer = _layoutManager!.Layouts[info.ID.LayoutID].Layers[info.ID.LayerID];

		if (layer is WidgetTextBox textBox)
		{
			textBox.Text = info.Text;
			return new();
		}
		else
			return new($"Layout:{info.ID.LayoutID} Layer:{info.ID.LayerID} not WidgetTextBox");
	}

	public static Response SetTextBoxFont(TextBoxFont info)
	{
		Response response;
		response = CheckInit(); if (response.Type != ResponseType.Success) return response;
		response = CheckLayer(info.ID); if (response.Type != ResponseType.Success) return response;

		if (info.Font != "" && _resourceManager!.CheckFont(info.Font) == false)
		{
			response.Type = ResponseType.Fail;
			response.Message = $"Font: {info.Font} is not loaded";
			return response;
		}

		ILayer layer = _layoutManager!.Layouts[info.ID.LayoutID].Layers[info.ID.LayerID];

		if (layer is WidgetTextBox textBox)
		{
			textBox.SetFontStyle(_resourceManager!.GetFont(info.Font));
			return new();
		}
		else
			return new($"Layout:{info.ID.LayoutID} Layer:{info.ID.LayerID} not WidgetTextBox");
	}

	public static Response SetTextBoxFontSize(TextBoxFontSize info)
	{
		Response response;
		response = CheckInit(); if (response.Type != ResponseType.Success) return response;
		response = CheckLayer(info.ID); if (response.Type != ResponseType.Success) return response;

		ILayer layer = _layoutManager!.Layouts[info.ID.LayoutID].Layers[info.ID.LayerID];

		if (layer is WidgetTextBox textBox)
		{
			textBox.SetFontSize(info.FontSize);
			return new();
		}
		else
			return new($"Layout:{info.ID.LayoutID} Layer:{info.ID.LayerID} not WidgetTextBox");
	}
	#endregion
}