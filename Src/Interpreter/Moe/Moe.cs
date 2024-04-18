using Microsoft.JSInterop;
using WebGal.API;
using WebGal.API.Data;
using FileInfo = WebGal.API.Data.FileInfo;

namespace WebGal.MeoInterpreter;

public partial class MoeInterpreter
{
	protected static readonly StringSplitOptions defaultStringSplitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
	protected static readonly ElfHeader _elfHeader = new();
	protected static readonly MoeRuntime _runtime = new();

#pragma warning disable CA2211
	protected static int _activeTask;
#pragma warning restore CA2211

	public static Dictionary<string, MoeVariable> Variables => _runtime.Variables;
	public static Dictionary<int, Stack<MoeStackFrame>> Tasks => _runtime.Tasks;
	public static Stack<MoeStackFrame> ActiveTasks => _runtime.Tasks[_activeTask];


	[JSInvokable]
	public static async Task GameStart()
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

		response = await Driver.GetScriptAsync(elfFile);
		if (response.Type != ResponseType.Success) throw new Exception(response.Message);

		Console.WriteLine(response.Message);
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
		foreach (var item in _elfHeader.File)
			Console.WriteLine($"{item.Key}:: {item.Value}");

		Console.WriteLine(">>> Dump Function: ");
		foreach (var item in _elfHeader.Function)
			Console.WriteLine($"{item.Key}:\n{item.Value}");

		Console.WriteLine(">>> Dump Vaiable: ");
		foreach (var item in _elfHeader.Data)
			Console.WriteLine($"{item.Key}:: {item.Value}");

		Console.WriteLine(">>> Dump Form: ");
		foreach (var item in _elfHeader.Data)
			Console.WriteLine($"{item.Key}:\n{item.Value}");

		Console.WriteLine(">>> Dump Start: ");
		Console.WriteLine("\t" + _elfHeader.Start);
		Console.WriteLine(_elfHeader.Function[_elfHeader.Start]);
	}
}