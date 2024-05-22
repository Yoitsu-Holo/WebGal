using Microsoft.JSInterop;
using WebGal.API;
using WebGal.API.Data;
using WebGal.Handler.Event;
using FileInfo = WebGal.API.Data.FileInfo;

namespace WebGal.MeoInterpreter;

public partial class MoeInterpreter
{
	[JSInvokable]
	public static async Task GameStartAsync()
	{
		Clear();

		Response response;
		FileInfo elfFile = new()
		{
			Type = FileType.Script,
			Name = "elf",
			URL = "/main.elf",
		};

		response = await Driver.PullFileAsync(elfFile);
		if (response.Type != ResponseType.Success) throw new Exception(response.Message);

		response = Driver.GetScriptAsync(elfFile);
		if (response.Type != ResponseType.Success) throw new Exception(response.Message);

		await LoadELF(response.Message);
		Dump();
	}

	[JSInvokable]
	public static async Task GameTestAsync()
	{
		await GameStartAsync();
		await Driver.RegisterAudioContextAsync(new AudioIdInfo() { ContextID = 0, });
		await Driver.RegisterAudioNodeAsync(new AudioNodeInfo() { Request = RequestType.Set, ID = new() { ContextID = 0, NodeID = 0, }, Type = AudioNodeType.Simple, });
		await Driver.RegisterAudioNodeAsync(new AudioNodeInfo() { Request = RequestType.Set, ID = new() { ContextID = 0, NodeID = 1, }, Type = AudioNodeType.Simple, });
		await Driver.RegisterAudioNodeAsync(new AudioNodeInfo() { Request = RequestType.Set, ID = new() { ContextID = 0, NodeID = 2, }, Type = AudioNodeType.Simple, });

		FormRegister();
		Tasks[_activeTask] = new();
		// Console.WriteLine(Functions["test"]);
		Call(Functions["test"], []);

		ParseScene("sss");
		ParseScene("ch-1");
		SetSceneList("sss");
		// SetSceneList("ch-1");
		LoadScene(0);

		Driver.RegisterLayoutAction(
			new() { LayoutID = 0, },
			(value) => { if (value is MouseEventData mouse && mouse.Status == MouseStatus.Up) { OnCLick(); } }
		);
	}

	[JSInvokable]
	public static void Clear()
	{
		_elfHeader.CLear();
		_runtime.Clear();
		_activeTask = 0;
	}

	[JSInvokable]
	public static void Dump()
	{
		Console.WriteLine(">>> Dump File: ");
		foreach (var item in _elfHeader.Files)
			Console.WriteLine($"{item.Value}");

		Console.WriteLine(">>> Dump Function: ");
		foreach (var item in _elfHeader.Functions)
			Console.WriteLine($"{item.Value.Header}");

		Console.WriteLine(">>> Dump Vaiable: ");
		foreach (var item in _elfHeader.Datas)
			Console.WriteLine($"{item.Value}");

		Console.WriteLine(">>> Dump Form: ");
		foreach (var item in _elfHeader.Forms)
			Console.WriteLine($"{item.Value}");

		Console.WriteLine(">>> Dump Start: ");
		Console.WriteLine("\t" + _elfHeader.Start);
		Console.WriteLine(_elfHeader.Functions[_elfHeader.Start].Header);
	}
}