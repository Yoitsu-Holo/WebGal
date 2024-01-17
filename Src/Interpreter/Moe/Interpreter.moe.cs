using WebGal.Services.Include;

namespace WebGal.MeoInterpreter;

/// <summary>
/// 程序加载结构描述
/// 脚本程序标准加载文件应包含如下四个部分
/// file	文件名称和 [类型，URL] 的映射表
/// table	程序函数和文件对应关系 [文件，函数名称行数]
/// data	程序数据段，包含全局变量
/// form	界面配置文件，通过json文件描述
/// start	程序开始位置
/// </summary>
public class MoeInterpreter
{
	private static readonly StringSplitOptions defaultStringSplitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
	private readonly ElfHeader _elfHeader = new();

	private readonly SceneManager _sceneManager;
	private readonly ResourceManager _resourceManager;

	public MoeInterpreter(SceneManager sceneManager, ResourceManager resourceManager)
	{
		_sceneManager = sceneManager;
		_resourceManager = resourceManager;
	}

	public async Task LoadELF(string MoeELF)
	{
		List<string> MoeELFs = new(MoeELF.Split('\n', defaultStringSplitOptions));
		MoeELF elfFlag = MeoInterpreter.MoeELF.Void;

		for (int i = 0; i < MoeELFs.Count; i++)
		{
			string line = MoeELFs[i];

			// Filter empty lines
			if (line == "")
				continue;

			// Header Flag
			if (line[0] == '.')
			{
				elfFlag = line switch
				{
					".file" => MeoInterpreter.MoeELF.FILE,
					".table" => MeoInterpreter.MoeELF.TABLE,
					".data" => MeoInterpreter.MoeELF.DATA,
					".form" => MeoInterpreter.MoeELF.FORM,
					".start" => MeoInterpreter.MoeELF.START,
					_ => MeoInterpreter.MoeELF.Void,
				};
				continue;
			}

			if (elfFlag == MeoInterpreter.MoeELF.Void)
				continue;

			LineSpcaeFormatter(ref line);
			List<string> lines = new(line.Split(' ', defaultStringSplitOptions));

			if (elfFlag == MeoInterpreter.MoeELF.FILE)
			{
				if (lines.Count != 3)
					throw new Exception("错误的参数数量" + line);

				_elfHeader.MoeFiles[lines[0]] = new()
				{
					FileName = lines[0],
					FileType = lines[1] switch
					{
						"png" => MoeFileType.Img_png,
						"jpg" => MoeFileType.Img_jpg,
						"bmp" => MoeFileType.Img_bmp,

						"wav" => MoeFileType.Audio_wav,
						"mp3" => MoeFileType.Audio_mp3,
						"flac" => MoeFileType.Audio_flac,
						"midi" => MoeFileType.Audio_midi,

						"script" => MoeFileType.Text_script,
						"ui" => MoeFileType.Text_ui,

						"bin" => MoeFileType.Bin_font,
						"block" => MoeFileType.Bin_block,

						_ => MoeFileType.Void,
					},
					FileURL = lines[2],
				};
				continue;
			}

			if (elfFlag == MeoInterpreter.MoeELF.DATA)
			{
				if (lines.Count < 3)
					throw new Exception("错误的参数数量" + line);

				MoeBasicAccess access = MoeBasicAccess.Void;
				MoeBasicType type = MoeBasicType.Void;

				access = lines[0] switch
				{
					"const" => MoeBasicAccess.Const,
					"static" => MoeBasicAccess.Static,
					"var" => MoeBasicAccess.Variable,
					_ => MoeBasicAccess.Void,
				};
				type = lines[1] switch
				{
					"int" => MoeBasicType.Int,
					"string" => MoeBasicType.String,
					"double" => MoeBasicType.Double,
					_ => MoeBasicType.Void,
				};

				string temp = "";
				for (int tempIndex = 2; tempIndex < lines.Count; tempIndex++)
					temp += lines[tempIndex];

				lines = new(temp.Split(',', defaultStringSplitOptions));

				foreach (var rawVar in lines)
				{
					string varName = "";
					int varSize = 0;
					MoeBasicType varType = type;
					var rawVarPart = rawVar.Split(':', defaultStringSplitOptions);

					varName = rawVarPart[0];
					varSize = (rawVarPart.Length == 2) ? AtoI(rawVarPart[1]) : 1;

					if (!IsLableName(varName) || varSize <= 0)
						throw new Exception("Error Paramater");

					MoeVariable variable = new()
					{
						Access = access,
						Type = varType,
						Obj = varType switch
						{
							MoeBasicType.Int => new int[varSize],
							MoeBasicType.Double => new double[varSize],
							MoeBasicType.String => new string[varSize],
							_ => throw new Exception("Unknow Type")
						},
						Size = varSize
					};

					_elfHeader.MoeData[varName] = variable;
				}
				continue;
			}

			if (elfFlag == MeoInterpreter.MoeELF.START)
			{
				if (lines.Count != 1)
					throw new Exception("错误的参数数量");

				_elfHeader.MoeStart = lines[0];
			}
		}


		// File Loader 预加载所有脚本和字体，图片和音频过大，不在此加载
		List<Task> tasks = [];
		foreach (var (_, file) in _elfHeader.MoeFiles)
		{
			if (file.FileType == MoeFileType.Text_script || file.FileType == MoeFileType.Text_ui)
				tasks.Add(_resourceManager.PullScriptAsync(file.FileName, file.FileURL));
			else if (file.FileType == MoeFileType.Bin_font)
				tasks.Add(_resourceManager.PullFontAsync(file.FileName, file.FileURL));
		}

		await Task.WhenAll(tasks);

		// 单独扫描文件
		foreach (var (_, file) in _elfHeader.MoeFiles)
		{
			if (file.FileType != MoeFileType.Text_script)
				continue;
			// script
			List<string> codes = new(_resourceManager.GetScript(file.FileName).Split('\n'));

			// ! 扫描所有代码行
			for (int fileLine = 0; fileLine < codes.Count; fileLine++)
			{
				string codeLine = codes[fileLine];

				LineSpcaeFormatter(ref codeLine);

				List<string> words = new(codeLine.Split(' ', defaultStringSplitOptions));

				if (words.Count < 1 || words[0] != "func")
					continue;

				// 处理函数
				List<string> parts = new(codeLine.Split('@', defaultStringSplitOptions));

				if (parts.Count != 2)
					throw new Exception(file.FileName + " : " + fileLine + " : " + codeLine + " 不完整的函数定义");


				// 函数签名
				string[] signature = parts[0].Split(' ', defaultStringSplitOptions);
				if (signature.Length != 3)
					throw new Exception(file.FileName + " : " + fileLine + " : " + parts[0] + "不完整或错误的函数签名");

				MoeFunction func = new()
				{
					FileName = file.FileName,
					FileLine = fileLine,
					FunctionName = signature[2],
					ReturnType = signature[1] switch
					{
						"int" => MoeBasicType.Int,
						"double" => MoeBasicType.Double,
						"string" => MoeBasicType.String,
						_ => MoeBasicType.Void,
					}
				};


				// 函数参数
				string[] paramaterList = parts[1].Split(',', defaultStringSplitOptions);
				if (paramaterList.Length <= 0)
					throw new Exception(file.FileName + " : " + fileLine + " : " + parts[0] + "错误的参数列表");

				for (int paramaIndex = 0; paramaIndex < paramaterList.Length && paramaterList[0] != "void"; paramaIndex++)
				{
					string paramater = paramaterList[paramaIndex];

					LineSpcaeFormatter(ref paramater);

					List<string> variableInfo = new(paramater.Split(' ', defaultStringSplitOptions));
					string[] temp = variableInfo[2].Split(':', defaultStringSplitOptions);

					variableInfo[2] = temp[0];
					variableInfo.Add("1");

					if (temp.Length == 2)
						variableInfo[3] = temp[1];


					CheckParamaDefine(variableInfo);

					int variableSize = AtoI(variableInfo[3]);

					MoeVariable variable = new()
					{
						Access = variableInfo[0] switch
						{
							"const" => MoeBasicAccess.Const,
							"static" => MoeBasicAccess.Static,
							"var" => MoeBasicAccess.Variable,
							_ => MoeBasicAccess.Void,
						},
						Type = variableInfo[1] switch
						{
							"int" => MoeBasicType.Int,
							"double" => MoeBasicType.Double,
							"string" => MoeBasicType.String,
							_ => MoeBasicType.Void,
						},
						Size = variableSize,
					};

					if (variable.Type == MoeBasicType.Int)
						variable.Obj = new int[variable.Size];
					else if (variable.Type == MoeBasicType.Double)
						variable.Obj = new double[variable.Size];
					else if (variable.Type == MoeBasicType.String)
						variable.Obj = new string[variable.Size];
					else
						variable.Obj = new byte[variable.Size];

					func.CallType.Add(variable);
				}

				_elfHeader.MoeFunctions[func.FunctionName] = func;
			}
		}
	}

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
		foreach (var item in _elfHeader.MoeFiles)
			Console.WriteLine(item.Key + ":" + item.Value);

		Console.WriteLine(">>> Dump Function: ");
		foreach (var item in _elfHeader.MoeFunctions)
			Console.WriteLine(item.Key + ":" + item.Value);


		Console.WriteLine(">>> Dump Vaiable: ");
		foreach (var item in _elfHeader.MoeData)
			Console.WriteLine(item.Key + ":" + item.Value);

		Console.WriteLine(">>> Dump Form: ");
		foreach (var item in _elfHeader.MoeData)
			Console.WriteLine(item.Key + ":" + item.Value);

		Console.WriteLine(">>> Dump Start: ");
		Console.WriteLine(_elfHeader.MoeStart);
		Console.WriteLine(_elfHeader.MoeFunctions[_elfHeader.MoeStart]);
	}
}