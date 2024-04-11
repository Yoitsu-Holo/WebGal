using System.Text.Json;
using Microsoft.JSInterop;
using SkiaSharp;
using WebGal.API.Data;
using WebGal.Global;
using WebGal.Layer;
using WebGal.Layer.Widget;
using WebGal.Libs.Base;

namespace WebGal.API;


/// <summary>
/// 文本管理接口：包括处理游戏中的文本显示、对话框管理、文本样式和格式化等功能。这个接口应该允许开发人员轻松地添加、编辑和显示游戏中的文本内容。
/// </summary>
public partial class Driver
{
	[JSInvokable]
	public static string SetTextBoxInfo(string json)
	{
		Response respone = new()
		{
			Type = ResponseType.Success,
			Message = "",
		};
		var info = JsonSerializer.Deserialize<TextBoxInfo>(json, JsonConfig.Options);

		if (_resourceManager is null || _layoutManager is null)
		{
			respone.Type = ResponseType.Fail;
			respone.Message = "LayoutManager not set OR Game not loading";
			return JsonSerializer.Serialize(respone, JsonConfig.Options);
		}

		if (info.Font != "" && _resourceManager.CheckFont(info.Font) == false)
		{
			respone.Type = ResponseType.Fail;
			respone.Message = $"Image: {info.Font} is not loaded";
			return JsonSerializer.Serialize(respone, JsonConfig.Options);
		}

		string responeString = CheckLayer(info.ID);
		respone = JsonSerializer.Deserialize<Response>(responeString, JsonConfig.Options);
		if (respone.Type != ResponseType.Success)
			return responeString;

		ILayer layer = _layoutManager.Layouts[info.ID.LayoutID].Layers[info.ID.LayerID];

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
			respone.Type = ResponseType.Fail;
			respone.Message = $"Layout:{info.ID.LayoutID} Layer:{info.ID.LayerID} not WidgetTextBox";
			return JsonSerializer.Serialize(respone, JsonConfig.Options);
		}

		respone.Type = ResponseType.Success;
		return JsonSerializer.Serialize(respone, JsonConfig.Options);
	}

	[JSInvokable]
	public static string SetTextBoxText(string json)
	{
		Response respone = new();
		var text = JsonSerializer.Deserialize<TextBoxText>(json, JsonConfig.Options);

		return JsonSerializer.Serialize(respone, JsonConfig.Options);
	}

	[JSInvokable]
	public static string SetTextBoxFont(string json)
	{
		Response respone = new();
		var font = JsonSerializer.Deserialize<TextBoxFont>(json, JsonConfig.Options);

		return JsonSerializer.Serialize(respone, JsonConfig.Options);
	}

	[JSInvokable]
	public static string SetTextBoxFontSize(string json)
	{
		Response respone = new();
		var size = JsonSerializer.Deserialize<TextBoxFontSize>(json, JsonConfig.Options);

		return JsonSerializer.Serialize(respone, JsonConfig.Options);
	}
}