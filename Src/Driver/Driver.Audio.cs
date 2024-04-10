using System.Text.Json;
using Microsoft.JSInterop;
using WebGal.API.Data;
using WebGal.Audio;
using FileInfo = WebGal.API.Data.FileInfo;

namespace WebGal.API;


/// <summary>
/// 音频处理接口，用于管理游戏音频
/// </summary>
public partial class Driver
{
	[JSInvokable]
	public static string RegisterAudioContext(string json)
	{
		var layoutInfo = JsonSerializer.Deserialize<LayoutInfo>(json);
		ResponseHeader respone = new()
		{
			Type = ResponseType.Success,
			Message = "",
		};

		if (_layoutManager is not null)
		{
			if (_layoutManager.Layouts.ContainsKey(layoutInfo.LayoutId) == false)
				_layoutManager.Layouts[layoutInfo.LayoutId] = new();
		}
		else
			respone = new()
			{
				Type = ResponseType.Fail,
				Message = "LayoutManager not set OR Game not loading",
			};
		return JsonSerializer.Serialize(respone);
	}

	[JSInvokable]
	public static string RegisterAudioNode(string json)
	{
		var layoutInfo = JsonSerializer.Deserialize<LayoutInfo>(json);
		ResponseHeader respone = new()
		{
			Type = ResponseType.Success,
			Message = "",
		};

		if (_layoutManager is not null)
		{
			if (_layoutManager.Layouts.ContainsKey(layoutInfo.LayoutId) == false)
				_layoutManager.Layouts[layoutInfo.LayoutId] = new();
		}
		else
			respone = new()
			{
				Type = ResponseType.Fail,
				Message = "LayoutManager not set OR Game not loading",
			};
		return JsonSerializer.Serialize(respone);
	}

	[JSInvokable]
	public static string CheckAudioContext(string json)
	{
		var layoutInfo = JsonSerializer.Deserialize<LayoutInfo>(json);
		ResponseHeader respone = new()
		{
			Type = ResponseType.Success,
			Message = "",
		};

		if (_layoutManager is not null)
		{
			if (_layoutManager.Layouts.ContainsKey(layoutInfo.LayoutId) == false)
				_layoutManager.Layouts[layoutInfo.LayoutId] = new();
		}
		else
			respone = new()
			{
				Type = ResponseType.Fail,
				Message = "LayoutManager not set OR Game not loading",
			};
		return JsonSerializer.Serialize(respone);
	}
}