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
		string[] MoeELFs = MoeELF.Split('\n');
		MoeELF elfFlag = MeoInterpreter.MoeELF.Void;
		for (int i = 0; i < MoeELFs.Length; i++)
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
			string[] lines = line.Split(" ");

			if (elfFlag == MeoInterpreter.MoeELF.FILE)
			{
				if (lines.Length != 3)
				{
					throw new Exception();
					// continue;
				}

				string fileName = lines[0];
				MoeFileType fileType = lines[1] switch
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
				};
				string fileURL = lines[2];

				_elfHeader.MoeFiles[fileName] = new()
				{
					FileName = fileName,
					FileType = fileType,
					FileURL = fileURL,
				};

				continue;
			}

			// File Loader 预加载所有脚本和字体，图片和音频过大，不在此加载

			List<Task> tasks = [];
			foreach (var (_, file) in _elfHeader.MoeFiles)
				if (file.FileType == MoeFileType.Text_script || file.FileType == MoeFileType.Text_ui)
					tasks.Add(_resourceManager.PullScriptAsync(file.FileName, file.FileURL));
				else if (file.FileType == MoeFileType.Bin_font)
					tasks.Add(_resourceManager.PullFontAsync(file.FileName, file.FileURL));

			await Task.WhenAll(tasks);

			if (elfFlag == MeoInterpreter.MoeELF.TABLE)
			{
				foreach (var (_, file) in _elfHeader.MoeFiles)
				{
					if (file.FileType != MoeFileType.Text_script)
						continue;
					// script
					string[] codes = _resourceManager.GetScript(file.FileName).Split('\n');

					// ! 扫描所有代码行
					for (int fileLine = 0; fileLine < codes.Length; fileLine++)
					{
						string codeLine = codes[fileLine];

						LineSpcaeFormatter(ref codeLine);

						string[] words = codeLine.Split(' ');

						if (words[0] != "func")
							continue;

						// 处理函数


						string[] parts = codeLine.Split(':');

						if (parts.Length != 2)
							throw new Exception(file.FileName + " : " + fileLine + " : " + codeLine + " 不完整的函数定义");

						// 格式化字符串
						LineSpcaeFormatter(ref parts[0]); // 函数签名
						LineSpcaeFormatter(ref parts[1]); // 函数参数

						string[] signature = parts[0].Split(' ');
						string[] paramaterList = parts[1].Split(',');

						if (signature.Length != 3)
							throw new Exception(file.FileName + " : " + fileLine + " : " + parts[0] + "不完整或错误的函数签名");

						MoeFunction func = new()
						{
							FileName = file.FileName,
							FileLine = fileLine,
							ReturnType = signature[1] switch
							{
								"int" => MoeBasicType.Int,
								"double" => MoeBasicType.Double,
								"string" => MoeBasicType.String,
								_ => MoeBasicType.Void,
							}
						};

						if (paramaterList.Length <= 0)
							throw new Exception(file.FileName + " : " + fileLine + " : " + parts[0] + "错误的参数列表");

						for (int ip = 0; ip < paramaterList.Length && paramaterList[0] != "void"; ip++)
						{
							string paramater = paramaterList[ip];

							LineSpcaeFormatter(ref paramater);

							string[] variableInfo = paramater.Split(' ');
							string[] temp = variableInfo[2].Split(':');

							variableInfo[2] = temp[0];
							variableInfo[3] = "1";

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

							_ = func.CallType.Append(variable);
						}

						_elfHeader.MoeFunctions[func.FunctionName] = func;
					}
				}
			}

			if (elfFlag == MeoInterpreter.MoeELF.DATA)
			{
				if (lines.Length < 3)
				{
					throw new Exception();
					// continue;
				}

				MoeBasicAccess access = MoeBasicAccess.Void;
				MoeBasicType type = MoeBasicType.Void;

				access |= lines[0] switch
				{
					"const" => MoeBasicAccess.Const,
					"static" => MoeBasicAccess.Static,
					"var" => MoeBasicAccess.Variable,
					_ => MoeBasicAccess.Void,
				};
				type |= lines[1] switch
				{
					"int" => MoeBasicType.Int,
					"string" => MoeBasicType.String,
					"double" => MoeBasicType.Double,
					_ => MoeBasicType.Void,
				};

				string temp = "";
				for (int elm = 2; i < lines.Length; i++)
					temp += lines[elm];

				lines = temp.Split(',');

				foreach (var rawVar in lines)
				{
					string varName = "";
					int varSize = 0;
					MoeBasicType varType = type;
					var rawVarPart = rawVar.Split(':');

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
				}

				continue;
			}

			if (elfFlag == MeoInterpreter.MoeELF.START)
			{

			}
		}
	}

	private static bool IsAlpha(char c) => (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
	private static bool IsDigital(char c) => (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');

	private static bool IsNumber(string s)
	{
		for (int i = 0; i < s.Length; i++)
		{
			char c = s[i];
			if (i == 0 && c == '-')
				continue;
			if (!IsDigital(c))
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
		string[] ss = rawString.Split(' ');
		rawString = "";
		foreach (string s in ss)
		{
			if (s == "" || s == "\t")
				continue;
			rawString += s;
			_ = rawString.Append(' ');
		}
	}

	private static bool CheckParamaDefine(string[] variableDefine)
	{
		if (variableDefine.Length != 4)
			throw new Exception("错误的参数定义");

		if (variableDefine[0] != "" && variableDefine[0] != "" && variableDefine[0] != "")
			throw new Exception("错误的变量修饰");

		if (variableDefine[1] != "int" && variableDefine[1] != "double" && variableDefine[1] != "string")
			throw new Exception("错误的变量类型");

		if (!IsLableName(variableDefine[2]))
			throw new Exception("错误的变量名称");

		if (!IsNumber(variableDefine[3]) && variableDefine[3][0] != '-')
			throw new Exception("错误的变量长度");

		return true;
	}

	public void Dump()
	{
		Console.WriteLine("File: ");

		foreach (var item in _elfHeader.MoeFiles)
		{
			Console.WriteLine(item.Key);
			Console.WriteLine(item.Value);
		}

		Console.WriteLine("Function: ");
		foreach (var item in _elfHeader.MoeFunctions)
		{
			Console.WriteLine(item.Key);
			Console.WriteLine(item.Value);
		}

		Console.WriteLine("Vaiable: ");
		foreach (var item in _elfHeader.MoeData)
		{
			Console.WriteLine(item.Key);
			Console.WriteLine(item.Value);
		}


		Console.WriteLine("Form: ");
		foreach (var item in _elfHeader.MoeData)
		{
			Console.WriteLine(item.Key);
			Console.WriteLine(item.Value);
		}

		Console.WriteLine("Start: ");
		Console.WriteLine(_elfHeader.MoeStart);
	}
}