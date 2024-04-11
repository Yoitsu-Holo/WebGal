using System.Text.Json;
using Microsoft.JSInterop;
using SkiaSharp;
using WebGal.API.Data;
using WebGal.Layer;
using WebGal.Layer.Widget;

namespace WebGal.API;


/// <summary>
/// 单色框接口：包括处理游戏中的文本显示、对话框管理、文本样式和格式化等功能。这个接口应该允许开发人员轻松地添加、编辑和显示游戏中的文本内容。
/// </summary>
public partial class Driver
{
	[JSInvokable]
	public static string SetColorBoxInfo(string json)
	{
		Response respone = new()
		{
			Type = ResponseType.Success,
			Message = "",
		};
		var info = JsonSerializer.Deserialize<ColorBoxInfo>(json);

		if (_resourceManager is null || _layoutManager is null)
		{
			respone.Type = ResponseType.Fail;
			respone.Message = "LayoutManager not set OR Game not loading";
			return JsonSerializer.Serialize(respone);
		}

		string responeString = CheckLayer(info.ID);
		respone = JsonSerializer.Deserialize<Response>(responeString);
		if (respone.Type != ResponseType.Success)
			return responeString;

		ILayer layer = _layoutManager.Layouts[info.ID.LayoutID].Layers[info.ID.LayerID];


		if (layer is WidgetColorBox imageBox)
		{
			imageBox.SetColor(new SKColor(info.R, info.G, info.B, info.A));
		}
		else
		{
			respone.Type = ResponseType.Fail;
			respone.Message = $"Layout:{info.ID.LayoutID} Layer:{info.ID.LayerID} not WidgetImageBox";
			return JsonSerializer.Serialize(respone);
		}

		respone.Type = ResponseType.Success;
		return JsonSerializer.Serialize(respone);
	}

	[JSInvokable]
	public static string SetColorBoxImage(string json)
	{
		Response respone = new();
		var image = JsonSerializer.Deserialize<ColorBoxColor>(json);

		return JsonSerializer.Serialize(respone);
	}
}