using System.Text.Json;
using Microsoft.JSInterop;
using SkiaSharp;
using WebGal.API.Data;
using WebGal.Layer;
using WebGal.Layer.Widget;
using WebGal.Libs.Base;
using WebGal.Types;

namespace WebGal.API;


/// <summary>
/// 文本管理接口：包括处理游戏中的文本显示、对话框管理、文本样式和格式化等功能。这个接口应该允许开发人员轻松地添加、编辑和显示游戏中的文本内容。
/// </summary>
public partial class Driver
{
	[JSInvokable]
	public static string SetTextBoxInfo(string json)
	{
		ResponseHeader respone = new()
		{
			Type = ResponseType.Fail,
			Message = "",
		};
		var info = JsonSerializer.Deserialize<TextBoxInfo>(json);

		if (_resourceManager is null || _layoutManager is null)
		{
			respone.Message = "LayoutManager not set OR Game not loading";
			return JsonSerializer.Serialize(respone);
		}

		if (info.Font != "" && _resourceManager.CheckFont(info.Font) == false)
		{
			respone.Message = $"Image: {info.Font} is not loaded";
			return JsonSerializer.Serialize(respone);
		}

		if (_layoutManager.Layouts.ContainsKey(info.LayoutID) == false)
		{
			respone.Message = $"Layout:{info.LayoutID} not registered";
			return JsonSerializer.Serialize(respone);
		}

		Layout layout = _layoutManager.Layouts[info.LayoutID];
		if (layout.Layers.ContainsKey(info.LayerID) == false)
		{
			respone.Message = $"Layer:{info.LayerID} not registered";
			return JsonSerializer.Serialize(respone);
		}

		ILayer layer = layout.Layers[info.LayerID];


		if (layer is WidgetTextBox textBox)
		{
			textBox.Text = info.Text;
			textBox.Style = new()
			{
				MarginBottom = 20
			};
			textBox.SetColor(SKColors.Red);
			textBox.SetFontSize(info.FontSize);
			textBox.SetFontStyle(_resourceManager.GetFont(info.Font));
		}
		else
		{
			respone.Message = $"Layout:{info.LayoutID} Layer:{info.LayerID} not WidgetTextBox";
			return JsonSerializer.Serialize(respone);
		}

		respone.Type = ResponseType.Success;
		return JsonSerializer.Serialize(respone);
	}

	[JSInvokable]
	public static string SetTextBoxText(string json)
	{
		ResponseHeader respone = new();
		var text = JsonSerializer.Deserialize<TextBoxText>(json);

		return JsonSerializer.Serialize(respone);
	}

	[JSInvokable]
	public static string SetTextBoxFont(string json)
	{
		ResponseHeader respone = new();
		var font = JsonSerializer.Deserialize<TextBoxFont>(json);

		return JsonSerializer.Serialize(respone);
	}

	[JSInvokable]
	public static string SetTextBoxFontSize(string json)
	{
		ResponseHeader respone = new();
		var size = JsonSerializer.Deserialize<TextBoxFontSize>(json);

		return JsonSerializer.Serialize(respone);
	}
}