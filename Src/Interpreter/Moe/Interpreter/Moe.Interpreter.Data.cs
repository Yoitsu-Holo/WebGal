namespace WebGal.MeoInterpreter;


public enum SimpleTokenType
{
	Void,

	Access, Type, Name, Number, String,
	Keyword, Operator, Point,

	LeftParen, RightParen,
	LeftRange, RangeDelimiter, RightRange,
	LeftCodeBlock, RightCodeBlock,
	LineEnd,

	EOF,
	Error,
}

public enum ComplexTokenType
{
	Void,

	VariableAccess, VaribaleType,
	VarName, FuncName,
	IntNumber, FloatNumber, ConstString,
	IF, ELIF, ELSE,
	WHILE, CONTINUE, BREAK,
	BothOperator, MathOperator, LogicOperator, AssignmentOperator,

	LeftParen, RightParen,
	LeftRange, RangeDelimiter, RightRange,
	LeftCodeBlock, RightCodeBlock,

	LineEnd, EOF,

	Error,
}


public class SimpleToken
{
	public SimpleTokenType Type = SimpleTokenType.Void;
	public string Value = "";
	public int Line = 0;

	public override string ToString()
	{
		return new string(Line + ":" + Type.ToString() + ": " + Value + "\n");
	}
}

// token 类型
public class ComplexToken
{
	public ComplexTokenType Type = ComplexTokenType.Void;
	public string Value = "";
	public int Line = 0;

	public override string ToString()
	{
		return new string(Line + ":" + Type.ToString() + ": " + Value + "\n");
	}
}

// 代码块
public class CodeBlock
{
	public bool IsCodeBlock = false;
	public ComplexToken Token = new();
	public List<CodeBlock> CodeBlocks = [];

	public override string ToString()
	{
		string ret = "";
		foreach (var codeBlock in CodeBlocks)
		{
			ret += codeBlock.Token.ToString();
			if (IsCodeBlock)
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
	public List<ComplexToken> Tokens = [];
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
	Conditional, Loop, LoopControl,
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
		string ret = $"FileName: {FileName}\t FuncName: {FuncName}\t ReturnType: {ReturnType}\n";
		foreach (var call in CallType)
			ret += $"\tParam: {call}\n";
		if (CallType.Count == 0)
			ret += "\tParam: Null\n";
		ret += Program;
		return ret;
	}
}

public class MathExpressionNode
{
	public MathType Type = MathType.Void;
	public List<MathExpressionNode> Expressions = [];
	public ComplexToken token = new();

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
	public ComplexToken token = new();

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
		string ret = $"CallFunc: {FunctionName}\tParamName: ";
		foreach (var param in ParamName)
			ret += $"{param} ";
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
		ret += "\n";
		return ret;
	}
}

public class ConditionalNode
{
	public LogicExpressionNode Conditional = new();
	public ProgramNode Program = new();

	public override string ToString()
	{
		return $"{Conditional} Hash:{GetHashCode()}\n{Program}";
	}
}

public class IfCaseNode
{
	public List<ConditionalNode> If = [];

	public override string ToString()
	{
		string ret = "";
		foreach (var ifcase in If)
			ret += "IF: " + ifcase;
		return ret;
	}
}

public class LoopNode
{
	public ConditionalNode Loop = new();

	public override string ToString()
	{
		return "WHILE: " + Loop;
	}
}

public class LoopControlNode
{
	public ConditionalNode Loop = new();

	public bool ContinueFlag = true;

	public override string ToString()
	{
		return (ContinueFlag ? "CONTINUE" : "BREAK") + " Hash: " + Loop.GetHashCode() + "\n";
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
	public LoopControlNode? LoopControl;                  // 循环
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
		else if (ASTType == ASTNodeType.LoopControl && LoopControl is not null)
			ret += LoopControl;
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

