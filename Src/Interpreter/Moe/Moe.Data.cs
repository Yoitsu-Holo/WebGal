using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebGal.Extend.Json;
using WebGal.Types;

namespace WebGal.MeoInterpreter;

public class ElfHeader
{
	// .file 
	public Dictionary<string, MoeFile> Files = [];
	// .data
	public Dictionary<string, MoeVariable> Datas = [];
	// .form [Audo Gen]
	public Dictionary<int, FromLayoutInfo> Forms = [];
	// .func [Auto Gen]
	public Dictionary<string, FuncntionNode> Functions = [];
	// .start
	public string Start = "main";

	public void CLear()
	{
		Files.Clear();
		Datas.Clear();
		Forms.Clear();
		Functions.Clear();
	}
}

public enum MoeELFsegment
{
	Void,
	FILE, TABLE, DATA, FORM, START,
	Error,
}

public enum MoeFileType : ulong
{
	Void = 0,
	Image = 0x000F, Audio = 0x00F0, Text = 0x0F00, Bin = 0xF000,

	Image_png = 0x1, Image_jpg = 0x2, Image_bmp = 0x4,
	Audio_wav = 0x10, Audio_mp3 = 0x20, Audio_flac = 0x40, Audio_ogg = 0x8,
	Text_script = 0x100, Text_form = 0x200, Text_opera = 0x400,
	Bin_font = 0x1000, Bin_block = 0x2000,
	Error,
}

public enum MoeVariableType
{
	Void, Int, Float, String,
	Error,
}

public enum MoeVariableAccess
{
	Const, Static, Variable, Reference,
	Error,
}

public class MoeFile()
{
	public string Name = "";
	public MoeFileType Type = MoeFileType.Void;
	public string URL = "";

	override public string ToString() => $"FileName: {Name}, \tFileType: {Type}, \tFileURL: {URL}";
}

public class MoeVariable : ICloneable
{
	public string Name = "";
	public MoeVariableAccess Access { get; private set; } = MoeVariableAccess.Const;
	public MoeVariableType Type { get; private set; } = MoeVariableType.Void;
	public object? Obj;

	private List<int> dimension = [];
	public int Size { get; private set; } = 0;
	private bool _dirty = true;

	public List<int> Dimension
	{
		private get { return dimension; }
		set
		{
			int totalSize = 1;
			if (value.Count == 0) value.Add(1);
			dimension = value;
			foreach (var size in value) totalSize *= size;
			Size = totalSize;
		}
	}

	public MoeVariable() { _dirty = true; }

	public MoeVariable(object obj)
	{
		Access = MoeVariableAccess.Const;
		Dimension = [1];
		if (obj is int ivalue)
		{
			Type = MoeVariableType.Int;
			Init();
			this[0] = ivalue;
		}
		else if (obj is float fvalue)
		{
			Type = MoeVariableType.Int;
			Init();
			this[0] = fvalue;
		}
		else if (obj is string svalue)
		{
			Type = MoeVariableType.Int;
			Init();
			this[0] = svalue;
		}
		_dirty = true;
	}

	public MoeVariable(MoeVariableAccess access, MoeVariableType type)
	{
		Type = type;
		Access = access;
		_dirty = true;
	}

	private void Init()
	{
		Obj = Type switch
		{
			MoeVariableType.Int => new int[Size],
			MoeVariableType.Float => new float[Size],
			MoeVariableType.String => new string[Size],
			_ => throw new Exception(""),
		};
		_dirty = false;
	}

	public override string ToString()
	{
		string ret = $"Name: {Name},\tAccess: {Access}: {Type},\tSize:";
		ret += (Dimension.Count < 0) ? "Error" : $"{Size} , \tDimension: [{string.Join(", ", Dimension)}]";
		return ret;
	}

	public object Clone()
	{
		if (_dirty) Init();

		MoeVariable copy = new()
		{
			Access = Access,
			Type = Type,
			Name = (string)Name.Clone(),
			Size = Size,
			Obj = Type switch
			{
				MoeVariableType.Int => new int[Size],
				MoeVariableType.Float => new float[Size],
				MoeVariableType.String => new string[Size],
				_ => throw new Exception($"Could not be a variable type {Type}"),
			}
		};

		foreach (var item in Dimension)
			copy.Dimension.Add(item);

		if (Obj is not null)
			for (int i = 0; i < Size; i++)
				copy[i] = this[i];
		return copy;
	}

	// 通常情况下使用
	public object this[List<int> index]
	{
		get
		{
			if (_dirty) Init();
			if (Obj is null) throw new Exception("Enpty Object");
			if (index.Count == 0) index.Add(0);
			if (index.Count != Dimension.Count) throw new IndexOutOfRangeException($"{ToString()} {index.Count}:{Dimension.Count}");

			int pos = 0;
			for (int i = 0; i < Dimension.Count; i++)
			{
				if (index[i] >= Dimension[i] || index[i] < 0) throw new IndexOutOfRangeException();
				pos = pos * Dimension[i] + index[i];
			}

			return Type switch
			{
				MoeVariableType.Void => throw new Exception("Unknow Error"),
				MoeVariableType.Int => ((int[])Obj)[pos],
				MoeVariableType.Float => ((float[])Obj)[pos],
				MoeVariableType.String => ((string[])Obj)[pos],
				_ => throw new Exception("Unknow Error"),
			};
		}
		set
		{
			if (_dirty) Init();
			if (Obj is null) throw new Exception("Enpty Object");
			if (index.Count == 0) index.Add(0);
			if (index.Count != Dimension.Count) throw new IndexOutOfRangeException($"{ToString()} {index.Count}:{Dimension.Count}");

			int pos = 0;
			for (int i = 0; i < Dimension.Count; i++)
			{
				if (index[i] >= Dimension[i] || index[i] < 0) throw new IndexOutOfRangeException();
				pos = pos * Dimension[i] + index[i];
			}

			switch (Type)
			{
				case MoeVariableType.Void: throw new Exception("Unknow Error");
				case MoeVariableType.Int: ((int[])Obj)[pos] = (int)value; break;
				case MoeVariableType.Float: ((float[])Obj)[pos] = (float)value; break;
				case MoeVariableType.String: ((string[])Obj)[pos] = (string)value; break;
				default: throw new Exception("Unknow Error");
			};
		}
	}

	// ref 时使用
	public object this[int index]
	{
		get
		{
			if (_dirty) Init();
			if (Obj is null) throw new Exception("Enpty Object");
			if (index < 0 || index >= Dimension[^1]) throw new IndexOutOfRangeException();

			return Type switch
			{
				MoeVariableType.Void => throw new Exception("Unknow Error"),
				MoeVariableType.Int => ((int[])Obj)[index],
				MoeVariableType.Float => ((float[])Obj)[index],
				MoeVariableType.String => ((string[])Obj)[index],
				_ => throw new Exception("Unknow Error"),
			};
		}
		set
		{
			if (_dirty) Init();
			if (Obj is null) throw new Exception("Enpty Object");
			if (index < 0 || index >= Dimension[^1]) throw new IndexOutOfRangeException();

			switch (Type)
			{
				case MoeVariableType.Void: throw new Exception("Unknow Error");
				case MoeVariableType.Int: ((int[])Obj)[index] = (int)value; break;
				case MoeVariableType.Float: ((float[])Obj)[index] = (float)value; break;
				case MoeVariableType.String: ((string[])Obj)[index] = (string)value; break;
				default: throw new Exception("Unknow Error");
			};
		}
	}

	public static implicit operator int(MoeVariable variable) => (variable.Type == MoeVariableType.Int && variable.Obj is not null) ? (int)variable[0] : 0;
	public static implicit operator float(MoeVariable variable) => (variable.Type == MoeVariableType.Float && variable.Obj is not null) ? (float)variable[0] : 0.0f;
	public static implicit operator string(MoeVariable variable) => (variable.Type == MoeVariableType.String && variable.Obj is not null) ? (string)variable[0] : "";
	public static implicit operator int[](MoeVariable variable) => (variable.Type == MoeVariableType.Int && variable.Obj is not null) ? (int[])variable.Obj : [];
	public static implicit operator float[](MoeVariable variable) => (variable.Type == MoeVariableType.Float && variable.Obj is not null) ? (float[])variable.Obj : [];
	public static implicit operator string[](MoeVariable variable) => (variable.Type == MoeVariableType.String && variable.Obj is not null) ? (string[])variable.Obj : [];

	public static implicit operator MoeVariable(int value)
	{
		MoeVariable ret = new(MoeVariableAccess.Const, MoeVariableType.Int) { Dimension = [1] };
		ret[0] = value;
		return ret;
	}
	public static implicit operator MoeVariable(float value)
	{
		MoeVariable ret = new(MoeVariableAccess.Const, MoeVariableType.Float) { Dimension = [1] };
		ret[0] = value;
		return ret;
	}
	public static implicit operator MoeVariable(string value)
	{
		MoeVariable ret = new(MoeVariableAccess.Const, MoeVariableType.String) { Dimension = [1] };
		ret[0] = value;
		return ret;
	}
}

// 栈帧
public class MoeStackFrame
{
	// 程序运行环境
	public Dictionary<string, MoeVariable> LVariable = []; // 局部变量字典
	public Stack<int> PC = []; // 程序栈内指针
	public Stack<ProgramNode> CodeBlock = []; // 当前正在运行的代码块
	public Stack<HashSet<string>> BlockVarName = []; // 代码块变量名称

	// 函数参数
	// public List<MoeVariable> ParamList = []; // 函数调用传入的参数列表，可以优化到局部变量字典中
	public MoeVariable ReturnData = new(); // 函数返回值
}

// 全局运行时空间
public class MoeRuntime
{
	public Dictionary<string, MoeVariable> Variables = []; // 全局变量字典

	public Dictionary<int, Stack<MoeStackFrame>> Tasks = []; // 任务函数栈，可能有多个并行的函数栈

	public void Clear()
	{
		Variables.Clear();
		Tasks.Clear();
	}
}

//^ ----------------------------------- Scene ------------------------------------
public record struct Behave
{
	public Behave() { Func = ""; Param = []; }

	/// <summary>
	/// 使用用户自定义的函数时，必须再函数名前加 "$" 标识。
	/// 此外，系统提供一些常用函数可供调用，这些函数不需要添加标识符。
	/// 同时可以注册函数到系统中，以简化调用时间。
	/// </summary>
	public string Func { get; set; }

	[JsonConverter(typeof(DictionaryStringObjectConverter))]
	public Dictionary<string, object> Param { get; set; }
}

public record struct Scene
{
	public Scene() { Behaves = []; }

	public int SceneID { get; set; }
	public int SceneBack { get; set; }
	public int SceneNext { get; set; }

	public List<Behave> Behaves { get; set; }
}

public record struct SceneList
{
	public SceneList() { Scenes = []; }

	public List<Scene> Scenes { get; set; }
	public int SceneId = 0;

	public readonly int NextSceneID => Scenes[SceneId].SceneNext;
	public readonly int BackSceneID => Scenes[SceneId].SceneBack;
}


//^ ----------------------------------- Form -------------------------------------

public record struct FormLayerInfo
{
	public FormLayerInfo() { Name = ""; Type = ""; Visible = Enable = true; }

	public int LayerID { get; set; }
	public string Name { get; set; }    // 设置名字
	public string Type { get; set; }

	public IVector Position { get; set; }
	public IVector Size { get; set; }
	public IVector Offset { get; set; }

	public bool Visible { get; set; }   // 可见性
	public bool Enable { get; set; }    // 功能性

	public override readonly string ToString() => JsonSerializer.Serialize(this);
}

public record struct FromLayoutInfo
{
	public int LayoutID { get; set; }
	public List<FormLayerInfo> Layers { get; set; }

	public override string ToString()
	{
		string ret = "";
		ret += LayoutID.ToString() + "\n";
		foreach (var layer in Layers)
			ret += layer.ToString() + "\n";
		return ret;
	}
}
