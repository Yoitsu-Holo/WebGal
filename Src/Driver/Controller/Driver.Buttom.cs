using System.Text.Json;
using Microsoft.JSInterop;
using WebGal.API.Data;
using WebGal.Layer;
using WebGal.Layer.Controller;

namespace WebGal.API;


/// <summary>
/// 图片接口：包括处理游戏中的文本显示、对话框管理、文本样式和格式化等功能。这个接口应该允许开发人员轻松地添加、编辑和显示游戏中的文本内容。
/// </summary>
public partial class Driver
{
	[JSInvokable]
	public static string SetButtomBoxInfo(string json)
	{
		ResponseHeader respone = new()
		{
			Type = ResponseType.Success,
			Message = "",
		};
		var info = JsonSerializer.Deserialize<ButtomBoxInfo>(json);

		if (_resourceManager is null || _layoutManager is null)
		{
			respone.Type = ResponseType.Fail;
			respone.Message = "LayoutManager not set OR Game not loading";
			return JsonSerializer.Serialize(respone);
		}

		if (_resourceManager.CheckImage(info.NormalImage.ImageName) == false)
		{
			respone.Type = ResponseType.Fail;
			respone.Message = $"Image: {info.NormalImage.ImageName} is not loaded";
			return JsonSerializer.Serialize(respone);
		}

		string responeString = CheckLayer(info.LayoutID, info.LayerID);
		respone = JsonSerializer.Deserialize<ResponseHeader>(responeString);
		if (respone.Type != ResponseType.Success)
			return responeString;

		ILayer layer = _layoutManager.Layouts[info.LayoutID].Layers[info.LayerID];


		if (layer is ControllerButtom buttomBox)
		{
			if (_resourceManager.CheckImage(info.NormalImage.ImageName))
				buttomBox.SetImage(_resourceManager.GetImage(info.NormalImage.ImageName), info.NormalImage.SubRect, 0);

			if (_resourceManager.CheckImage(info.HoverImage.ImageName))
				buttomBox.SetImage(_resourceManager.GetImage(info.HoverImage.ImageName), info.HoverImage.SubRect, 1);

			if (_resourceManager.CheckImage(info.PressedImage.ImageName))
				buttomBox.SetImage(_resourceManager.GetImage(info.PressedImage.ImageName), info.PressedImage.SubRect, 2);

			if (_resourceManager.CheckImage(info.FocusedImage.ImageName))
				buttomBox.SetImage(_resourceManager.GetImage(info.FocusedImage.ImageName), info.FocusedImage.SubRect, 3);
		}
		else
		{
			respone.Type = ResponseType.Fail;
			respone.Message = $"Layout:{info.LayoutID} Layer:{info.LayerID} not WidgetImageBox";
			return JsonSerializer.Serialize(respone);
		}

		respone.Type = ResponseType.Success;
		return JsonSerializer.Serialize(respone);
	}

	[JSInvokable]
	public static string SetButtomBoxImage(string json)
	{
		ResponseHeader respone = new();
		var image = JsonSerializer.Deserialize<ImageBoxImage>(json);

		return JsonSerializer.Serialize(respone);
	}
}