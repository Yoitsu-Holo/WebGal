using System.Text.Json;
using Microsoft.JSInterop;
using WebGal.API.Data;
using WebGal.Layer;
using WebGal.Layer.Widget;
using WebGal.Libs.Base;

namespace WebGal.API;


/// <summary>
/// 图片接口：包括处理游戏中的文本显示、对话框管理、文本样式和格式化等功能。这个接口应该允许开发人员轻松地添加、编辑和显示游戏中的文本内容。
/// </summary>
public partial class Driver
{
	[JSInvokable]
	public static string SetImageBoxInfo(string json)
	{
		ResponseHeader respone = new()
		{
			Type = ResponseType.Fail,
			Message = "",
		};
		var info = JsonSerializer.Deserialize<ImageBoxInfo>(json);

		if (_resourceManager is null || _layoutManager is null)
		{
			respone.Message = "LayoutManager not set OR Game not loading";
			return JsonSerializer.Serialize(respone);
		}

		if (_resourceManager.CheckImage(info.ImageName) == false)
		{
			respone.Message = $"Image: {info.ImageName} is not loaded";
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


		if (layer is WidgetImageBox imageBox)
		{
			imageBox.SetImage(_resourceManager.GetImage(info.ImageName));
		}
		else
		{
			respone.Message = $"Layout:{info.LayoutID} Layer:{info.LayerID} not WidgetImageBox";
			return JsonSerializer.Serialize(respone);
		}

		respone.Type = ResponseType.Success;
		return JsonSerializer.Serialize(respone);
	}

	[JSInvokable]
	public static string SetImageBoxImage(string json)
	{
		ResponseHeader respone = new();
		var image = JsonSerializer.Deserialize<ImageBoxImage>(json);

		return JsonSerializer.Serialize(respone);
	}
}