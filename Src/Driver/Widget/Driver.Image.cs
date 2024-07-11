using System.Text.Json;
using Microsoft.JSInterop;
using WebGal.API.Data;
using WebGal.Global;
using WebGal.Layer;
using WebGal.Layer.Widget;
using WebGal.Types;

namespace WebGal.API;


/// <summary>
/// 图片接口：包括处理游戏中的文本显示、对话框管理、文本样式和格式化等功能。这个接口应该允许开发人员轻松地添加、编辑和显示游戏中的文本内容。
/// </summary>
public partial class Driver
{
	#region API
	[JSInvokable]
	public static string SetImageBoxInfo(string json)
	{
		var info = JsonSerializer.Deserialize<ImageBoxInfo>(json, JsonConfig.Options);
		return JsonSerializer.Serialize(SetImageBoxInfo(info), JsonConfig.Options);
	}

	[JSInvokable]
	public static string SetImageBoxImage(string json)
	{
		var info = JsonSerializer.Deserialize<ImageBoxImage>(json, JsonConfig.Options);
		return JsonSerializer.Serialize(SetImageBoxImage(info), JsonConfig.Options);
	}
	#endregion


	#region Driect
	public static Response SetImageBoxInfo(ImageBoxInfo info)
	{
		Response response;
		response = CheckInit(); if (response.Type != ResponseType.Success) return response;
		response = CheckLayer(info.ID); if (response.Type != ResponseType.Success) return response;


		if (_resourceManager!.CheckImage(info.Image.ImageName) == false)
			return new($"Image: {info.Image.ImageName} is not loaded");

		ILayer layer = _layoutManager!.Layouts[info.ID.LayoutID].Layers[info.ID.LayerID];

		if (layer is WidgetImageBox imageBox)
			imageBox.SetImage(_resourceManager.GetImage(info.Image.ImageName), info.Image.SubRect);
		else
			return new($"Layout:{info.ID.LayoutID} Layer:{info.ID.LayerID} not WidgetImageBox");

		return response;
	}

	public static Response SetImageBoxImage(ImageBoxImage json)
	{
		Response response = new();
		return response;
	}
	#endregion
}