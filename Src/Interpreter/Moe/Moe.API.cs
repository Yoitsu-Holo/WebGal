using Microsoft.JSInterop;
using WebGal.API;
using WebGal.API.Data;
using FileInfo = WebGal.API.Data.FileInfo;

namespace WebGal.MeoInterpreter;

public partial class MoeInterpreter
{
	[JSInvokable]
	public static async Task GameStartAsync()
	{
		Clear();

		Response response;
		FileInfo elfFile = new() { Type = FileType.Script, Name = "elf", URL = "/main.elf", };

		response = await Driver.PullFileAsync(elfFile);
		if (response.Type != ResponseType.Success) throw new Exception(response.Message);

		response = Driver.GetScriptAsync(elfFile);
		if (response.Type != ResponseType.Success) throw new Exception(response.Message);

		await LoadELF(response.Message);
		Dump();
	}

	[JSInvokable]
	public static async Task GameTestAsync() => await Test.GameTestAsync();

	[JSInvokable]
	public static async Task Benchmark() => await Test.Benchmark();

	[JSInvokable]
	public static async Task ScriptBanech() => await Test.ScriptBench();

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
		foreach (var item in _elfHeader.ImageFiles)
			Console.WriteLine($"{item.Value}");
		foreach (var item in _elfHeader.AudioFiles)
			Console.WriteLine($"{item.Value}");
		foreach (var item in _elfHeader.TextFiles)
			Console.WriteLine($"{item.Value}");
		foreach (var item in _elfHeader.BinFiles)
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