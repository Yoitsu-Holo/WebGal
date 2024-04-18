using WebGal.API;
using WebGal.API.Data;
using FileInfo = WebGal.API.Data.FileInfo;

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
		static void LineSpaceFormatter(ref string rawString)
		{
			string[] ss = rawString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			rawString = string.Join(" ", ss);
		}

		List<string> MoeELFs = new(MoeELF.Split('\n', defaultStringSplitOptions));
		MoeELFsegment elfFlag = MoeELFsegment.Void;

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
					".file" => MoeELFsegment.FILE,
					".table" => MoeELFsegment.TABLE,
					".data" => MoeELFsegment.DATA,
					".form" => MoeELFsegment.FORM,
					".start" => MoeELFsegment.START,
					_ => MoeELFsegment.Void,
				};
				continue;
			}

			if (elfFlag == MoeELFsegment.Void)
				continue;

			LineSpaceFormatter(ref line);
			List<string> lines = new(line.Split(' ', defaultStringSplitOptions));

			if (elfFlag == MoeELFsegment.FILE)
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

			if (elfFlag == MoeELFsegment.DATA)
			{
				if (lines.Count < 3)
					throw new Exception("错误的参数数量" + line);

				MoeVariableAccess access = lines[0] switch
				{
					"const" => MoeVariableAccess.Const,
					"static" => MoeVariableAccess.Static,
					"var" => MoeVariableAccess.Partial,
					_ => MoeVariableAccess.Void,
				};
				MoeVariableType type = lines[1] switch
				{
					"int" => MoeVariableType.Int,
					"string" => MoeVariableType.String,
					"double" => MoeVariableType.Double,
					_ => MoeVariableType.Void,
				};
				string temp = "";
				for (int tempIndex = 2; tempIndex < lines.Count; tempIndex++)
					temp += lines[tempIndex];

				lines = new(temp.Split(',', defaultStringSplitOptions));

				foreach (var rawVar in lines)
				{
					List<int> varDimension = [];
					int varSize = 1;
					MoeVariableType varType = type;

					Lexer varLex = new(rawVar);
					varLex.Parse();

					List<SingleToken> tokens = varLex.GlobleTokens;

					if (tokens[0].Type != TokenType.Name)
						throw new Exception("错误的变量名称: " + tokens[0].Value);
					if (tokens.Count > 1 && (tokens[1].Value != "[" || tokens[^1].Value != "]"))
						throw new Exception("错误的多维数组申明： 错误的语法格式 " + rawVar);
					if (tokens.Count == 3)
						throw new Exception("错误的多维数组申明： 未声明数组大小 " + rawVar);

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
						Name = tokens[0].Value,
						Access = access,
						Type = varType,
						Dimension = varDimension,
						Obj = varType switch
						{
							MoeVariableType.Int => new int[varSize],
							MoeVariableType.Double => new double[varSize],
							MoeVariableType.String => new string[varSize],
							_ => throw new Exception("Unknow Type")
						},
					};
					variable.Dimension.Add(varSize);

					_elfHeader.Data[tokens[0].Value] = variable;
				}
				continue;
			}

			if (elfFlag == MoeELFsegment.START)
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
			FileInfo fileInfo;
			if (file.FileType == MoeFileType.Text_script || file.FileType == MoeFileType.Text_ui)
				fileInfo = new() { Type = FileType.Script, Name = file.FileName, URL = file.FileURL, };
			else if (file.FileType == MoeFileType.Bin_font)
				fileInfo = new() { Type = FileType.Font, Name = file.FileName, URL = file.FileURL, };
			else
				continue;
			tasks.Add(Driver.PullFileAsync(fileInfo));
		}
		await Task.WhenAll(tasks);


		// 加载完毕，将elf header中变量数据加入到全局运行空间
		foreach (var item in _elfHeader.Data)
			_runtime.Variables[item.Key] = item.Value;

		// 扫描所有脚本
		foreach (var (_, file) in _elfHeader.File)
		{
			if (file.FileType != MoeFileType.Text_script) continue;

			FileInfo fileInfo = new() { Type = FileType.Script, Name = file.FileName };
			Response response = await Driver.GetScriptAsync(fileInfo);
			if (response.Type != ResponseType.Success) throw new Exception(response.Message);

			Lexer lexer = new(response.Message);
			lexer.Parse();

			Syntax syntax = new();
			var ASTs = syntax.ProgramBuild(lexer.GlobleStatements);

			foreach (var functionAST in ASTs.Statements)
			{
				if (functionAST.ASTType != ASTNodeType.FunctionDeclaration || functionAST.FuncDefine is null)
					throw new Exception($"不能在全局代码区定义函数: File{file.FileName}");
				if (_elfHeader.Function.ContainsKey(functionAST.FuncDefine.FuncName))
					throw new Exception($"重复的函数定义: File:{file.FileName} \tFunc{functionAST.FuncDefine.FuncName}");

				_elfHeader.Function[functionAST.FuncDefine.FuncName] = functionAST;
			}
		}

		_runtime.Entry = _elfHeader.Start;
	}
}