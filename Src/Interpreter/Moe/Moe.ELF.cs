// 
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
public partial class MoeInterpreter
{
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

				_elfHeader.File[lines[0]] = new()
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

					_elfHeader.Data[varName] = variable;
				}
				continue;
			}

			if (elfFlag == MeoInterpreter.MoeELF.START)
			{
				if (lines.Count != 1)
					throw new Exception("错误的参数数量");

				_elfHeader.Start = lines[0];
			}
		}


		// File Loader 预加载所有脚本和字体，图片和音频过大，不在此加载
		List<Task> tasks = [];
		foreach (var (_, file) in _elfHeader.File)
		{
			if (file.FileType == MoeFileType.Text_script || file.FileType == MoeFileType.Text_ui)
				tasks.Add(_resourceManager.PullScriptAsync(file.FileName, file.FileURL));
			else if (file.FileType == MoeFileType.Bin_font)
				tasks.Add(_resourceManager.PullFontAsync(file.FileName, file.FileURL));
		}

		await Task.WhenAll(tasks);

		// 单独扫描文件
		foreach (var (_, file) in _elfHeader.File)
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

				_elfHeader.Function[func.FunctionName] = func;
			}
		}

		// 加载完毕，将elf header中变量数据加入到全局运行空间
		foreach (var item in _elfHeader.Data)
			_globleSpace.VariableData[item.Key] = item.Value;

		_globleSpace.InterpretFile.Name = _elfHeader.Function[_elfHeader.Start].FileName;
		_globleSpace.InterpretFile.Line = _elfHeader.Function[_elfHeader.Start].FileLine;
	}
}