using System.Reflection.Metadata.Ecma335;

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

public enum MoeBasicType
{
	Void,
	Int, Double, String,
	Error,
}

public enum MoeBasicAccess
{
	Void,
	Const, Static, Variable,
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
	public MoeBasicAccess Access;
	public MoeBasicType Type;
	public object? Obj;

	public int Size = 0;
	public List<int> Dimension = [];

	public override string ToString() => $"\tAccess: {Access}, \tType: {Type}, \tObj: {Obj}, \tSize: {Size}";

	public object this[List<int> index]
	{
		get
		{
			if (Obj is null) throw new Exception("Enpty Object");
			if (index.Count != Dimension.Count) throw new IndexOutOfRangeException();

			int pos = 0;
			for (int i = 0; i < Dimension.Count; i++)
			{
				if (index[i] >= Dimension[i] || index[i] < 0) throw new IndexOutOfRangeException();
				pos = pos * Dimension[i] + index[i];
			}

			return Type switch
			{
				MoeBasicType.Void => throw new Exception("Unknow Error"),
				MoeBasicType.Int => ((int[])Obj)[pos],
				MoeBasicType.Double => ((double[])Obj)[pos],
				MoeBasicType.String => ((string[])Obj)[pos],
				_ => throw new Exception("Unknow Error"),
			};
		}
		set
		{
			if (Obj is null) throw new Exception("Enpty Object");
			if (index.Count != Dimension.Count) throw new IndexOutOfRangeException();

			int pos = 0;
			for (int i = 0; i < Dimension.Count; i++)
			{
				if (index[i] >= Dimension[i] || index[i] < 0) throw new IndexOutOfRangeException();
				pos = pos * Dimension[i] + index[i];
			}

			switch (Type)
			{
				case MoeBasicType.Void:
					throw new Exception("Unknow Error");
				case MoeBasicType.Int:
					((int[])Obj)[pos] = (int)value;
					break;
				case MoeBasicType.Double:
					((double[])Obj)[pos] = (double)value;
					break;
				case MoeBasicType.String:
					((string[])Obj)[pos] = (string)value;
					break;
				default:
					throw new Exception("Unknow Error");
			};
		}
	}

	public object this[int index]
	{
		get
		{
			if (Obj is null) throw new Exception("Enpty Object");
			if (index < 0 || index >= Size) throw new IndexOutOfRangeException();

			return Type switch
			{
				MoeBasicType.Void => throw new Exception("Unknow Error"),
				MoeBasicType.Int => ((int[])Obj)[index],
				MoeBasicType.Double => ((double[])Obj)[index],
				MoeBasicType.String => ((string[])Obj)[index],
				_ => throw new Exception("Unknow Error"),
			};
		}
		set
		{
			if (Obj is null) throw new Exception("Enpty Object");
			if (index < 0 || index >= Size) throw new IndexOutOfRangeException();

			switch (Type)
			{
				case MoeBasicType.Void:
					throw new Exception("Unknow Error");
				case MoeBasicType.Int:
					((int[])Obj)[index] = (int)value;
					break;
				case MoeBasicType.Double:
					((double[])Obj)[index] = (double)value;
					break;
				case MoeBasicType.String:
					((string[])Obj)[index] = (string)value;
					break;
				default:
					throw new Exception("Unknow Error");
			};
		}
	}
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


	public Dictionary<string, Stack<MoeStackFrame>> Task = []; // 任务函数栈，可能有多个并行的函数栈
}


public enum TokenType
{
	Void,       // 空串
	Type,       // 类型
	Name,       // 名称
	Number,     // 数字
	String,     // 字符串
	Keyword,    // 关键字
	Operator,   // 运算符
	Delimiter,  // 分隔符
	CodeBlock,  // 代码块
	Error,      // 错误
}

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
			// Console.WriteLine(codeBlock.Token.Value);
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
	VariableDeclaration,    // 变量定义
	FunctionDeclaration,    // 函数定义
	MathExpression,         // 算数表达式
	LogicExpression,        // 逻辑表达式
	FunctionCall,           // 函数调用
	Assignment,             // 赋值
	Conditional,            // 条件分支
	Loop,                   // 循环
	Program,                // 程序
	Error,
}

public class VariableDefineNode
{
	// public VarTypeNode Info = new();       // 变量信息
	// public List<(string, int)> Variable = [];   // 变量组
	public List<MoeVariable> Variables = [];

	public override string ToString()
	{
		string ret = "";
		foreach (var variable in Variables)
			ret += variable + "\t";
		return ret;
	}
}

public class FunctionDefineNode
{
	public string FileName = "main.moe";
	public string FuncName = "main";

	public MoeBasicType ReturnType;
	public List<MoeVariable> CallType = [];

	public ProgramNode Program = new();

	public override string ToString()
	{
		string ret = "";
		ret += $"FileName: {FileName}\t FuncName: {FuncName}\t ReturnType: {ReturnType}\n";
		foreach (var call in CallType)
			ret += $"\t{call}";
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
		string ret = "";
		ret += "(";
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
		string ret = "";
		ret += "(";
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
			ret += $"\t{param}";
		return ret;
	}
}

public class AssignmentNode
{
	public string LeftVarName = "";
	public ASTNodeType RightType = ASTNodeType.Void;

	public MathExpressionNode? MathExpressions;
	public LogicExpressionNode? LogicExpressions;
	public FunctionCallNode? FunctionCalls;

	public override string ToString()
	{
		string ret = "";
		ret += LeftVarName + " = ";
		if (RightType == ASTNodeType.MathExpression && MathExpressions is not null)
			ret += MathExpressions;
		if (RightType == ASTNodeType.LogicExpression && LogicExpressions is not null)
			ret += LogicExpressions;
		if (RightType == ASTNodeType.FunctionCall && FunctionCalls is not null)
			ret += FunctionCalls;
		return ret;
	}
}

public class ConditionalNode
{
	public LogicExpressionNode Conditional = new();
	public ProgramNode Program = new();

	public override string ToString()
	{
		string ret = "";
		ret += $"{Conditional}\n{Program}";
		return ret;
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
		string ret = "";
		ret += "WHILE: " + Loop + "\n";
		return ret;
	}
}

public class ASTNode // 可解释单元
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
		{
			ret += "$ Void AST\n";
		}
		else if (ASTType == ASTNodeType.FunctionDeclaration && FuncDefine is not null)
		{
			// throw new Exception("函数表达未实现");
			// ret += "Function Define: ";
			// ret += "Name: " + FuncDefine.FuncName + "   R_Type: " + FuncDefine.ReturnType + "   C_Type: ";
			// foreach (var item in FuncDefine.CallType)
			// 	ret += item.Name + " ";
			// if (FuncDefine.CallType.Count == 0)
			// 	ret += "Void";
			// ret += "\n";
			// ret += FuncDefine.Program.ToString();
			// ret += "\n";
			ret += FuncDefine + "\n";
		}
		else if (ASTType == ASTNodeType.VariableDeclaration && VarDefine is not null)
		{
			// foreach (var moeVar in VarDefine.Variables)
			// {
			// 	ret += "Access: " + moeVar.Access + " Type: " + moeVar.Type + " ";
			// 	ret += " Name: " + moeVar.Name + " Size: " + moeVar.Size + "[ ";
			// 	foreach (var size in moeVar.Dimension)
			// 		ret += size + " ";
			// 	ret += "]\n";
			// }
			// ret += "\n";
			ret += VarDefine + "\n";
		}
		else if (ASTType == ASTNodeType.FunctionCall && FunctionCall is not null)
		{
			// ret += "Function Call: " + FunctionCall.FunctionName + ", Param List:";
			// foreach (var param in FunctionCall.ParamName)
			// 	ret += param + " ";
			// if (FunctionCall.ParamName.Count == 0)
			// 	ret += "Void";
			// ret += "\n";
			ret += FunctionCall + "\n";
		}
		else if (ASTType == ASTNodeType.Assignment && Assignment is not null)
		{
			// ret += Assignment.LeftVarName + " = ";
			// if (Assignment.RightType == ASTNodeType.MathExpression && Assignment.MathExpressions is not null)
			// {
			// 	foreach (var item in Assignment.MathExpressions.Expressions)
			// 		ret += item.token.Value + "";
			// }
			// else if (Assignment.RightType == ASTNodeType.LogicExpression && Assignment.LogicExpressions is not null)
			// {
			// 	foreach (var item in Assignment.LogicExpressions.Expressions)
			// 		ret += item.token.Value + " ";
			// }
			// ret += "\n";
			ret += Assignment + "\n";
		}
		else if (ASTType == ASTNodeType.Conditional && IfCase is not null)
		{
			// foreach (var ifcase in IfCase.If)
			// {
			// 	ret += "ifcase: ";
			// 	foreach (var sExp in ifcase.Conditional.Expressions)
			// 		ret += (sExp.Type != LogicType.Void ? sExp.Type : sExp.token.Value) + " ";
			// 	ret += "\n";
			// 	ret += ifcase.Program.ToString();
			// 	ret += "\n";
			// }
			ret += IfCase + "\n";
		}
		else if (ASTType == ASTNodeType.Loop && Loop is not null)
		{
			// ret += "while: ";
			// foreach (var sExp in Loop.Loop.Conditional.Expressions)
			// 	ret += (sExp.Type != LogicType.Void ? sExp.Type : sExp.token.Value) + " ";
			// ret += Loop.Loop.Program.ToString();
			// ret += "\n";
			ret += Loop + "\n";
		}
		else if (ASTType == ASTNodeType.Program && CodeBlock is not null)
			// ret += CodeBlock.ToString();
			ret += CodeBlock + "\n";
		else
			ret += ">>> error line\n";
		return ret;
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


// 内部规则结构
public class VarTypeNode
{
	public MoeBasicType Type;       // 类型
	public MoeBasicAccess Access;   // 访问属性
}

