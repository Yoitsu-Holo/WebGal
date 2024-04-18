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
	public Dictionary<string, ASTNode> Function = [];
	// .start
	public string Start = "main";

	public void CLear()
	{
		File.Clear();
		Data.Clear();
		From.Clear();
		Function.Clear();
	}
}

public enum MoeELFsegment
{
	Void,
	FILE, TABLE, DATA,
	FORM, CODE, START,
	Error,
}

public enum MoeFileType
{
	Void,
	Img_png, Img_jpg, Img_bmp,
	Audio_wav, Audio_mp3, Audio_flac, Audio_midi,
	Text_script, Text_ui,
	Bin_font, Bin_block,
	Error,
}

public enum MoeVariableType
{
	Void,
	Int, Double, String,
	Error,
}

public enum MoeVariableAccess
{
	Void,
	Const, Static, Partial,
	Error,
}

public enum TokenType
{
	Void,
	Type, Name, Number, String,
	Keyword, Operator, Delimiter, CodeBlock,
	Error,
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
	// 默认全为连续内存数组，不同维度的访问放置在 dimension 中
	public string Name = "";
	public MoeVariableAccess Access;
	public MoeVariableType Type;
	public object? Obj;

	public int Size { get { return (Dimension.Count == 0) ? 0 : Dimension[^1]; } set { if (Dimension.Count != 0) Dimension[^1] = value; } }
	public List<int> Dimension = [];

	public override string ToString()
	{
		string ret = $"Access: {Access}, \tType: {Type}, \tName: {Name}, \tObject: {Obj}, \tSize:";
		ret += (Dimension.Count <= 0) ? "Error" : $"{Dimension[^1]} , \tDimension: [{string.Join(", ", Dimension)}]";
		return ret;
	}

	// 通常情况下使用
	public object this[List<int> index]
	{
		get
		{
			if (Obj is null) throw new Exception("Enpty Object");
			if (index.Count != Dimension.Count - 1) throw new IndexOutOfRangeException();

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
				MoeVariableType.Double => ((double[])Obj)[pos],
				MoeVariableType.String => ((string[])Obj)[pos],
				_ => throw new Exception("Unknow Error"),
			};
		}
		set
		{
			if (Obj is null) throw new Exception("Enpty Object");
			if (index.Count != Dimension.Count - 1) throw new IndexOutOfRangeException();

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
				case MoeVariableType.Double: ((double[])Obj)[pos] = (double)value; break;
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
			if (Obj is null) throw new Exception("Enpty Object");
			if (index < 0 || index >= Dimension[^1]) throw new IndexOutOfRangeException();

			return Type switch
			{
				MoeVariableType.Void => throw new Exception("Unknow Error"),
				MoeVariableType.Int => ((int[])Obj)[index],
				MoeVariableType.Double => ((double[])Obj)[index],
				MoeVariableType.String => ((string[])Obj)[index],
				_ => throw new Exception("Unknow Error"),
			};
		}
		set
		{
			if (Obj is null) throw new Exception("Enpty Object");
			if (index < 0 || index >= Dimension[^1]) throw new IndexOutOfRangeException();

			switch (Type)
			{
				case MoeVariableType.Void: throw new Exception("Unknow Error");
				case MoeVariableType.Int: ((int[])Obj)[index] = (int)value; break;
				case MoeVariableType.Double: ((double[])Obj)[index] = (double)value; break;
				case MoeVariableType.String: ((string[])Obj)[index] = (string)value; break;
				default: throw new Exception("Unknow Error");
			};
		}
	}
}

// 栈帧
public class MoeStackFrame
{
	// 程序运行环境
	public Dictionary<string, MoeVariable> VariableData = []; // 局部变量字典

	// 函数参数
	public List<MoeVariable> ParamList = []; // 函数调用传入的参数列表
	public MoeVariable ReturnData = new(); // 函数返回值
}

// 全局运行时空间
public class MoeRuntime
{
	public string Entry = "main";   // 入口函数
	public Dictionary<string, MoeVariable> Variables = []; // 全局变量字典

	public Dictionary<int, Stack<MoeStackFrame>> Tasks = []; // 任务函数栈，可能有多个并行的函数栈

	public void Clear()
	{
		Entry = "main";
		Variables.Clear();
		Tasks.Clear();
	}
}

//^ ----------------------------------- Lexer ------------------------------------

// token 类型


public class SingleToken
{
	public TokenType Type = TokenType.Void;
	public string Value = "";
	public int Line = 0;

	public override string ToString()
	{
		return new string(Line + ":" + Type.ToString() + ": " + Value + "\n");
	}
}

public class CodeBlock
{
	public SingleToken Token = new();
	public List<CodeBlock> CodeBlocks = [];

	public override string ToString()
	{
		string ret = "";
		foreach (var codeBlock in CodeBlocks)
		{
			ret += codeBlock.Token.ToString();
			if (codeBlock.Token.Type == TokenType.CodeBlock)
				ret += "{ 0x" + codeBlock.GetHashCode().ToString("X") + "\n"
				+ codeBlock.ToString()
				+ "} 0x" + codeBlock.GetHashCode().ToString("X") + "\n";
		}
		return ret;
	}
}

public class Statement
{
	public int Deep = 0;
	public List<SingleToken> Tokens = [];
	public List<Statement> Statements = [];

	public override string ToString()
	{
		string ret = "";
		foreach (var statement in Statements)
		{
			if (statement.Tokens.Count != 0)
			{
				ret += new string('\t', statement.Deep);
				foreach (var token in statement.Tokens)
					ret += token.Value + " ";
				ret += "\n";
			}
			// foreach (var state in statement.Statements)
			if (statement.Statements.Count != 0)
				ret += statement.ToString();
		}
		return ret;
	}
}


public enum MathType
{
	Void,
	POW,
	MUL, DIV, MOD,
	ADD, SUB,
	AND, OR, XOR, NOT,
	SHL, SHR,

	VAR, EXP,
	Error,
}

public enum LogicType
{
	Void,
	EQ, NEQ,
	GT, LT, EGT, ELT,
	AND, OR, NOT,

	VAR, EXP,
	Error,
}


public enum ASTNodeType
{
	Void,                   // 空
	VariableDeclaration, FunctionDeclaration,
	MathExpression, LogicExpression,
	Conditional, Loop,
	Assignment,
	FunctionCall,
	Program,
	Error,
}

public class VariableDefineNode
{
	public List<MoeVariable> Variables = [];

	public override string ToString()
	{
		string ret = "";
		foreach (var variable in Variables)
			ret += $"{variable}\n";
		return ret;
	}
}

public class FunctionDefineNode
{
	public string FileName = "main.moe";
	public string FuncName = "main";

	public MoeVariableType ReturnType;
	public List<MoeVariable> CallType = [];

	public ProgramNode Program = new();

	public override string ToString()
	{
		string ret = $"FileName: {FileName}\t FuncName: {FuncName}\t ReturnType: {ReturnType}\n\t";
		foreach (var call in CallType)
			ret += $"{call}\n";
		if (CallType.Count == 0)
			ret += "Void";
		ret += "\n";
		ret += Program;
		return ret;
	}
}

public class MathExpressionNode
{
	public MathType Type = MathType.Void;
	public List<MathExpressionNode> Expressions = [];
	public SingleToken token = new();

	public override string ToString()
	{
		string ret = "( ";
		foreach (var exp in Expressions)
		{
			if (exp.Type == MathType.EXP)
				ret += exp.ToString();
			else
				ret += $"{exp.token.Value} ";
		}
		ret += ")";
		return ret;
	}
}

public class LogicExpressionNode
{
	public LogicType Type = LogicType.Void;
	public List<LogicExpressionNode> Expressions = [];
	public SingleToken token = new();

	public override string ToString()
	{
		string ret = "( ";
		foreach (var exp in Expressions)
		{
			if (exp.Type == LogicType.EXP)
				ret += exp.ToString();
			else
				ret += $"{exp.token.Value} ";
		}
		ret += ")";
		return ret;
	}
}

public class FunctionCallNode
{
	public string FunctionName = "";
	public List<string> ParamName = [];

	public override string ToString()
	{
		string ret = "";
		ret += $"CallFunc: {FunctionName}\n";
		foreach (var param in ParamName)
			ret += $"{param}";
		return ret;
	}
}

public class AssignmentNode
{
	public string LeftVarName = "";
	public ASTNodeType RightType = ASTNodeType.Void;

	public MathExpressionNode? MathExp;
	public LogicExpressionNode? LogicExp;
	public FunctionCallNode? FuncCall;

	public override string ToString()
	{
		string ret = "";
		ret += LeftVarName + " = ";
		if (RightType == ASTNodeType.MathExpression && MathExp is not null)
			ret += MathExp;
		else if (RightType == ASTNodeType.LogicExpression && LogicExp is not null)
			ret += LogicExp;
		else if (RightType == ASTNodeType.FunctionCall && FuncCall is not null)
			ret += FuncCall;
		else throw new Exception("未初始化表达式");
		return ret;
	}
}

public class ConditionalNode
{
	public LogicExpressionNode Conditional = new();
	public ProgramNode Program = new();

	public override string ToString()
	{
		return $"{Conditional}\n{Program}";
	}
}

public class IfCaseNode
{
	public List<ConditionalNode> If = [];

	public override string ToString()
	{
		string ret = "";
		foreach (var ifcase in If)
			ret += "IF: " + ifcase + "\n";
		return ret;
	}
}

public class LoopNode
{
	public ConditionalNode Loop = new();

	public override string ToString()
	{
		return "WHILE: " + Loop + "\n";
	}
}

public class ProgramNode // 程序段（由多个并列的可解释单元组成）
{
	public List<ASTNode> Statements = [];
	public override string ToString()
	{
		string ret = "";
		foreach (var ast in Statements)
			ret += ast.ToString();
		return ret;
	}
}

public class ASTNode // 可解释单元，执行器唯一可接受的结构
{
	public ASTNodeType ASTType = ASTNodeType.Void;
	public VariableDefineNode? VarDefine;   // 变量定义
	public FunctionDefineNode? FuncDefine;  // 函数定义
	public AssignmentNode? Assignment;      // 赋值表达式
	public IfCaseNode? IfCase;              // 条件分支
	public LoopNode? Loop;                  // 循环
	public FunctionCallNode? FunctionCall;  // 函数调用
	public ProgramNode? CodeBlock;          // 代码块

	public override string ToString()
	{
		string ret = "";
		if (ASTType == ASTNodeType.Void)
			ret += "$ Void AST\n";
		else if (ASTType == ASTNodeType.FunctionDeclaration && FuncDefine is not null)
			ret += FuncDefine;
		else if (ASTType == ASTNodeType.VariableDeclaration && VarDefine is not null)
			ret += VarDefine;
		else if (ASTType == ASTNodeType.FunctionCall && FunctionCall is not null)
			ret += FunctionCall;
		else if (ASTType == ASTNodeType.Assignment && Assignment is not null)
			ret += Assignment;
		else if (ASTType == ASTNodeType.Conditional && IfCase is not null)
			ret += IfCase;
		else if (ASTType == ASTNodeType.Loop && Loop is not null)
			ret += Loop;
		else if (ASTType == ASTNodeType.Program && CodeBlock is not null)
			ret += CodeBlock;
		else
			ret += ">>> error line\n";
		return ret;
	}
}

// 内部规则结构
public class VarTypeNode
{
	public MoeVariableType Type;       // 类型
	public MoeVariableAccess Access;   // 访问属性
}

