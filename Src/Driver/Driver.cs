using Microsoft.JSInterop;

namespace WebGal.Driver;


// 全局驱动程序，也是引擎暴露的接口
public partial class Driver
{
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

	[JSInvokable]
	public static Task<string> Demo(string json)
	{
		return Task.FromResult("Result json");
	}
}