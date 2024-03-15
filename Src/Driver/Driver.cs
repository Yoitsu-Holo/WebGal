using Microsoft.JSInterop;

namespace WebGal.Driver;


// 全局驱动程序，也是引擎暴露的接口
public class Driver
{
	[JSInvokable]
	public static Task<string> SayHello(string name)
	{
		// 暴露接口
		Console.WriteLine($"Hello, {name}!");
		return Task.FromResult($"Hello, {name}!");
		/*
		DotNet.invokeMethodAsync('YourAssemblyName', 'SayHello', 'World')
			.then(result => {
				console.log(result); // 输出：Hello, World!
			});
		*/
	}
}