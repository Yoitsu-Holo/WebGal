using System.Text.Json;
using Microsoft.JSInterop;
using WebGal.API.Data;
using WebGal.Global;
using WebGal.Services.Include;

namespace WebGal.API;


// 全局驱动程序，也是引擎暴露的接口
public partial class Driver
{
	private static LayoutManager? _layoutManager;
	private static ResourceManager? _resourceManager;
	private static AudioManager? _audioManager;


	public static void Init(LayoutManager layoutManager, ResourceManager resourceManager, AudioManager audioManager)
	{
		_layoutManager = layoutManager;
		_resourceManager = resourceManager;
		_audioManager = audioManager;
	}

	public static (bool, string) CheckInit()
	{
		Response respone = new();

		if (_layoutManager is null)
		{
			respone.Type = ResponseType.Fail;
			respone.Message = "LayoutManager not set OR Game not loading";
			return (false, JsonSerializer.Serialize(respone, JsonConfig.Options));
		}

		if (_resourceManager is null)
		{
			respone.Type = ResponseType.Fail;
			respone.Message = "ResourceManager not set OR Game not loading";
			return (false, JsonSerializer.Serialize(respone, JsonConfig.Options));
		}

		if (_audioManager is null)
		{
			respone.Type = ResponseType.Fail;
			respone.Message = "AudioManager not set OR Game not loading";
			return (false, JsonSerializer.Serialize(respone, JsonConfig.Options));
		}

		return (true, JsonSerializer.Serialize(respone, JsonConfig.Options));
	}

	//! test
	[JSInvokable]
	public static async Task<string> SayHelloAsync(string name)
	{
		// 暴露接口
		Console.WriteLine($"Hello, {name}!");
		await Task.Run(() => { });
		return $"Hello, {name}!";
		/*
		DotNet.invokeMethodAsync('WebGal', 'SayHelloAsync', 'World');

		DotNet.invokeMethodAsync('WebGal', 'SayHelloAsync', 'World')
			.then(result => {
				console.log(result); // 输出：Hello, World!
			});
		*/
	}

	//! test
	[JSInvokable]
	public static string SayHello(string name)
	{
		// 暴露接口
		Console.WriteLine($"Hello, {name}!");
		return $"Hello, {name}!";
		/*
		DotNet.invokeMethod('WebGal', 'SayHello', 'World');
		*/
	}

	//! test
	[JSInvokable]
	public static Task<string> Empty()
	{
		Response respone = new()
		{
			Type = ResponseType.Success,
			Message = "",
		};
		return Task.FromResult(JsonSerializer.Serialize(respone, JsonConfig.Options));
	}
}