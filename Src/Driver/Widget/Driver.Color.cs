using System.Text.Json;
using Microsoft.JSInterop;
using SkiaSharp;
using WebGal.API.Data;
using WebGal.Global;
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
		Response respone = new();
		var info = JsonSerializer.Deserialize<ColorBoxInfo>(json, JsonConfig.Options);

		var (flag, ret) = CheckLayer(info.ID);
		if (flag == false) return ret;

		ILayer layer = _layoutManager!.Layouts[info.ID.LayoutID].Layers[info.ID.LayerID];

		if (layer is WidgetColorBox imageBox)
		{
			imageBox.SetColor(new SKColor(info.R, info.G, info.B, info.A));
		}
		else
		{
			respone.Type = ResponseType.Fail;
			respone.Message = $"Layout:{info.ID.LayoutID} Layer:{info.ID.LayerID} not WidgetImageBox";
			return JsonSerializer.Serialize(respone, JsonConfig.Options);
		}

		return JsonSerializer.Serialize(respone, JsonConfig.Options);
	}

	[JSInvokable]
	public static string SetColorBoxImage(string json)
	{
		Response respone = new();
		var image = JsonSerializer.Deserialize<ColorBoxColor>(json, JsonConfig.Options);

		return JsonSerializer.Serialize(respone, JsonConfig.Options);
	}
}