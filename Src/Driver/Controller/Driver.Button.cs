using System.Text.Json;
using Microsoft.JSInterop;
using WebGal.API.Data;
using WebGal.Global;
using WebGal.Layer;
using WebGal.Layer.Controller;

namespace WebGal.API;


/// <summary>
/// 图片接口：包括处理游戏中的文本显示、对话框管理、文本样式和格式化等功能。这个接口应该允许开发人员轻松地添加、编辑和显示游戏中的文本内容。
/// </summary>
public partial class Driver
{
	#region API
	[JSInvokable]
	public static string SetButtonBoxInfo(string json)
	{
		var info = JsonSerializer.Deserialize<ButtonBoxInfo>(json, JsonConfig.Options);
		return JsonSerializer.Serialize(SetButtonBoxInfo(info), JsonConfig.Options);
	}

	[JSInvokable]
	public static string SetButtonBoxImage(string json)
	{
		var info = JsonSerializer.Deserialize<ImageBoxImage>(json, JsonConfig.Options);
		return JsonSerializer.Serialize(SetButtonBoxImage(info), JsonConfig.Options);
	}
	#endregion


	#region Driect
	public static Response SetButtonBoxInfo(ButtonBoxInfo info)
	{
		Response response;
		response = CheckInit(); if (response.Type != ResponseType.Success) return response;
		response = CheckLayer(info.ID); if (response.Type != ResponseType.Success) return response;


		if (_resourceManager!.CheckImage(info.NormalImage.ImageName) == false)
			return new($"Button: {info.NormalImage.ImageName} is not loaded");

		ILayer layer = _layoutManager!.Layouts[info.ID.LayoutID].Layers[info.ID.LayerID];
		if (layer is ControllerButton buttonBox)
		{
			if (_resourceManager.CheckImage(info.NormalImage.ImageName))
				buttonBox.SetImage(_resourceManager.GetImage(info.NormalImage.ImageName), info.NormalImage.SubRect, 0);

			if (_resourceManager.CheckImage(info.HoverImage.ImageName))
				buttonBox.SetImage(_resourceManager.GetImage(info.HoverImage.ImageName), info.HoverImage.SubRect, 1);

			if (_resourceManager.CheckImage(info.PressedImage.ImageName))
				buttonBox.SetImage(_resourceManager.GetImage(info.PressedImage.ImageName), info.PressedImage.SubRect, 2);

			if (_resourceManager.CheckImage(info.FocusedImage.ImageName))
				buttonBox.SetImage(_resourceManager.GetImage(info.FocusedImage.ImageName), info.FocusedImage.SubRect, 3);
			return new();
		}
		else
			return new($"Layout:{info.ID.LayoutID} Layer:{info.ID.LayerID} not WidgetImageBox");
	}

	public static Response SetButtonBoxImage(ImageBoxImage info)
	{
		Response response = new();
		return response;
	}
	#endregion
}