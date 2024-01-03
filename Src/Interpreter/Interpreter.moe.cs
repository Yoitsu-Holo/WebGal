namespace WebGal.MeoInterpreter;


public enum MoeFileType
{
	Void,
	Img_png, Img_jpg, Img_bmp,
	Audio_wav, Audio_mp3, Audio_flac, Audio_midi,
	File_font, File_script
}

public enum MoeBasicType
{
	Void,
	Int, Float, String,
	IntArray, FloatArray, StringArray
}

public enum MoeBasicAccess
{
	Void,
	// 可访问性
	Const, Static, Variable,
	// 局部性
	Public = 0x10, Partial = 0x11
}

public class MoeFile()
{
	public string FileName = "";
	public MoeFileType FileType = MoeFileType.Void;
	public string FileURL = "";
}

public class MoeVariable
{
	public MoeBasicType Type;
	public MoeBasicAccess Access;
	public object? obj;
}

public class MoeFunction()
{
	public string FunctionName = "main";

	public string FileName = "main.moe";
	public int FileLine = 0;
	public MoeBasicType ReturnType;
	public MoeBasicType[] CallType = [];
}

/// <summary>
/// 程序加载结构描述
/// 脚本程序标准加载文件应包含如下四个部分
/// file	文件名称和 [类型，URL] 的映射表
/// table	程序函数和文件对应关系 [文件，函数名称行数]
/// data	程序数据段，包含全局变量
/// form	界面配置文件，通过json文件描述
/// start	程序开始位置
/// </summary>
public class ELFHeader
{
	// .file 
	public Dictionary<string, MoeFile> MoeFiles = [];

	// .table
	public Dictionary<string, MoeFunction> MoeFunctions = [];

	// .data
	public Dictionary<string, MoeVariable> MoeData = [];

	public Dictionary<string, string> MoeFrom = [];
	public MoeFunction MoeStart = new();
}


public class MoeInterpreter
{

}