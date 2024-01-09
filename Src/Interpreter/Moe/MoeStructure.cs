namespace WebGal.MeoInterpreter;


public enum MoeELF
{
	Void,
	FILE,
	TABLE,
	DATA,
	FORM,
	CODE,
	START,
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
}

public class MoeVariable
{
	public MoeBasicAccess Access;
	public MoeBasicType Type;
	public object? Obj;
	public int Size = 0; // 0 时默认非数组
}

public class MoeFunction()
{
	public string FileName = "main.moe";
	public int FileLine = 0;


	public string FunctionName = "main";
	public MoeBasicType ReturnType;
	public List<MoeVariable> CallType = [];
}
