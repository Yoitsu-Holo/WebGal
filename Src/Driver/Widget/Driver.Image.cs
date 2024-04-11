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
	[JSInvokable]
	public static string SetImageBoxInfo(string json)
	{
		Response respone = new()
		{
			Type = ResponseType.Success,
			Message = "",
		};
		var info = JsonSerializer.Deserialize<ImageBoxInfo>(json, JsonConfig.Options);

		if (_resourceManager is null || _layoutManager is null)
		{
			respone.Type = ResponseType.Fail;
			respone.Message = "LayoutManager not set OR Game not loading";
			return JsonSerializer.Serialize(respone, JsonConfig.Options);
		}

		if (_resourceManager.CheckImage(info.Image.ImageName) == false)
		{
			respone.Type = ResponseType.Fail;
			respone.Message = $"Image: {info.Image.ImageName} is not loaded";
			return JsonSerializer.Serialize(respone, JsonConfig.Options);
		}

		string responeString = CheckLayer(info.ID);
		respone = JsonSerializer.Deserialize<Response>(responeString, JsonConfig.Options);
		if (respone.Type != ResponseType.Success)
			return responeString;

		ILayer layer = _layoutManager.Layouts[info.ID.LayoutID].Layers[info.ID.LayerID];


		if (layer is WidgetImageBox imageBox)
		{
			imageBox.SetImage(_resourceManager.GetImage(info.Image.ImageName), info.Image.SubRect);
		}
		else
		{
			respone.Type = ResponseType.Fail;
			respone.Message = $"Layout:{info.ID.LayoutID} Layer:{info.ID.LayerID} not WidgetImageBox";
			return JsonSerializer.Serialize(respone, JsonConfig.Options);
		}

		respone.Type = ResponseType.Success;
		return JsonSerializer.Serialize(respone, JsonConfig.Options);
	}

	[JSInvokable]
	public static string SetImageBoxImage(string json)
	{
		Response respone = new();
		var image = JsonSerializer.Deserialize<ImageBoxImage>(json, JsonConfig.Options);

		return JsonSerializer.Serialize(respone, JsonConfig.Options);
	}
}