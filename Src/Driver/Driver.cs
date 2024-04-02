using System.Text.Json;
using Microsoft.JSInterop;
using WebGal.API.Data;
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
		ResponseHeader respone = new()
		{
			Type = ResponseType.Success,
			Message = "",
		};
		return Task.FromResult(JsonSerializer.Serialize(respone));
	}
}