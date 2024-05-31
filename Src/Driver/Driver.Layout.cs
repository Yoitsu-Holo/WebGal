using System.Text.Json;
using Microsoft.JSInterop;
using WebGal.API.Data;
using WebGal.Global;
using WebGal.Handler;
using WebGal.Layer;

namespace WebGal.API;


/// <summary>
/// 文本管理接口：包括处理游戏中的文本显示、对话框管理、文本样式和格式化等功能。这个接口应该允许开发人员轻松地添加、编辑和显示游戏中的文本内容。
/// </summary>
public partial class Driver
{
	#region API
	[JSInvokable]
	public static string RegisteLayout(string json)
	{
		var info = JsonSerializer.Deserialize<LayerIdInfo>(json, JsonConfig.Options);
		return JsonSerializer.Serialize(RegisteLayout(info), JsonConfig.Options);
	}

	[JSInvokable]
	public static string RegisteLayer(string json)
	{
		var info = JsonSerializer.Deserialize<LayerBox>(json, JsonConfig.Options);
		return JsonSerializer.Serialize(RegisteLayer(info), JsonConfig.Options);
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

	[JSInvokable]
	public static void RegisteLayoutAction(string json)
	{
		// Todo
		Logger.LogInfo("JS 事件处理", Global.LogLevel.Todo);
	}
	#endregion


	#region Driect
	public static Response RegisteLayout(LayerIdInfo info)
	{
		Response response = CheckInit();

		if (response.Type != ResponseType.Success) return response;

		if (_layoutManager!.Layouts.ContainsKey(info.LayoutID) == false)
			_layoutManager.Layouts[info.LayoutID] = new();

		response.Type = ResponseType.Success;
		response.Message = "";
		return response;
	}

	public static Response RegisteLayer(LayerBox info)
	{
		Response response = CheckInit();

		if (response.Type != ResponseType.Success) return response;

		if (_layoutManager!.Layouts.TryGetValue(info.Attribute.ID.LayoutID, out Layout? layout))
		{
			layout.Layers[info.Attribute.ID.LayerID] = LayerBoxRegister.GetLayerBox(info.Attribute.Type);
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

	public static Response RegisteLayoutAction(LayerIdInfo info, Func<EventArgs, bool> action)
	{
		Response response = CheckLayout(info);
		if (response.Type != ResponseType.Success) return response;

		_layoutManager!.Layouts[info.LayoutID].RegisterAction(new ActionBase() { Action = action });

		return response;
	}
	#endregion
}
