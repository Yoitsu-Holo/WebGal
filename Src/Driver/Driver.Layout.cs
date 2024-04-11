using System.Text.Json;
using Microsoft.JSInterop;
using WebGal.API.Data;
using WebGal.Global;
using WebGal.Layer;
using WebGal.Layer.Controller;
using WebGal.Layer.Widget;
using WebGal.Libs.Base;

namespace WebGal.API;


/// <summary>
/// 文本管理接口：包括处理游戏中的文本显示、对话框管理、文本样式和格式化等功能。这个接口应该允许开发人员轻松地添加、编辑和显示游戏中的文本内容。
/// </summary>
public partial class Driver
{
	[JSInvokable]
	public static string RegisterLayout(string json)
	{
		var layoutInfo = JsonSerializer.Deserialize<LayoutInfo>(json, JsonConfig.Options);
		Response respone = new();

		var (flag, ret) = CheckInit();
		if (flag == false) return ret;

		if (_layoutManager!.Layouts.ContainsKey(layoutInfo.LayoutId) == false)
			_layoutManager.Layouts[layoutInfo.LayoutId] = new();

		return JsonSerializer.Serialize(respone, JsonConfig.Options);
	}

	[JSInvokable]
	public static string RegisterLayer(string json)
	{
		Response respone = new();
		var layerInfo = JsonSerializer.Deserialize<LayerBox>(json, JsonConfig.Options);

		var (flag, ret) = CheckInit();
		if (flag == false) return ret;

		if (_layoutManager!.Layouts.TryGetValue(layerInfo.Attribute.LayoutID, out Layout? value))
		{
			Layout layout = value;
			layout.Layers[layerInfo.Attribute.LayerID] = layerInfo.Attribute.Type switch
			{
				LayerType.TextBox => new WidgetTextBox(),
				LayerType.ImageBox => new WidgetImageBox(),
				LayerType.ColorBox => new WidgetColorBox(),
				LayerType.ButtomBox => new ControllerButtom(),
				LayerType.ControllerBox => throw new Exception("控制组件未完善: todo"),
				_ => throw new Exception("未标识的控件类型: todo"),
			};
			ILayer layer = layout.Layers[layerInfo.Attribute.LayerID];
			layer.Size = layerInfo.Attribute.Size;
			layer.Position = layerInfo.Attribute.Position;
		}
		else
		{
			respone.Type = ResponseType.Fail;
			respone.Message = $"Layout {layerInfo.Attribute.LayoutID} not registed";
			return JsonSerializer.Serialize(respone, JsonConfig.Options);
		}

		return JsonSerializer.Serialize(respone, JsonConfig.Options);
	}

	[JSInvokable]
	public static string SetActiveLayout(string json)
	{
		Response respone = new();
		var info = JsonSerializer.Deserialize<LayerIdInfo>(json, JsonConfig.Options);

		var (flag, ret) = CheckLayout(info);
		if (flag == false) return ret;

		_layoutManager!.ActiveLayout = info.LayoutID;

		return JsonSerializer.Serialize(respone, JsonConfig.Options);
	}

	public static (bool, string) CheckLayout(LayerIdInfo info)
	{
		Response respone = new();

		var (flag, ret) = CheckInit();
		if (flag == false)
			return (flag, ret);

		if (_layoutManager!.Layouts.ContainsKey(info.LayoutID) == false)
		{
			respone.Type = ResponseType.Fail;
			respone.Message = $"Layout:{info.LayoutID} not registered";
			return (false, JsonSerializer.Serialize(respone, JsonConfig.Options));
		}

		return (true, JsonSerializer.Serialize(respone, JsonConfig.Options));
	}

	public static (bool, string) CheckLayer(LayerIdInfo info)
	{
		Response respone = new();

		var (flag, ret) = CheckLayout(info);
		if (flag == false)
			return (flag, ret);

		if (_layoutManager!.Layouts[info.LayoutID].Layers.ContainsKey(info.LayerID) == false)
		{
			respone.Type = ResponseType.Fail;
			respone.Message = $"Layer:{info.LayerID} not registered";
		}

		return (true, JsonSerializer.Serialize(respone, JsonConfig.Options));
	}
}
