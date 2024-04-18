using System.Text.Json;
using Microsoft.JSInterop;
using WebGal.API.Data;
using WebGal.Audio;
using WebGal.Global;

namespace WebGal.API;

public partial class Driver
{
	#region API
	// [JSInvokable]
	// public static async Task<string> SetAudioSpeekerInfoAsync(string json)
	// {
	// 	return JsonSerializer.Serialize(await SetAudioSpeekerInfoAsync(), JsonConfig.Options);
	// }
	#endregion


	// 	public static async Task<string> SetAudioSpeekerInfoAsync()
	// 	{
	// 		Response respone = new()
	// 		{
	// 			Type = ResponseType.Success,
	// 			Message = ""
	// 		};
	// 		await Task.Run(() => { });
	// 		return JsonSerializer.Serialize(respone, JsonConfig.Options);
	// 	}
}
