using WebGal.Services.Include;

namespace WebGal.MeoInterpreter;

public partial class MoeInterpreter(SceneManager sceneManager, ResourceManager resourceManager)
{
	private static readonly StringSplitOptions defaultStringSplitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
	private readonly ElfHeader _elfHeader = new();
	private readonly MoeGlobleSpace _globleSpace = new();

	private readonly SceneManager _sceneManager = sceneManager;
	private readonly ResourceManager _resourceManager = resourceManager;


	// 基础方法

	private static bool IsAlpha(char c) => (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
	private static bool IsDigital(char c) => c >= '0' && c <= '9';

	private static bool IsNumber(string s)
	{
		for (int i = (s[0] == '-') ? 1 : 0; i < s.Length; i++)
		{
			if (!IsDigital(s[i]))
				return false;
		}
		return true;
	}

	private static bool IsLableName(string s)
	{
		if (!IsAlpha(s[0]))
			return false;
		for (int i = 1; i < s.Length; i++)
			if (!(IsAlpha(s[i]) || IsDigital(s[i]) || s[i] == '_'))
				return false;
		return true;
	}

	private static int AtoI(string s)
	{
		int flag = (s[0] == '-') ? (-1) : 1;

		int ret = 0;
		for (int i = 0; i < s.Length; i++)
		{
			if (i == 0 && s[i] == '-')
				continue;

			if (s[i] < '0' || s[i] > '9')
				throw new Exception(s + " : not A Number");

			ret *= 10;
			ret += s[i] - '0';
		}
		return ret * flag;
	}

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

	private static bool CheckParamaDefine(List<string> variableDefine)
	{
		if (variableDefine.Count != 4)
			throw new Exception("错误的参数定义: " + variableDefine.Count);

		if (variableDefine[0] != "const" && variableDefine[0] != "static" && variableDefine[0] != "var")
			throw new Exception("错误的变量修饰: " + variableDefine[0]);

		if (variableDefine[1] != "int" && variableDefine[1] != "double" && variableDefine[1] != "string")
			throw new Exception("错误的变量类型: " + variableDefine[1]);

		if (!IsLableName(variableDefine[2]))
			throw new Exception("错误的变量名称: " + variableDefine[2]);

		if (!IsNumber(variableDefine[3]) || variableDefine[3][0] == '-')
			throw new Exception("错误的变量长度: " + variableDefine[3]);

		return true;
	}

	public void Dump()
	{
		Console.WriteLine(">>> Dump File: ");
		foreach (var item in _elfHeader.File)
			Console.WriteLine(item.Key + ":" + item.Value);

		Console.WriteLine(">>> Dump Function: ");
		foreach (var item in _elfHeader.Function)
			Console.WriteLine(item.Key + ":" + item.Value);


		Console.WriteLine(">>> Dump Vaiable: ");
		foreach (var item in _elfHeader.Data)
			Console.WriteLine(item.Key + ":" + item.Value);

		Console.WriteLine(">>> Dump Form: ");
		foreach (var item in _elfHeader.Data)
			Console.WriteLine(item.Key + ":" + item.Value);

		Console.WriteLine(">>> Dump Start: ");
		Console.WriteLine(_elfHeader.Start);
		Console.WriteLine(_elfHeader.Function[_elfHeader.Start]);
	}
}