using System.Text.Json;
using Microsoft.JSInterop;
using WebGal.API;
using WebGal.Global;
using WebGal.Services.Include;

namespace WebGal.Driver;


// 全局驱动程序，也是引擎暴露的接口
public partial class Driver
{
	private readonly SceneManager _sceneManager;
	private readonly ResourceManager _resourceManager;

	public Driver(SceneManager sceneManager, ResourceManager resourceManager)
	{
		_sceneManager = sceneManager;
		_resourceManager = resourceManager;
	}


	//! test
	[JSInvokable]
	public static Task<string> SayHelloAsync(string name)
	{
		// 暴露接口
		Console.WriteLine($"Hello, {name}!");
		return Task.FromResult($"Hello, {name}!");
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
	public static Task<string> Demo(string json)
	{
		return Task.FromResult("Result json");
	}


	public static Task<string> Bg(string jsonBackGround)
	{
		ResponseHeader respone = new();
		var bg = JsonSerializer.Deserialize<BackGround>(jsonBackGround);



		respone.Type = ResponseType.Success;
		return Task.FromResult(JsonSerializer.Serialize(respone, JsonConfig.Options));
	}
}