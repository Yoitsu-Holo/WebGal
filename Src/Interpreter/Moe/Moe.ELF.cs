using System.Text.Json;
using WebGal.API;
using WebGal.API.Data;
using WebGal.Global;
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
	public static async Task LoadELF(string MoeELF)
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
					".data" => MoeELFsegment.DATA,
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

				_elfHeader.Files[lines[0]] = new()
				{
					Name = lines[0],
					Type = lines[1] switch
					{
						"png" => MoeFileType.Image_png,
						"jpg" => MoeFileType.Image_jpg,
						"bmp" => MoeFileType.Image_bmp,

						"wav" => MoeFileType.Audio_wav,
						"mp3" => MoeFileType.Audio_mp3,
						"flac" => MoeFileType.Audio_flac,
						"midi" => MoeFileType.Audio_midi,

						"script" => MoeFileType.Text_script,
						"form" => MoeFileType.Text_form,

						"bin" => MoeFileType.Bin_font,
						"block" => MoeFileType.Bin_block,

						_ => MoeFileType.Void,
					},
					URL = lines[2],
				};
				continue;
			}

			if (elfFlag == MoeELFsegment.DATA)
			{
				Lexer lexer = new(line);
				lexer.Parse();
				VariableDefineNode multiVar = Syntax.ParseMultiVar(lexer.Tokens.GetEnumerator());

				foreach (var variable in multiVar.Variables)
					_elfHeader.Datas[variable.Name] = variable;

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
		foreach (var (_, file) in _elfHeader.Files)
		{
			FileInfo fileInfo;
			if (file.Type == MoeFileType.Text_script || file.Type == MoeFileType.Text_form)
				fileInfo = new() { Type = FileType.Script, Name = file.Name, URL = file.URL, };
			else if (file.Type == MoeFileType.Bin_font)
				fileInfo = new() { Type = FileType.Font, Name = file.Name, URL = file.URL, };
			else
				continue;
			tasks.Add(Driver.PullFileAsync(fileInfo));
		}
		await Task.WhenAll(tasks);


		// 加载完毕，将elf header中变量数据加入到全局运行空间
		foreach (var item in _elfHeader.Datas)
			_runtime.Variables[item.Key] = item.Value;

		// 扫描所有脚本
		foreach (var (_, file) in _elfHeader.Files)
		{
			if ((file.Type & MoeFileType.Text) == 0) continue;

			FileInfo fileInfo = new() { Type = FileType.Script, Name = file.Name };
			Response response = await Driver.GetScriptAsync(fileInfo);
			if (response.Type != ResponseType.Success) throw new Exception(response.Message);

			if (file.Type == MoeFileType.Text_form)
			{
				var layout = JsonSerializer.Deserialize<FromLayoutInfo>(response.Message, Global.JsonConfig.Options);
				_elfHeader.Forms[layout.LayoutID] = layout;
				continue;
			}

			Lexer lexer = new(response.Message);
			lexer.Parse();

			List<FuncntionNode> funcntions = Syntax.ParseFile(lexer.CodeStatement);

			foreach (var function in funcntions)
			{
				FunctionHeader header = function.Header;
				header.FileName = file.Name;
				if (_elfHeader.Functions.ContainsKey(header.FuncName))
					throw new Exception($"重复的函数定义: File:{file.Name} \tFunc{header.FuncName}");

				_elfHeader.Functions[header.FuncName] = function;
			}
		}
	}

	public static void FormRegister()
	{
		foreach (var (_, layout) in _elfHeader.Forms)
		{
			var layers = layout.Layers;
			int layoutID = layout.LayoutID;
			LayoutInfo layoutInfo = new() { Request = RequestType.Set, LayoutID = layoutID, };
			Driver.RegisterLayout(layoutInfo);
			foreach (var layer in layers)
			{
				LayerInfo layerInfo = new()
				{
					ID = new() { LayoutID = layoutID, LayerID = layer.LayerID, },

					Size = layer.Size,
					Position = layer.Position,
					Type = layer.Type,
				};

				Driver.RegisterLayer(new LayerBox() { Request = RequestType.Set, Attribute = layerInfo });
			}
		}
	}
}