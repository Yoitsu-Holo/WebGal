using System.Text.Json;
using Microsoft.JSInterop;
using WebGal.API.Data;
using WebGal.Audio;
using WebGal.Global;

namespace WebGal.API;

public partial class Driver
{
	[JSInvokable]
	public static async Task<string> SetAudioSpeekerInfoAsync(string json)
	{
		Response respone = new()
		{
			Type = ResponseType.Success,
			Message = ""
		};
		await Task.Run(() => { });
		return JsonSerializer.Serialize(respone, JsonConfig.Options);
	}
}
