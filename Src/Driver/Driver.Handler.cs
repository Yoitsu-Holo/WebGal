using Microsoft.JSInterop;
using WebGal.API.Data;
using WebGal.Global;
using WebGal.Handler;

namespace WebGal.API;

public partial class Driver
{
	#region API
	[JSInvokable]
	public static void RegisteLayoutAction(string json)
	{
		// Todo
		Logger.LogInfo("JS 事件处理", Global.LogLevel.Todo);
	}

	[JSInvokable]
	public static void RegisteLayerHandle(string json)
	{
		// Todo
		Logger.LogInfo("JS 事件处理", Global.LogLevel.Todo);
	}
	#endregion


	#region Driect
	public static Response RegisteLayoutAction(LayerIdInfo info, Func<EventArgs, bool> action)
	{
		Response response = CheckLayout(info);
		if (response.Type != ResponseType.Success) return response;

		_layoutManager!.Layouts[info.LayoutID].RegisterAction(new ActionBase() { Action = action });

		return response;
	}

	public static Response RegisteLayerAction(LayerIdInfo info, Func<EventArgs, bool> action)
	{
		Response response = CheckLayout(info);
		if (response.Type != ResponseType.Success) return response;

		_layoutManager!.Layouts[info.LayoutID].Layers[info.LayerID].RegisterAction(new ActionBase() { Action = action });

		return response;
	}
	#endregion
}