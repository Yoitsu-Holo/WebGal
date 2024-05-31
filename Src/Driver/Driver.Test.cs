using System.Text.Json;
using Microsoft.JSInterop;
using WebGal.API.Data;
using WebGal.Global;

namespace WebGal.API;

public partial class Driver
{
	/*
	//测试画面
	DotNet.invokeMethodAsync('WebGal', 'TestAsync', '')
		.then(result => {console.log(result);});

	// 拉取图片资源
	DotNet.invokeMethodAsync('WebGal', 'PullFileAsync', '{"Type":1,"Name":"st-aoi","URL":"/Image/st-aoi_a101.png"}')
		.then(result => {console.log(result);});

	// 注册图层 1
	DotNet.invokeMethodAsync('WebGal', 'RegisterLayer', '{"Request":"set","Attribute":{"Type":1,"Position":{"X":0,"Y":0},"Size":{"X":464,"Y":763},"ID":{"LayoutID":10,"LayerID":1}}}')
		.then(result => {console.log(result);});

	// 设置图层 1
	DotNet.invokeMethodAsync('WebGal', 'SetImageBoxInfo', '{"ID":{"LayoutID":10,"LayerID":1},"Image":{"ImageName":"st-aoi","SubRect":{"X":0,"Y":0,"W":0,"H":0}}}')
		.then(result => {console.log(result);});

	// 裁切背景图片
	DotNet.invokeMethodAsync('WebGal', 'SetImageBoxInfo', '{"ID":{"LayoutID":10,"LayerID":0},"Image":{"ImageName":"bg010a","SubRect":{"X":0,"Y":0,"W":1280,"H":720}}}')
		.then(result => {console.log(result);});

	// 音频测试
	DotNet.invokeMethodAsync('WebGal', 'AudioTestAsync', '')
		.then(result => {console.log(result);});
	
	DotNet.invokeMethodAsync('WebGal', 'DirectTestAsync', '')
		.then(result => {console.log(result);});
	*/

	[JSInvokable]
	public static async Task<string> TestAsync() => await Test.TestAsync();

	[JSInvokable]
	public static async Task<string> AudioTestAsync() => await Test.AudioTestAsync();

	[JSInvokable]
	public static async Task DirectTestAsync() => await Test.DirectTestAsync();

	[JSInvokable]
	public static async Task MoeTest() => await Test.MoeTest();

	[JSInvokable]
	public static async Task RenderBench() => await Test.RenderBench();


	[JSInvokable]
	public static async Task<string> SayHelloAsync(string name)
	{
		await Task.Run(() => { });
		return SayHello(name);
	}

	[JSInvokable]
	public static string SayHello(string name)
	{
		Console.WriteLine($"Hello, {name}!");
		return $"Hello, {name}!";
	}

	[JSInvokable]
	public static Task<string> EmptyAsync()
	{
		Response respone = new();
		return Task.FromResult(JsonSerializer.Serialize(respone, JsonConfig.Options));
	}

	[JSInvokable]
	public static Response Empty()
	{
		Response respone = new();
		return respone;
	}

	[JSInvokable]
	public static void StackTraceTest()
	{
		var trace = new System.Diagnostics.StackTrace();
		Logger.LogInfo("Stack Trace: \n" + trace.ToString());
	}
}