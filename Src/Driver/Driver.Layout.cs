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
	#region API
	[JSInvokable]
	public static string RegisterLayout(string json)
	{
		var info = JsonSerializer.Deserialize<LayoutInfo>(json, JsonConfig.Options);
		return JsonSerializer.Serialize(RegisterLayout(info), JsonConfig.Options);
	}

	[JSInvokable]
	public static string RegisterLayer(string json)
	{
		var info = JsonSerializer.Deserialize<LayerBox>(json, JsonConfig.Options);
		return JsonSerializer.Serialize(RegisterLayer(info), JsonConfig.Options);
	}

	[JSInvokable]
	public static string SetActiveLayout(string json)
	{
		var info = JsonSerializer.Deserialize<LayerIdInfo>(json, JsonConfig.Options);
		return JsonSerializer.Serialize(SetActiveLayout(info), JsonConfig.Options);
	}

	[JSInvokable]
	public static void DumpLayout()
	{
		if (_layoutManager is null)
			return;
		foreach (var (layoutID, layout) in _layoutManager.Layouts)
		{
			Console.WriteLine("Layout : " + layoutID + " : ");
			foreach (var (layerID, layer) in layout.Layers)
				Console.WriteLine($"\t Layer : {layerID} : {layer}");
		}
	}
	#endregion

	public static Response RegisterLayout(LayoutInfo info)
	{
		Response response = CheckInit();

		if (response.Type != ResponseType.Success) return response;

		if (_layoutManager!.Layouts.ContainsKey(info.LayoutID) == false)
			_layoutManager.Layouts[info.LayoutID] = new();

		response.Type = ResponseType.Success;
		response.Message = "";
		return response;
	}

	public static Response RegisterLayer(LayerBox info)
	{
		Response response = CheckInit();

		if (response.Type != ResponseType.Success) return response;

		if (_layoutManager!.Layouts.TryGetValue(info.Attribute.ID.LayoutID, out Layout? value))
		{
			Layout layout = value;
			layout.Layers[info.Attribute.ID.LayerID] = info.Attribute.Type switch
			{
				LayerType.TextBox => new WidgetTextBox(),
				LayerType.ImageBox => new WidgetImageBox(),
				LayerType.ColorBox => new WidgetColorBox(),
				LayerType.ButtomBox => new ControllerButtom(),
				LayerType.ControllerBox => throw new Exception("控制组件未完善: todo"),
				_ => throw new Exception("未标识的控件类型: todo"),
			};
			ILayer layer = layout.Layers[info.Attribute.ID.LayerID];
			layer.Size = info.Attribute.Size;
			layer.Position = info.Attribute.Position;
		}
		else
		{
			response.Type = ResponseType.Fail;
			response.Message = $"Layout {info.Attribute.ID.LayoutID} not registed";
			return response;
		}

		return response;
	}

	public static Response SetActiveLayout(LayerIdInfo info)
	{
		Response response = CheckLayout(info);
		if (response.Type != ResponseType.Success) return response;

		_layoutManager!.ActiveLayout = info.LayoutID;

		return response;
	}

	public static Response CheckLayout(LayerIdInfo info)
	{
		Response response = CheckInit();
		if (response.Type != ResponseType.Success) return response;

		if (_layoutManager!.Layouts.ContainsKey(info.LayoutID) == false)
		{
			response.Type = ResponseType.Fail;
			response.Message = $"Layout:{info.LayoutID} not registered";
		}

		return response;
	}

	public static Response CheckLayer(LayerIdInfo info)
	{
		Response response = CheckLayout(info);
		if (response.Type != ResponseType.Success) return response;

		if (_layoutManager!.Layouts[info.LayoutID].Layers.ContainsKey(info.LayerID) == false)
		{
			response.Type = ResponseType.Fail;
			response.Message = $"Layer:{info.LayerID} not registered";
		}

		return response;
	}
}
