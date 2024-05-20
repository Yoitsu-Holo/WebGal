using System.Reflection.Metadata;
using System.Text.Json;
using Microsoft.JSInterop;
using WebGal.API.Data;
using WebGal.Global;
using WebGal.Handler;
using WebGal.Handler.Event;
using WebGal.Services.Include;

namespace WebGal.API;


// 全局驱动程序，也是引擎暴露的接口
public partial class Driver
{
	private static IJSRuntime? _jsRuntime;
	private static LayoutManager? _layoutManager;
	private static ResourceManager? _resourceManager;
	private static AudioManager? _audioManager;
	private static HandlerBase _handlerBase = new();


	public static void Init(LayoutManager layoutManager, ResourceManager resourceManager, AudioManager audioManager)
	{
		_layoutManager = layoutManager;
		_resourceManager = resourceManager;
		_audioManager = audioManager;
		_jsRuntime = audioManager.JSRuntime;

		_handlerBase.HandlerAction = (value) =>
		{
			Console.WriteLine("Trigger!");
			if (value is MouseEventData mouse)
			{
				if (mouse.Status == MouseStatus.Hold)
					_layoutManager.ActiveLayout = _gameLayout;
			}
		};
	}

	public static Response CheckInit()
	{
		Response response = new() { Type = ResponseType.Fail };
		if (_layoutManager is null)
		{
			response.Message = "LayoutManager not set OR Game not loading";
			return response;
		}
		if (_resourceManager is null)
		{
			response.Message = "ResourceManager not set OR Game not loading";
			return response;
		}
		if (_audioManager is null)
		{
			response.Message = "AudioManager not set OR Game not loading";
			return response;
		}

		response.Type = ResponseType.Success;
		return response;
	}

	//! test
	[JSInvokable]
	public static async Task<string> SayHelloAsync(string name)
	{
		await Task.Run(() => { });
		return SayHello(name);
	}

	//! test
	[JSInvokable]
	public static string SayHello(string name)
	{
		Console.WriteLine($"Hello, {name}!");
		return $"Hello, {name}!";
	}

	//! test
	[JSInvokable]
	public static Task<string> Empty()
	{
		Response respone = new();
		return Task.FromResult(JsonSerializer.Serialize(respone, JsonConfig.Options));
	}

	//! test
	[JSInvokable]
	public static async Task StackTraceTest()
	{
		await Task.Run(() => { });
		var trace = new System.Diagnostics.StackTrace();
		Console.WriteLine("Stack Trace: " + trace.ToString());
		// Log.LogWarning("This is a warning message.");
	}


	public event EventHandler<EventArgs>? Subscribers;

	public void TriggerEvent(EventArgs args)
	{
		throw new NotImplementedException();
	}

	public void RegistEvent(IEvent e)
	{
		throw new NotImplementedException();
	}

	public void Action(object? sender, EventArgs eventArgs)
	{
		throw new NotImplementedException();
	}
}