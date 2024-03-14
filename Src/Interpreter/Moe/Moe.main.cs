using WebGal.Services.Include;

namespace WebGal.MeoInterpreter;

public partial class MoeInterpreter(SceneManager sceneManager, ResourceManager resourceManager)
{
	private static readonly StringSplitOptions defaultStringSplitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
	private readonly ElfHeader _elfHeader = new();
	private readonly MoeRuntime _globleSpace = new();

	private readonly SceneManager _sceneManager = sceneManager;
	private readonly ResourceManager _resourceManager = resourceManager;

	private readonly string _activeTaskName = "main";
	private Stack<MoeStackFrame> _activeTask = new();


	private static void LineSpcaeFormatter(ref string rawString)
	{
		string[] ss = rawString.Split(' ', defaultStringSplitOptions);
		rawString = "";
		foreach (string s in ss)
		{
			rawString += s;
			rawString += " ";
		}
	}

	public void Dump()
	{
		Console.WriteLine(">>> Dump File: ");
		foreach (var item in _elfHeader.File)
			Console.WriteLine($"\t{item.Key}:\t{item.Value}");

		Console.WriteLine(">>> Dump Function: ");
		foreach (var item in _elfHeader.Function)
			Console.WriteLine($"\t{item.Key}:\t{item.Value}");

		Console.WriteLine(">>> Dump Vaiable: ");
		foreach (var item in _elfHeader.Data)
			Console.WriteLine($"\t{item.Key}:\t{item.Value}");

		Console.WriteLine(">>> Dump Form: ");
		foreach (var item in _elfHeader.Data)
			Console.WriteLine($"\t{item.Key}:\t{item.Value}");

		Console.WriteLine(">>> Dump Start: ");
		Console.WriteLine("\t" + _elfHeader.Start);
		Console.WriteLine(_elfHeader.Function[_elfHeader.Start]);
	}
}