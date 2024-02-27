namespace WebGal.MeoInterpreter;

public class ElfHeader
{
	// .file 
	public Dictionary<string, MoeFile> File = [];
	// .data
	public Dictionary<string, MoeVariable> Data = [];
	// .form
	public Dictionary<string, string> From = [];
	// .table [Auto Gen]
	public Dictionary<string, MoeFunction> Function = [];
	// .start
	public string Start = "main";
}

public enum MoeELF
{
	Void,
	FILE, TABLE, DATA,
	FORM, CODE, START,
}

public enum MoeFileType
{
	Void,
	Img_png, Img_jpg, Img_bmp,
	Audio_wav, Audio_mp3, Audio_flac, Audio_midi,
	Text_script, Text_ui,
	Bin_font, Bin_block,
}

public enum MoeBasicType
{
	Void,
	Int, Double, String,
}

public enum MoeBasicAccess
{
	Void,
	// 全局变量
	Const, Static, Variable,
	// 局部变量，几乎等同于全局的 Variable
	Partial = 0x10,
}

public class MoeFile()
{
	public string FileName = "";
	public MoeFileType FileType = MoeFileType.Void;
	public string FileURL = "";

	override public string ToString() => $"\tFileName: {FileName}, \tFileType: {FileType}, \tFileURL: {FileURL}";
}

public class MoeVariable
{
	public MoeBasicAccess Access;
	public MoeBasicType Type;
	public object? Obj;
	public int Size = 0; // 0 时默认非数组

	public override string ToString() => $"\tAccess: {Access}, \tType: {Type}, \tObj: {Obj}, \tSize: {Size}";
}

public class MoeFunction
{
	public string FileName = "main.moe";
	public int FileLine = 0;


	public string FunctionName = "main";
	public MoeBasicType ReturnType;
	public List<MoeVariable> CallType = [];

	public override string ToString()
	{
		string ret = $"\tFileName : {FileName}, \tFileLine : {FileLine}, \tFunctionName : {FunctionName}, \tReturnType : {ReturnType}\n";
		ret += $"\tCallType : {CallType.Count}";
		foreach (var call in CallType)
			ret += "\n\t" + call.ToString();
		return ret;
	}
}


// 正在解析的文件
public class InterpretFileInfo
{
	public string Name = "main"; // 正在解析的文件名称
	public int Line = 0; // 解析的文件行数
}

// 栈帧
public class MoeStackFrame
{
	// 程序运行环境
	public Dictionary<string, MoeVariable> VariableData = []; // 局部变量字典
	public InterpretFileInfo InterpreterFile = new();

	// 函数参数
	public List<MoeVariable> ParamList = []; // 函数调用传入的参数列表
	public MoeVariable ReturnData = new(); // 函数返回值
}

// 全局空间
public class MoeGlobleSpace
{
	public Dictionary<string, MoeVariable> VariableData = []; // 全局变量字典
	public InterpretFileInfo InterpretFile = new();


	public Dictionary<string, Stack<MoeStackFrame>> MowStack = []; // 函数栈，可能有多个并行的函数栈
}