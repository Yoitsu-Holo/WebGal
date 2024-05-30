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
	public static string SetSliderBoxInfo(string json)
	{
		var info = JsonSerializer.Deserialize<SliderBoxInfo>(json, JsonConfig.Options);
		return JsonSerializer.Serialize(SetSliderBoxInfo(info), JsonConfig.Options);
	}

	[JSInvokable]
	public static string SetSliderBoxImage(string json)
	{
		var info = JsonSerializer.Deserialize<SliderBoxImage>(json, JsonConfig.Options);
		return JsonSerializer.Serialize(SetSliderBoxImage(info), JsonConfig.Options);
	}

	[JSInvokable]
	public static string SetSliderBoxTrackImage(string json)
	{
		var info = JsonSerializer.Deserialize<SliderBoxTrackImage>(json, JsonConfig.Options);
		return JsonSerializer.Serialize(SetSliderBoxTrackImage(info), JsonConfig.Options);
	}
	#endregion


	#region Driect
	public static Response SetSliderBoxInfo(SliderBoxInfo info)
	{
		Response response;
		response = CheckInit(); if (response.Type != ResponseType.Success) return response;
		response = CheckLayer(info.ID); if (response.Type != ResponseType.Success) return response;

		if (_resourceManager!.CheckImage(info.NormalImage.ImageName) == false)
		{
			response.Type = ResponseType.Fail;
			response.Message = $"Image: {info.NormalImage.ImageName} is not loaded";
			return response;
		}

		ILayer layer = _layoutManager!.Layouts[info.ID.LayoutID].Layers[info.ID.LayerID];
		if (layer is ControllerSliderBase sliderBox)
		{
			if (_resourceManager.CheckImage(info.TrackImage.ImageName))
				sliderBox.SetImage(_resourceManager.GetImage(info.TrackImage.ImageName), info.TrackImage.SubRect, -1);

			if (_resourceManager.CheckImage(info.NormalImage.ImageName))
				sliderBox.SetImage(_resourceManager.GetImage(info.NormalImage.ImageName), info.NormalImage.SubRect, 0);

			if (_resourceManager.CheckImage(info.HoverImage.ImageName))
				sliderBox.SetImage(_resourceManager.GetImage(info.HoverImage.ImageName), info.HoverImage.SubRect, 1);

			if (_resourceManager.CheckImage(info.PressedImage.ImageName))
				sliderBox.SetImage(_resourceManager.GetImage(info.PressedImage.ImageName), info.PressedImage.SubRect, 2);

			if (_resourceManager.CheckImage(info.FocusedImage.ImageName))
				sliderBox.SetImage(_resourceManager.GetImage(info.FocusedImage.ImageName), info.FocusedImage.SubRect, 3);

			if (info.ThumbSize != default && info.TrackImage != default)
				sliderBox.InitAttribute(info.TrackSize, info.ThumbSize);
		}
		else
		{
			response.Type = ResponseType.Fail;
			response.Message = $"Layout:{info.ID.LayoutID} Layer:{info.ID.LayerID} not WidgetImageBox";
			return response;
		}

		return response;
	}

	public static Response SetSliderBoxImage(SliderBoxImage info)
	{
		Response response = new();
		return response;
	}

	public static Response SetSliderBoxTrackImage(SliderBoxTrackImage info)
	{
		Response response = new();
		return response;
	}
	#endregion
}