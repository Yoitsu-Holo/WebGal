using WebGal.Global;

namespace WebGal.MeoInterpreter;

public partial class MoeInterpreter
{
	// 语法解析阶段，完成后即构建AST
	public class Syntax
	{
		public static List<FuncntionNode> ParseFile(Statement fileStatement)
		{
			Statement temp = new();
			List<FuncntionNode> functions = [];

			foreach (var statement in fileStatement.CodeBlock)
			{
				if (statement.IsCodeblock == false && statement.Tokens[0].Type == ComplexTokenType.Function)
				{
					if (temp.CodeBlock.Count == 0)
						temp.CodeBlock.Add(statement);
					else
						throw new Exception(Log.LogMessage(""));
				}
				else if (statement.IsCodeblock == true)
				{
					if (temp.CodeBlock.Count == 1)
					{
						temp.CodeBlock.Add(statement);
						functions.Add(ParseFunction(temp));
						temp = new();
					}
					else
						throw new Exception(Log.LogMessage(""));
				}
				else
					throw new Exception(Log.LogMessage(""));
			}

			return functions;
		}

		public static FuncntionNode ParseFunction(Statement funncStatement)
		{
			// function:
			// code block[0]: func header
			// code block[0]: func body
			FuncntionNode funcntionNode = new()
			{
				FuncHeader = ParseFunctionHeader(funncStatement.CodeBlock[0].Tokens),
				FuncBody = ParseProgram(funncStatement.CodeBlock[1])
			};

			return funcntionNode;
		}

		public static ProgramNode ParseProgram(Statement programeStatement)
		{
			ProgramNode programNode = new();

			// foreach (var statement in programeStatement.CodeBlock)
			programNode = PraseProgram(programeStatement, null);

			return programNode;
		}


		public static FunctionHeader ParseFunctionHeader(List<ComplexToken> tokens)
		{
			FunctionHeader header = new();

			if (tokens[0].Type != ComplexTokenType.Function)
				throw new Exception(Log.LogMessage("错误的函数定义语法"));

			if (tokens.Count < 5)
				throw new Exception(Log.LogMessage("不完整的函数定义"));

			if (tokens[1].Type != ComplexTokenType.VarType)
				throw new Exception(Log.LogMessage("错误的函数返回值类型: " + tokens[1].Type + " " + tokens[1].Tokens[0].Value));

			if (tokens[2].Type != ComplexTokenType.FuncName)
				throw new Exception(Log.LogMessage("错误的函数名称"));

			if (tokens[3].Type != ComplexTokenType.LeftParen || tokens[^1].Type != ComplexTokenType.RightParen)
				throw new Exception(Log.LogMessage("错误的函数参数列表"));

			header = new()
			{
				ReturnType = tokens[1].Tokens[0].Value switch
				{
					"void" => MoeVariableType.Void,
					"int" => MoeVariableType.Int,
					"double" => MoeVariableType.Double,
					"string" => MoeVariableType.String,
					_ => MoeVariableType.Error,
				},
				FuncName = tokens[2].Tokens[0].Value,
				CallType = [],
			};

			int start = 4;
			for (int end = 4; end < tokens.Count; end++)
			{
				if (tokens[end].Type != ComplexTokenType.VarDelimiter && tokens[end].Type != ComplexTokenType.RightParen)
					continue;

				MoeVariable variable = ParseSingleVar(tokens[start..end]).Variables[0];
				if (variable.Type != MoeVariableType.Void)
					header.CallType.Add(variable);

				start = end + 1;
			}

			return header;
		}

		public static ProgramNode PraseProgram(Statement FuncStatement, ConditionalNode? preWhile)
		{
			ProgramNode programNode = new();

			for (int i = 0; i < FuncStatement.CodeBlock.Count; i++)
			{
				Statement statement = FuncStatement.CodeBlock[i];
				ASTNode node = new();
				List<ComplexToken> tokens = statement.Tokens;

				if (tokens[0].Type == ComplexTokenType.VarAccess)
				{
					node.ASTType = ASTNodeType.VariableDeclaration;
					node.VarDefine = ParseVariableDefine(statement, preWhile);
					programNode.Statements.Add(node);
				}
				else if (tokens[0].Type == ComplexTokenType.IF)
				{
					//* if条件
					if (i + 1 >= FuncStatement.CodeBlock.Count)
						throw new Exception(Log.LogMessage("没有 if 语句"));
					ConditionalNode conditional = ParseConditional(statement, FuncStatement.CodeBlock[i + 1], preWhile);
					i++;

					node.ASTType = ASTNodeType.Conditional;
					node.IfCase = new() { If = [conditional] };
					programNode.Statements.Add(node);
				}
				else if (tokens[0].Type == ComplexTokenType.ELIF)
				{
					node = programNode.Statements[^1];

					//* else if条件
					if (node.ASTType != ASTNodeType.Conditional || node.IfCase is null)
						throw new Exception(Log.LogMessage("没有前置 if 条件"));
					if (i + 1 >= FuncStatement.CodeBlock.Count)
						throw new Exception(Log.LogMessage("没有 if 语句"));

					ConditionalNode conditional = ParseConditional(statement, FuncStatement.CodeBlock[i + 1], preWhile);
					i++;

					node.IfCase.If.Add(conditional);
					programNode.Statements[^1] = node;
				}
				else if (tokens[0].Type == ComplexTokenType.ELSE)
				{
					//* else 条件
					node = programNode.Statements[^1];

					if (node.ASTType != ASTNodeType.Conditional || node.IfCase is null)
						throw new Exception(Log.LogMessage("没有前置 if 结构"));
					if (i + 1 >= FuncStatement.CodeBlock.Count)
						throw new Exception(Log.LogMessage("没有 if 语句"));
					ConditionalNode conditional = ParseConditional(statement, FuncStatement.CodeBlock[i + 1], preWhile);
					i++;

					node.IfCase.If.Add(conditional);
					programNode.Statements[^1] = node;
				}
				else if (tokens[0].Type == ComplexTokenType.WHILE)
				{
					//* while 循环
					ConditionalNode conditional = ParseConditional(statement, FuncStatement.CodeBlock[i + 1], preWhile);
					i++;

					node.ASTType = ASTNodeType.Loop;
					node.Loop = new() { Loop = conditional };
					programNode.Statements.Add(node);
				}
				else if (tokens[0].Type == ComplexTokenType.CONTINUE || tokens[0].Type == ComplexTokenType.BREAK)
				{
					if (preWhile is null)
						throw new Exception(Log.LogMessage("无前置 while 循环"));

					node.ASTType = ASTNodeType.LoopControl;
					node.LoopControl = new() { Loop = preWhile, };

					if (tokens[0].Type == ComplexTokenType.CONTINUE)
						node.LoopControl.ContinueFlag = true;
					if (tokens[0].Type == ComplexTokenType.BREAK)
						node.LoopControl.ContinueFlag = false;

					programNode.Statements.Add(node);
				}
				else if (tokens[0].Type == ComplexTokenType.VarName)
				{
					//* 赋值语句
					node.ASTType = ASTNodeType.Assignment;
					node.Assignment = ParseAssignment(statement, preWhile);
					programNode.Statements.Add(node);
				}
				else if (tokens[0].Type == ComplexTokenType.FuncName)
				{
					//* 函数调用
					node.ASTType = ASTNodeType.FunctionCall;
					node.FunctionCall = ParseFunctionCall(statement, preWhile);
					programNode.Statements.Add(node);
				}
				else
				{
					string error = "";
					foreach (var ComplexToken in tokens)
						foreach (var token in ComplexToken.Tokens)
							error += token.Value + " ";
					throw new Exception(Log.LogMessage($"??? WTF \nLine: {tokens[0].Line} : {error}"));
				}
			}

			return programNode;
		}

		public static ConditionalNode ParseConditional(Statement statement, Statement programe, ConditionalNode? preWhile)
		{
			List<ComplexToken> tokens = statement.Tokens;

			if (tokens[0].Type != ComplexTokenType.ELSE)
				if (tokens.Count < 4 || tokens[1].Type != ComplexTokenType.LeftParen || tokens[^1].Type != ComplexTokenType.RightParen)
					throw new Exception(Log.LogMessage("错误的条件语法"));

			//* 条件
			ConditionalNode conditional = new();

			if (tokens[0].Type != ComplexTokenType.ELSE)
				conditional.Conditional.Expressions = MathExpression(tokens[2..^1]);

			if (tokens[0].Type == ComplexTokenType.WHILE)
				conditional.Program = PraseProgram(programe, conditional);
			else
				conditional.Program = PraseProgram(programe, preWhile);


			return conditional;
		}

		public static AssignmentNode ParseAssignment(Statement statement, ConditionalNode? preWhile)
		{
			List<ComplexToken> tokens = statement.Tokens;

			//* 赋值
			AssignmentNode assignment = new();

			int demerger;
			for (demerger = 0; tokens[demerger].Type != ComplexTokenType.AssignmentOperator; demerger++) ;

			List<ComplexToken> preTokens = tokens[0..demerger];
			List<ComplexToken> expTokens = tokens[(demerger + 1)..];

			assignment.LeftVar = new()
			{
				Name = preTokens[0].Tokens[0].Value,
				Index = (preTokens[^1].Type == ComplexTokenType.VarRange) ? VarSize(preTokens[^1]) : [1],
			};


			if (expTokens.Count >= 2 && expTokens[0].Type == ComplexTokenType.FuncName)
			{
				Log.LogInfo("Function call is todo", Global.LogLevel.Todo);
				assignment.FuncCall = new();
			}
			else if (preTokens[0].Type == ComplexTokenType.VarName)
			{
				assignment.MathExp = new() { Expressions = MathExpression(expTokens), };
			}

			return assignment;
		}

		public static FunctionCallNode ParseFunctionCall(Statement statement, ConditionalNode? preWhile)
		{
			List<ComplexToken> tokens = statement.Tokens;
			FunctionCallNode functionCall = new();

			if (tokens[0].Type != ComplexTokenType.FuncName)
				throw new Exception(Log.LogMessage("函数调用必须以函数名开始"));
			functionCall.FunctionName = tokens[0].Tokens[0].Value;

			List<ComplexToken> lestToken = tokens[2..^1];
			bool var = false;
			foreach (var token in lestToken)
			{
				if (token.Type == ComplexTokenType.VarDelimiter)
					var = false;
				else if (token.Type == ComplexTokenType.VarRange && var)
					Log.LogInfo("Todo: 函数数组传参待实现", Global.LogLevel.Todo);
				else if (var == false)
				{
					var = true;
					if (token.Type == ComplexTokenType.VarName)
						functionCall.ParamName.Add(token.Tokens[0].Value);
					else
						throw new Exception(Log.LogMessage("错误的变量名称"));
				}
				else
					throw new Exception(Log.LogMessage($"错误的函数入参列表 : {token}"));
			}
			return functionCall;
		}

		public static VariableDefineNode ParseVariableDefine(Statement statement, ConditionalNode? preWhile)
		{
			//* 变量定义
			List<ComplexToken> tokens = statement.Tokens;
			return ParseMultiVar(tokens);
		}

		public static VariableDefineNode ParseSingleVar(List<ComplexToken> tokens)
		{
			VariableDefineNode varNode = ParseMultiVar(tokens);
			if (varNode.Variables.Count > 1)
				throw new Exception(Log.LogMessage("错误的定义多个变量"));
			return varNode;
		}

		public static VariableDefineNode ParseMultiVar(List<ComplexToken> tokens)
		{
			VariableDefineNode ret = new();
			if (tokens.Count == 0)
			{
				ret.Variables.Add(new());
				return ret;
			}

			if (tokens.Count < 3)
				throw new Exception(Log.LogMessage("变量定义参数数量过少"));

			VarTypeNode info = VarType(tokens[0..2]);

			var lestTokens = tokens[2..];
			int pos = 0;
			List<ComplexToken> tempToken = [];
			while (pos < lestTokens.Count)
			{
				if (lestTokens[pos].Type != ComplexTokenType.VarDelimiter)
					tempToken.Add(lestTokens[pos]);

				if (pos + 1 == lestTokens.Count || lestTokens[pos].Type == ComplexTokenType.VarDelimiter)
				{
					MoeVariable variable = new()
					{
						Name = tempToken[0].Tokens[0].Value,
						Dimension = (tempToken[^1].Type == ComplexTokenType.VarRange) ? VarSize(tempToken[^1]) : [1],
						Access = info.Access,
						Type = info.Type,
					};

					int size = 1;
					foreach (var item in variable.Dimension)
						size *= item;
					variable.Dimension.Add(size);
					ret.Variables.Add(variable);

					tempToken = [];
				}
				pos++;
			}
			return ret;
		}

		public static VarTypeNode VarType(List<ComplexToken> tokens)
		{
			if (tokens[0].Type != ComplexTokenType.VarAccess || tokens[1].Type != ComplexTokenType.VarType)
				throw new Exception(Log.LogMessage(""));

			VarTypeNode varInfo = new()
			{
				Access = tokens[0].Tokens[0].Value switch
				{
					"var" => MoeVariableAccess.Variable,
					"const" => MoeVariableAccess.Const,
					"static" => MoeVariableAccess.Static,
					_ => MoeVariableAccess.Error,
				},
				Type = tokens[1].Tokens[0].Value switch
				{
					"int" => MoeVariableType.Int,
					"double" => MoeVariableType.Double,
					"string" => MoeVariableType.String,
					_ => MoeVariableType.Error,
				},
			};

			return varInfo;
		}

		public static List<int> VarSize(ComplexToken token)
		{
			List<int> varDimension = [];
			int varSize = 1;

			if (token.Type != ComplexTokenType.VarRange)
				throw new Exception(Log.LogMessage("错误的多维数组申明： 未声明数组大小： " + token));

			foreach (var ssize in token.Tokens)
			{
				if (ssize.Type == SimpleTokenType.Number)
				{
					int size = Convert.ToInt32(ssize.Value);
					varSize *= size;
					varDimension.Add(size);
				}
				else
					throw new Exception(Log.LogMessage("错误的多维数组申明： 维度大小必须为整数： " + ssize));
			}
			return varDimension;
		}

		public static List<ExpressionNode> MathExpression(List<ComplexToken> tokens)
		{
			List<ExpressionNode> math = [];

			int opCount = 1; // 默认最前面有一个 + ，这样可以解决 opCount = 0 不能进入名称处理的问题
			for (int i = 0; i < tokens.Count; i++)
			{
				if (tokens[i].Type == ComplexTokenType.IntNumber && opCount != 0)
				{
					math.Add(new()
					{
						Type = OperatorType.NUM,
						Number = int.Parse(tokens[i].Tokens[0].Value),
					});
					opCount = 0;
				}
				else if (tokens[i].Type == ComplexTokenType.FloatNumber && opCount != 0)
				{
					math.Add(new()
					{
						Type = OperatorType.NUM,
						Number = double.Parse(tokens[i].Tokens[0].Value),
					});
					opCount = 0;
				}
				else if (tokens[i].Type == ComplexTokenType.VarName && opCount != 0)
				{
					VariableInfo variable = new() { Name = tokens[i].Tokens[0].Value, };
					if (i + 1 < tokens.Count && tokens[i + 1].Type == ComplexTokenType.VarRange)
					{
						variable.Index = VarSize(tokens[i + 1]);
						i++;
					}
					math.Add(new()
					{
						Type = OperatorType.VAR,
						Var = variable,
					});
					opCount = 0;
				}
				else if (tokens[i].Type == ComplexTokenType.LeftParen && opCount != 0)
				{
					for (int j = tokens.Count - 1; j >= 0; j--)
						if (tokens[j].Type == ComplexTokenType.RightParen)
						{
							math.Add(new()
							{
								Type = OperatorType.EXP,
								Expressions = MathExpression(tokens[(i + 1)..j]),
							});
							i = j;
							opCount = 0;
							break;
						}
					if (opCount != 0)
						throw new Exception(Log.LogMessage("非配对的括号组"));
					opCount = 0;
				}
				else if (tokens[i].Type == ComplexTokenType.Operator)
				{
					if (opCount == 1 && (tokens[i].Tokens[0].Value != "-" || tokens[i].Tokens[0].Value != "~"))
						throw new Exception(Log.LogMessage("前置运算符过多"));

					string value = tokens[i].Tokens[0].Value;

					math.Add(new()
					{
						Type = value switch
						{
							"+" => OperatorType.ADD,
							"-" => OperatorType.SUB,
							"*" => OperatorType.MUL,
							"/" => OperatorType.DIV,
							"%" => OperatorType.MOD,
							"^^" => OperatorType.POW,
							"<<" => OperatorType.SHL,
							">>" => OperatorType.SHR,

							"==" => OperatorType.EQ,
							"!=" => OperatorType.NEQ,
							">=" => OperatorType.EGT,
							"<=" => OperatorType.ELT,
							">" => OperatorType.GT,
							"<" => OperatorType.LT,

							"&&" => OperatorType.AND,
							"||" => OperatorType.OR,
							"!" => OperatorType.NOT,

							"&" => OperatorType.bAND,
							"|" => OperatorType.bOR,
							"~" => OperatorType.bNOT,

							"^" => OperatorType.XOR,
							_ => throw new Exception(Log.LogMessage("错误的运算符")),
						},
					});
					opCount++;
				}
				else
					throw new Exception(Log.LogMessage("错误的算数表达式书写: " + tokens[i]));
			}
			return math;
		}
	}
}