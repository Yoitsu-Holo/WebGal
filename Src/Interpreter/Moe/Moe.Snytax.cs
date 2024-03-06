using System.Linq.Expressions;

namespace WebGal.MeoInterpreter;

public partial class MoeInterpreter
{
	// 语法解析阶段，完成后即构建AST

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
		public VarTypeNode Info = new();       // 变量信息
		public List<(string, int)> Variable = [];   // 变量组
	}

	public class FunctionDeclarationNode
	{
		public string FileName = "main.moe";
		public int FileLine = 0;

		public string FunctionName = "main";
		public MoeBasicType ReturnType;
		public List<MoeVariable> CallType = [];
	}

	public class MathExpressionNode
	{
		public MathType Type = MathType.Void;
		public List<MathExpressionNode> Expressions = [];
		public SingleToken token = new();
	}

	public class LogicExpressionNode
	{
		public LogicType Type = LogicType.Void;
		public List<LogicExpressionNode> Expressions = [];
		public SingleToken token = new();
	}

	public class FunctionCallNode
	{
		public string FunctionName = "";
		public List<string> ParamName = [];
	}

	public class AssignmentNode
	{
		public string LeftVarName = "";
		public ASTNodeType RightType = ASTNodeType.Void;

		public MathExpressionNode? MathExpressions;
		public LogicExpressionNode? LogicExpressions;
		public FunctionCallNode? FunctionCalls;
		public string str = "";
	}

	public class ConditionalNode
	{
		public LogicExpressionNode Conditional = new();
		public ProgramNode Program = new();
	}

	public class IfCaseNode
	{
		public List<ConditionalNode> If = [];
	}

	public class LoopNode
	{
		public ConditionalNode Loop = new();
	}

	public class ASTNode // 可解释单元
	{
		public ASTNodeType ASTType = ASTNodeType.Void;

		// public ArithmeticExpressionNode? ArithmeticException;   // 算数表达式	不可单独解释
		// public LogicExpressionNode? LogicExpression;            // 逻辑表达式	不可单独解释
		// public BitwiseExpressionNode? BitwiseExpression;        // 位运算表达式	不可单独解释
		public VariableDefineNode? VarDefine;   // 变量定义
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
			else if (ASTType == ASTNodeType.VariableDeclaration && VarDefine is not null)
			{
				ret += "Access: " + VarDefine.Info.Access + " Type: " + VarDefine.Info.Type + "    ";
				foreach (var (name, size) in VarDefine.Variable)
					ret += "[" + name + ": " + size + "], ";
				ret += "\n";
			}
			else if (ASTType == ASTNodeType.FunctionCall && FunctionCall is not null)
			{
				ret += "Function Call: " + FunctionCall.FunctionName + ", Param List:";
				foreach (var param in FunctionCall.ParamName)
					ret += param + " ";
				if (FunctionCall.ParamName.Count == 0)
					ret += "Void";
				ret += "\n";
			}
			else if (ASTType == ASTNodeType.Assignment && Assignment is not null)
			{
				ret += Assignment.LeftVarName + " = ";
				if (Assignment.RightType == ASTNodeType.MathExpression && Assignment.MathExpressions is not null)
				{
					foreach (var item in Assignment.MathExpressions.Expressions)
						ret += item.token.Value + "";
				}
				else if (Assignment.RightType == ASTNodeType.LogicExpression && Assignment.LogicExpressions is not null)
				{
					foreach (var item in Assignment.LogicExpressions.Expressions)
						ret += item.token.Value + " ";
				}
				ret += "\n";
			}
			else if (ASTType == ASTNodeType.Conditional && IfCase is not null)
			{
				foreach (var ifcase in IfCase.If)
				{
					ret += "ifcase: ";
					foreach (var sExp in ifcase.Conditional.Expressions)
						ret += (sExp.Type != LogicType.Void ? sExp.Type : sExp.token.Value) + " ";
					ret += "\n";
					ret += ifcase.Program.ToString();
				}
			}
			else if (ASTType == ASTNodeType.Loop && Loop is not null)
			{
				ret += "while: ";
				foreach (var sExp in Loop.Loop.Conditional.Expressions)
					ret += (sExp.Type != LogicType.Void ? sExp.Type : sExp.token.Value) + " ";
				ret += Loop.Loop.Program.ToString();
			}
			else if (ASTType == ASTNodeType.Program && CodeBlock is not null)
				ret += CodeBlock.ToString();
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



	public class Snytax
	{
		public HashSet<string> ArithmeticOperatorSet = [
			"+","-","*","/","%","^^",
		];
		public HashSet<string> LogicOperatorSet = [
			"==","!=",">=","<=",">","<",
			"&&","||","^","!",
		];
		public HashSet<string> BitwiseOperatorSet = [
			"&","|","^","~",
			"<<",">>",
		];
		public ProgramNode ProgramBuild(Statement FuncStatement)
		{
			ProgramNode programNode = new();

			foreach (var statement in FuncStatement.Statements)
			{
				var tokens = statement.Tokens;
				ASTNode node = new();

				// !
				// foreach (var item in tokens)
				// 	Console.Write(item.Value + " ");
				// Console.WriteLine();

				if (tokens[0].Type == TokenType.Keyword && tokens[0].Value == "var")
				{
					//* 变量定义
					node.ASTType = ASTNodeType.VariableDeclaration;
					node.VarDefine = MultiVarDec(tokens);
					programNode.Statements.Add(node);

				}
				else if (tokens[0].Type == TokenType.Keyword && tokens[0].Value == "func")
				{
					//todo 函数定义
					node.ASTType = ASTNodeType.FunctionDeclaration;
				}
				else if (tokens[0].Type == TokenType.Keyword && tokens[0].Value == "if")
				{
					//* if条件
					node.ASTType = ASTNodeType.Conditional;
					if (tokens.Count < 4 || tokens[1].Value != "(" || tokens[^1].Value != ")")
						throw new Exception("错误的 if 条件语法");

					node.IfCase = new();

					ConditionalNode conditional = new();
					conditional.Conditional.Expressions = LogicExpression(tokens[2..^1]);
					conditional.Program = ProgramBuild(statement);
					node.IfCase.If.Add(conditional);

					programNode.Statements.Add(node);
				}
				else if (tokens[0].Type == TokenType.Keyword && tokens[0].Value == "elif")
				{
					//* else if条件
					node = programNode.Statements[^1];
					if (node.ASTType != ASTNodeType.Conditional || node.IfCase is null)
						throw new Exception("没有前置 if 条件");
					if (tokens.Count < 4 || tokens[1].Value != "(" || tokens[^1].Value != ")")
						throw new Exception("错误的 if 条件语法");

					ConditionalNode conditional = new();
					conditional.Conditional.Expressions = LogicExpression(tokens[2..^1]);
					conditional.Program = ProgramBuild(statement);
					node.IfCase.If.Add(conditional);
					programNode.Statements[^1] = node;
				}
				else if (tokens[0].Type == TokenType.Keyword && tokens[0].Value == "else")
				{
					//* else 条件
					node = programNode.Statements[^1];
					if (node.ASTType != ASTNodeType.Conditional || node.IfCase is null)
						throw new Exception("没有前置 if/elif 结构");

					ConditionalNode conditional = new();
					conditional.Conditional.Expressions = [];
					conditional.Program = ProgramBuild(statement);
					node.IfCase.If.Add(conditional);
					programNode.Statements[^1] = node;
				}
				else if (tokens[0].Type == TokenType.Keyword && tokens[0].Value == "while")
				{
					//* while 循环
					node.ASTType = ASTNodeType.Loop;
					if (tokens.Count < 4 || tokens[1].Value != "(" || tokens[^1].Value != ")")
						throw new Exception("错误的 while 循环语法");

					ConditionalNode conditional = new();
					conditional.Conditional.Expressions = LogicExpression(tokens[2..^1]);
					conditional.Program = ProgramBuild(statement);

					node.Loop = new()
					{
						Loop = conditional
					};

					programNode.Statements.Add(node);
				}
				else if (tokens[0].Type == TokenType.Keyword && tokens[0].Value == "continue" || tokens[0].Value == "break")
				{
					Console.WriteLine("continue/break is todo");
				}
				else if (tokens[0].Type == TokenType.Name && tokens.Count >= 2 && tokens[1].Type == TokenType.Operator && tokens[1].Value == "=")
				{
					//* 赋值语句
					node.ASTType = ASTNodeType.Assignment;
					node.Assignment = new()
					{
						LeftVarName = tokens[0].Value,
					};
					List<SingleToken> leastTokens = tokens[2..];

					if (leastTokens.Count >= 2 && leastTokens[0].Type == TokenType.Name && leastTokens[1].Value == "(" && leastTokens[^1].Value == ")")
					{
						Console.WriteLine("Function call is todo");
						node.Assignment.FunctionCalls = new();
					}
					else
					{
						foreach (var item in leastTokens)
						{
							if (item.Type == TokenType.Operator)
							{
								if (LogicOperatorSet.Contains(item.Value))
								{
									node.Assignment.LogicExpressions = new()
									{
										Expressions = LogicExpression(leastTokens)
									};
									node.Assignment.RightType = ASTNodeType.LogicExpression;
								}
								break;
							}
						}
						node.Assignment.MathExpressions = new()
						{
							Expressions = MathExpression(leastTokens)
						};
						node.Assignment.RightType = ASTNodeType.MathExpression;
					}
					programNode.Statements.Add(node);
				}
				else if (tokens[0].Type == TokenType.Name && tokens.Count > 2 && tokens[1].Value == "(" && tokens[^1].Value == ")")
				{
					//* 函数调用
					node.ASTType = ASTNodeType.FunctionCall;
					node.FunctionCall = new()
					{
						FunctionName = tokens[0].Value
					};
					List<SingleToken> lestToken = tokens[2..^1];
					bool var = false;
					foreach (var token in lestToken)
					{
						if (token.Type == TokenType.Delimiter && token.Value == ",")
							var = false;
						else if (var == false)
						{
							var = true;
							if (token.Type == TokenType.Name)
								node.FunctionCall.ParamName.Add(token.Value);
							else
								throw new Exception("错误的变量名称");
						}
						else
							throw new Exception("错误的函数入参列表");
					}
					programNode.Statements.Add(node);
				}
				else if (tokens.Count == 1 && tokens[0].Value == "Null")
				{
					Console.WriteLine("Null");
				}
				else
				{
					string error = "";
					foreach (var item in tokens)
						error += item.Value + " ";
					throw new Exception("??? WTF " + error);
				}
			}

			return programNode;
		}

		public VariableDefineNode SingleVarDec(List<SingleToken> tokens)
		{
			VariableDefineNode varNode = MultiVarDec(tokens);
			return varNode;
		}

		public VariableDefineNode MultiVarDec(List<SingleToken> tokens)
		{
			if (tokens.Count < 2)
				throw new Exception("变量定义参数数量过少");
			VariableDefineNode ret = new()
			{
				Info = VarType(tokens[0..2])
			};

			var lestTokens = tokens[2..];
			int pos = 0;
			List<SingleToken> tempToken = [];
			while (pos < lestTokens.Count)
			{
				if (lestTokens[pos].Type != TokenType.Delimiter)
					tempToken.Add(lestTokens[pos]);
				if (pos + 1 == lestTokens.Count || lestTokens[pos].Type == TokenType.Delimiter)
				{
					ret.Variable.Add(VarSize(tempToken));
					tempToken = [];
				}
				pos++;
			}
			return ret;
		}

		public VarTypeNode VarType(List<SingleToken> tokens)
		{
			if (tokens[0].Type != TokenType.Keyword || tokens[1].Type != TokenType.Type)
				throw new Exception("");

			VarTypeNode varInfo = new()
			{
				Access = tokens[0].Value switch
				{
					"var" => MoeBasicAccess.Variable,
					"const" => MoeBasicAccess.Const,
					"static" => MoeBasicAccess.Static,
					_ => MoeBasicAccess.Error,
				},
				Type = tokens[1].Value switch
				{
					"int" => MoeBasicType.Int,
					"double" => MoeBasicType.Double,
					"string" => MoeBasicType.String,
					_ => MoeBasicType.Error,
				},
			};

			return varInfo;
		}

		public (string, int) VarSize(List<SingleToken> tokens)
		{
			if (tokens.Count != 1 && tokens.Count != 4)
				throw new Exception("错误的大小" + tokens.Count);

			int varSize = 1;
			string varName;
			if (tokens[0].Type == TokenType.Name)
				varName = tokens[0].Value;
			else
				throw new Exception("错误的变量名" + tokens[0].Value);

			if (tokens.Count == 4)
			{
				if (tokens[2].Type == TokenType.Number)
					varSize = Convert.ToInt32(tokens[2].Value);
				else
					throw new Exception("错误的变量大小");
			}

			return (varName, varSize);
		}

		public List<MathExpressionNode> MathExpression(List<SingleToken> tokens)
		{
			List<MathExpressionNode> arithmetic = [];

			int opCount = 1; // 默认最前面有一个 + ，这样可以解决 opCount = 0 不能进入名称处理的问题
			for (int i = 0; i < tokens.Count; i++)
			{
				if ((tokens[i].Type == TokenType.Number || tokens[i].Type == TokenType.Name) && opCount != 0)
				{
					string value = tokens[i].Value;
					if (tokens.Count > i + 2)
						if (tokens[i + 1].Type == TokenType.Delimiter && tokens[i + 1].Value == "." && tokens[i + 2].Type == TokenType.Number)
						{
							value += "." + tokens[i + 2].Value;
							i += 2;
						}

					arithmetic.Add(new()
					{
						Type = MathType.VAR,
						token = {
							Type = TokenType.Number,
							Value = value,
						}
					});
					opCount = 0;
				}
				else if (tokens[i].Type == TokenType.Operator && tokens[i].Value == "(" && opCount == 1)
				{
					for (int j = tokens.Count - 1; j >= 0; j--)
						if (tokens[j].Type == TokenType.Operator && tokens[j - 1].Value == ")")
						{
							arithmetic.Add(new()
							{
								Type = MathType.EXP,
								Expressions = MathExpression(tokens[i..(j + 1)]),
							});
							i = j;
							opCount = 0;
							break;
						}
					if (opCount != 0)
						throw new Exception("非配对的括号组");
				}
				else if (tokens[i].Type == TokenType.Operator)
				{
					if (opCount == 1 && (tokens[i].Value != "-" || tokens[i].Value != "~"))
						throw new Exception("前置运算符过多");

					arithmetic.Add(new()
					{
						Type = tokens[i].Value switch
						{
							"+" => MathType.ADD,
							"-" => MathType.SUB,
							"*" => MathType.MUL,
							"/" => MathType.DIV,
							"%" => MathType.MOD,
							"^^" => MathType.POW,
							"&" => MathType.AND,
							"|" => MathType.OR,
							"^" => MathType.XOR,
							"~" => MathType.NOT,
							"<<" => MathType.SHL,
							">>" => MathType.SHR,
							_ => MathType.Void,
						},
						token = tokens[i],
					});
					opCount++;
				}
				else
					throw new Exception("错误的算数表达式书写");
			}
			return arithmetic;
		}

		public List<LogicExpressionNode> LogicExpression(List<SingleToken> tokens)
		{
			List<LogicExpressionNode> logic = [];

			int opCount = 1; // 默认最前面有一个 + ，这样可以解决 opCount = 0 不能进入名称处理的问题
			for (int i = 0; i < tokens.Count; i++)
			{
				if ((tokens[i].Type == TokenType.Number || tokens[i].Type == TokenType.Name) && opCount != 0)
				{
					string value = tokens[i].Value;
					if (tokens.Count > i + 2)
						if (tokens[i + 1].Type == TokenType.Delimiter && tokens[i + 1].Value == "." && tokens[i + 2].Type == TokenType.Number)
						{
							value += "." + tokens[i + 2].Value;
							i += 2;
						}

					logic.Add(new()
					{
						Type = LogicType.Void,
						token = {
							Type = TokenType.Number,
							Value = value,
						}
					});
					opCount = 0;
				}
				else if (tokens[i].Type == TokenType.Operator && tokens[i].Value == "(" && opCount == 1)
				{
					for (int j = tokens.Count - 1; j >= 0; j--)
						if (tokens[j].Type == TokenType.Operator && tokens[j - 1].Value == ")")
						{
							logic.Add(new()
							{
								Type = LogicType.EXP,
								Expressions = LogicExpression(tokens[i..(j + 1)]),
							});
							i = j;
							opCount = 0;
							break;
						}
					if (opCount != 0)
						throw new Exception("非配对的括号组");
				}
				else if (tokens[i].Type == TokenType.Operator)
				{
					if (opCount == 1 && tokens[i].Value != "!")
						throw new Exception("前置运算符过多");

					logic.Add(new()
					{
						Type = tokens[i].Value switch
						{
							"==" => LogicType.EQ,
							"!=" => LogicType.NEQ,
							">" => LogicType.GT,
							"<" => LogicType.LT,
							">=" => LogicType.EGT,
							"<=" => LogicType.ELT,
							"&&" => LogicType.AND,
							"||" => LogicType.OR,
							_ => LogicType.Void,
						},
						token = tokens[i],
					});
					opCount++;
				}
				else
					throw new Exception("错误的算数表达式书写");
			}
			return logic;
		}
	}
}