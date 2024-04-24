namespace WebGal.MeoInterpreter;

public enum SimpleTokenType
{
	Void,

	VarAccess, VarType,
	Function, Retuen,
	Name, VarDelimiter,
	Number, Point, String,
	IF, ELIF, ELSE,
	WHILE, CONTINUE, BREAK,
	Operator, AssignmentOperator,

	LeftParen, RightParen,
	LeftRange, RangeDelimiter, RightRange,
	LeftCodeBlock, RightCodeBlock,

	LineEnd, EOF,

	Error,
}

public enum ComplexTokenType
{
	Void,

	VarAccess, VarType,
	Function, Return,
	VarName, VarRange, VarDelimiter,
	FuncName,

	IntNumber, FloatNumber, String,
	IF, ELIF, ELSE,
	WHILE, CONTINUE, BREAK,

	Operator, AssignmentOperator,

	LeftParen, RightParen,
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
		return new string(Line + ":" + Type + ": " + Value);
	}
}

public class ComplexToken
{
	public ComplexTokenType Type = ComplexTokenType.Void;
	public List<SimpleToken> Tokens = [];
	public int Line = 0;

	public override string ToString()
	{
		string ret = $"{Line}:{Type}\t";
		foreach (var token in Tokens)
		{
			ret += token.Value;
			ret += " ";
		}
		return ret;
	}
}

public class ProgramToken
{
	public ComplexTokenType Type = ComplexTokenType.Void;
	public string Value = "";

	public override string ToString()
	{
		return new string(Type + ": " + Value);
	}
}

public class Statement
{
	public bool IsCodeblock = false;
	public List<ComplexToken> Tokens = [];
	public List<Statement> CodeBlock = [];

	public override string ToString()
	{
		string ret = "";
		foreach (var token in Tokens)
			foreach (var rawToken in token.Tokens)
				ret += rawToken.Type + ":" + rawToken.Value + " ";
		return ret;
	}
}

//^ -----------------------------------------------------------------------------------

public enum OperatorType
{
	Void,
	POW,
	MUL, DIV, MOD,
	ADD, SUB,
	bAND, bOR, XOR, bNOT,
	SHL, SHR,

	EQ, NEQ,
	GT, LT, EGT, ELT,
	AND, OR, NOT,

	VAR, EXP,
	Error,
}

public enum ASTNodeType
{
	Void,                   // 空
	VariableDeclaration,
	MathExpression, LogicExpression,
	Conditional, Loop, LoopControl,
	Assignment,
	FunctionCall,
	Program,
	Error,
}

public class FunctionHeader
{
	public string FileName = "main.moe";
	public string FuncName = "main";

	public MoeVariableType ReturnType;
	public List<MoeVariable> CallType = [];

	public override string ToString()
	{
		string ret = $"FileName: {FileName}\t FuncName: {FuncName}\t ReturnType: {ReturnType}";
		foreach (var call in CallType)
			ret += $"\n\tParam: {call}";
		if (CallType.Count == 0)
			ret += "\n\tParam: Null";
		return ret;
	}
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

public class ExpressionNode
{
	public OperatorType Type = OperatorType.Void;
	public List<ExpressionNode> Expressions = [];
	public ProgramToken Token = new();

	public override string ToString()
	{
		string ret = "( ";
		foreach (var exp in Expressions)
		{
			if (exp.Type == OperatorType.EXP)
				ret += exp.ToString();
			else
				ret += $"{exp.Token} ";
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
	public List<int> Index = [];

	public ExpressionNode? MathExp;
	// public LogicExpressionNode? LogicExp;
	public FunctionCallNode? FuncCall;

	public override string ToString()
	{
		string ret = "";
		ret += LeftVarName + " = ";
		if (MathExp is not null)
			ret += MathExp;
		else
			throw new Exception("未初始化表达式");
		ret += "\n";
		return ret;
	}
}

public class ConditionalNode
{
	public ExpressionNode Conditional = new();
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

public class FuncntionNode
{
	public FunctionHeader FuncHeader = new();
	public ProgramNode FuncBody = new();

	public override string ToString()
	{
		string ret = "";
		ret += FuncHeader.ToString() + "\n";
		ret += FuncBody.ToString() + "\n";
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
		if (Statements.Count == 0)
			ret += "Without Program";
		return ret;
	}
}

public class ASTNode // 可解释单元，执行器唯一可接受的结构
{
	public ASTNodeType ASTType = ASTNodeType.Void;

	public VariableDefineNode? VarDefine;   // 变量定义
	public AssignmentNode? Assignment;      // 赋值表达式
	public IfCaseNode? IfCase;              // 条件分支
	public LoopNode? Loop;                  // 循环
	public LoopControlNode? LoopControl;    // 循环
	public FunctionCallNode? FunctionCall;  // 函数调用
	public ProgramNode? CodeBlock;          // 代码块

	public override string ToString()
	{
		string ret = "";
		if (ASTType == ASTNodeType.Void)
			ret += "$ Void AST\n";
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

