using System.ComponentModel;

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

		for (int lineCount = 0; lineCount < MoeELFs.Count; lineCount++)
		{
			string line = MoeELFs[lineCount];

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
					List<int> varDimension = [];
					int varSize = 0;
					MoeBasicType varType = type;

					Lexer varLex = new(rawVar);
					varLex.Parse();

					List<SingleToken> tokens = varLex.Tokens;

					if (tokens[0].Type != TokenType.Name)
						throw new Exception("错误的变量名称: " + tokens[0].Value);
					if (tokens.Count > 1 && (tokens[1].Value != "[" || tokens[^1].Value != "]"))
					{
						Console.WriteLine(tokens[^1].Type + " " + tokens[1].Value + " " + tokens[^1].Value);
						throw new Exception("错误的多维数组申明： 错误的语法格式 " + rawVar);
					}
					if (tokens.Count == 3)
						throw new Exception("错误的多维数组申明： 未声明数组大小 " + rawVar);

					varSize = 1;

					if (tokens.Count > 3)
					{
						for (int i = 2; i < tokens.Count - 1; i++)
						{
							if (i % 2 == 0 && tokens[i].Type == TokenType.Number)
							{
								int size = Convert.ToInt32(tokens[i].Value);
								varSize *= size;
								varDimension.Add(size);
							}
							else if (i % 2 == 1)
							{
								if (tokens[i].Value != ":")
									throw new Exception("错误的多维数组申明： 错误的维度分隔符 " + tokens[i].Value);
							}
							else
								throw new Exception("错误的多维数组申明： " + rawVar);
						}
					}
					else
						varDimension.Add(1);

					MoeVariable variable = new()
					{
						Access = access,
						Type = varType,
						Dimension = varDimension,
						Obj = varType switch
						{
							MoeBasicType.Int => new int[varSize],
							MoeBasicType.Double => new double[varSize],
							MoeBasicType.String => new string[varSize],
							_ => throw new Exception("Unknow Type")
						},
						Size = varSize
					};

					_elfHeader.Data[tokens[0].Value] = variable;
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


		// 加载完毕，将elf header中变量数据加入到全局运行空间
		foreach (var item in _elfHeader.Data)
			_globleSpace.VariableData[item.Key] = item.Value;

		// 扫描所有脚本
		foreach (var (_, file) in _elfHeader.File)
		{
			if (file.FileType != MoeFileType.Text_script)
				continue;
			Lexer lexer = new(_resourceManager.GetScript(file.FileName));
			Snytax snytax = new();
			lexer.Parse();
			snytax.ProgramBuild(lexer.GlobleStatements);
			// foreach (var statement in lexer.GlobleStatements.Statements)
			// {
			// 	List<SingleToken> tokens = statement.Tokens;
			// 	if (tokens.Count < 5)
			// 		throw new Exception("错误的函数定义");
			// 	if (tokens[0].Type != TokenType.Keyword | tokens[0].Value != "func")
			// 		throw new Exception("错误的函数全局定义关键字");
			// 	if (tokens[1].Type != TokenType.Type)
			// 		throw new Exception("错误的函数返回值类型");
			// 	if (tokens[2].Type != TokenType.Name)
			// 		throw new Exception("错误的函数名称");
			// 	if (tokens[3].Value != "(" || tokens[^1].Value != ")")
			// 		throw new Exception("错误的函数参数列表");

			// 	// Console.Write(">>>: ");
			// 	// foreach (var item in statement.Tokens)
			// 	// 	Console.Write(item.Value + " ");
			// 	// Console.WriteLine();
			// 	// ProgramNode programNode = snytax.ProgramBuild(statement);
			// 	// Console.WriteLine(programNode);
			// }
		}

		// _globleSpace.InterpretFile.Name = _elfHeader.Function[_elfHeader.Start].FileName;
		// _globleSpace.InterpretFile.Line = _elfHeader.Function[_elfHeader.Start].FileLine;
	}
}