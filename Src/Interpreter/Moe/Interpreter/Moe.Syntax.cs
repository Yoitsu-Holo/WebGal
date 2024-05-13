using WebGal.Extend.Collections;
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
				if (statement.IsCodeblock == false && statement.Tokens[0].Type == TokenType.Function)
				{
					if (temp.CodeBlock.Count == 0)
						temp.CodeBlock.Add(statement);
					else
						throw new Exception(Logger.LogMessage(""));
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
						throw new Exception(Logger.LogMessage(""));
				}
				else
					throw new Exception(Logger.LogMessage(""));
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
				Header = ParseFunctionHeader(funncStatement.CodeBlock[0].Tokens.GetEnumerator()),
				Body = ParseProgram(funncStatement.CodeBlock[1])
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


		public static FunctionHeader ParseFunctionHeader(IEnumerator<Token> tokens)
		{

			Token func, varType, FuncName;

			if (tokens.MoveNext() == false)
				throw new Exception(Logger.LogMessage("无函数定义语法"));
			func = tokens.Current;
			if (func.Type != TokenType.Function)
				throw new Exception(Logger.LogMessage("错误的函数定义语法"));

			if (tokens.MoveNext() == false)
				throw new Exception(Logger.LogMessage($"无函数返回值类型"));
			varType = tokens.Current;
			if (varType.Type != TokenType.VarType)
				throw new Exception(Logger.LogMessage($"错误的函数返回值类型: {varType}"));

			if (tokens.MoveNext() == false)
				throw new Exception(Logger.LogMessage($"无函数名称"));
			FuncName = tokens.Current;
			if (FuncName.Type != TokenType.FuncName)
				throw new Exception(Logger.LogMessage($"错误的函数名称: {FuncName}"));

			FunctionHeader header = new()
			{
				ReturnType = varType.Value switch
				{
					"void" => MoeVariableType.Void,
					"int" => MoeVariableType.Int,
					"double" => MoeVariableType.Double,
					"string" => MoeVariableType.String,
					_ => MoeVariableType.Error,
				},
				FuncName = FuncName.Value,
				CallParam = [],
			};

			if (tokens.MoveNext() == false)
				throw new Exception(Logger.LogMessage($"无函数参数列表"));
			if (tokens.Current.Type != TokenType.LeftParen)
				throw new Exception(Logger.LogMessage($"错误的函数参数列表 {tokens.Current}"));

			List<Token> param = [];
			bool close = false;
			while (tokens.MoveNext())
			{
				Token token = tokens.Current;

				if (token.Type != TokenType.VarDelimiter && token.Type != TokenType.RightParen)
					param.Add(token);
				else
				{
					if (param.Count != 0)
					{
						MoeVariable variable = ParseSingleVar(param.GetEnumerator()).Variables[0];
						header.CallParam.Add(variable);
					}
					param.Clear();
				}

				if (token.Type == TokenType.RightParen)
				{
					close = true;
					break;
				}
			}

			if (close == false)
			{
				throw new Exception(Logger.LogMessage($"未封闭的函数参数列表 {tokens.Current}"));
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
				List<Token> tokens = statement.Tokens;

				if (tokens[0].Type == TokenType.VarAccess)
				{
					node.ASTType = ASTNodeType.VariableDeclaration;
					node.VarDefine = ParseVariableDefine(statement, preWhile);
					programNode.Statements.Add(node);
				}
				else if (tokens[0].Type == TokenType.IF)
				{
					//* if条件
					if (i + 1 >= FuncStatement.CodeBlock.Count)
						throw new Exception(Logger.LogMessage("没有 if 语句"));
					ConditionalNode conditional = ParseConditional(statement, FuncStatement.CodeBlock[i + 1], preWhile);
					i++;

					node.ASTType = ASTNodeType.Conditional;
					node.IfCase = new() { If = [conditional] };
					programNode.Statements.Add(node);
				}
				else if (tokens[0].Type == TokenType.ELIF)
				{
					node = programNode.Statements[^1];

					//* else if条件
					if (node.ASTType != ASTNodeType.Conditional || node.IfCase is null)
						throw new Exception(Logger.LogMessage("没有前置 if 条件"));
					if (i + 1 >= FuncStatement.CodeBlock.Count)
						throw new Exception(Logger.LogMessage("没有 if 语句"));

					ConditionalNode conditional = ParseConditional(statement, FuncStatement.CodeBlock[i + 1], preWhile);
					i++;

					node.IfCase.If.Add(conditional);
					programNode.Statements[^1] = node;
				}
				else if (tokens[0].Type == TokenType.ELSE)
				{
					//* else 条件
					node = programNode.Statements[^1];

					if (node.ASTType != ASTNodeType.Conditional || node.IfCase is null)
						throw new Exception(Logger.LogMessage("没有前置 if 结构"));
					if (i + 1 >= FuncStatement.CodeBlock.Count)
						throw new Exception(Logger.LogMessage("没有 if 语句"));
					ConditionalNode conditional = ParseConditional(statement, FuncStatement.CodeBlock[i + 1], preWhile);
					i++;

					node.IfCase.If.Add(conditional);
					programNode.Statements[^1] = node;
				}
				else if (tokens[0].Type == TokenType.WHILE)
				{
					//* while 循环
					ConditionalNode conditional = ParseConditional(statement, FuncStatement.CodeBlock[i + 1], preWhile);
					i++;

					node.ASTType = ASTNodeType.Loop;
					node.Loop = new() { Loop = conditional };
					programNode.Statements.Add(node);
				}
				else if (tokens[0].Type == TokenType.CONTINUE || tokens[0].Type == TokenType.BREAK)
				{
					if (preWhile is null)
						throw new Exception(Logger.LogMessage("无前置 while 循环"));

					node.ASTType = ASTNodeType.LoopControl;
					node.LoopControl = new() { Loop = preWhile, };

					if (tokens[0].Type == TokenType.CONTINUE)
						node.LoopControl.ContinueFlag = true;
					if (tokens[0].Type == TokenType.BREAK)
						node.LoopControl.ContinueFlag = false;

					programNode.Statements.Add(node);
				}
				else if (tokens[0].Type == TokenType.VarName)
				{
					//* 赋值语句
					node.ASTType = ASTNodeType.Assignment;
					node.Assignment = ParseAssignment(statement, preWhile);
					programNode.Statements.Add(node);
				}
				else if (tokens[0].Type == TokenType.FuncName)
				{
					//* 函数调用
					node.ASTType = ASTNodeType.FunctionCall;
					node.FunctionCall = ParseFunctionCall(statement, preWhile);
					programNode.Statements.Add(node);
				}
				else
				{
					string error = "";
					foreach (var token in tokens)
						error += token.Value + " ";
					throw new Exception(Logger.LogMessage($"??? WTF \nLine: {tokens[0].Line} : {error}"));
				}
			}

			return programNode;
		}

		public static ConditionalNode ParseConditional(Statement statement, Statement programe, ConditionalNode? preWhile)
		{
			List<Token> tokens = statement.Tokens;

			if (tokens[0].Type != TokenType.ELSE)
				if (tokens.Count < 4 || tokens[1].Type != TokenType.LeftParen || tokens[^1].Type != TokenType.RightParen)
					throw new Exception(Logger.LogMessage("错误的条件语法"));

			//* 条件
			ConditionalNode conditional = new();

			if (tokens[0].Type != TokenType.ELSE)
				conditional.Conditional = MathExpression(new DoubleEndEnumerator<Token>(tokens[2..^1]));
			else
				conditional.Conditional.Tokens = [new() { Type = OperatorType.NUM, Number = 1, }];

			if (tokens[0].Type == TokenType.WHILE)
				conditional.Program = PraseProgram(programe, conditional);
			else
				conditional.Program = PraseProgram(programe, preWhile);

			return conditional;
		}

		public static AssignmentNode ParseAssignment(Statement statement, ConditionalNode? preWhile)
		{
			List<Token> tokens = statement.Tokens;

			//* 赋值
			AssignmentNode assignment = new();

			int demerger;
			for (demerger = 0; tokens[demerger].Type != TokenType.AssignmentOperator; demerger++) ;

			List<Token> preTokens = tokens[0..demerger];
			List<Token> expTokens = tokens[(demerger + 1)..];

			assignment.LeftVar = new()
			{
				Name = preTokens[0].Value,
			};


			DoubleEndEnumerator<Token> range = new(preTokens[1..]);
			while (range.TryGetNext(out Token? t))
			{
				if (t!.Type == TokenType.LeftRange)
					assignment.LeftVar.Index.Add(RangeExpression(range));
				else
					break;
			}

			if (expTokens.Count >= 2 && expTokens[0].Type == TokenType.FuncName)
			{
				Logger.LogInfo("Function call is todo", Global.LogLevel.Todo);
				assignment.FuncCall = new();
			}
			else if (preTokens[0].Type == TokenType.VarName)
			{
				assignment.RightExp = MathExpression(new DoubleEndEnumerator<Token>(expTokens));
			}

			return assignment;
		}

		public static FunctionCallNode ParseFunctionCall(Statement statement, ConditionalNode? preWhile)
		{
			List<Token> tokens = statement.Tokens;
			FunctionCallNode functionCall = new();

			if (tokens[0].Type != TokenType.FuncName)
				throw new Exception(Logger.LogMessage("函数调用必须以函数名开始"));
			functionCall.FunctionName = tokens[0].Value;

			List<Token> lestToken = tokens[2..^1];
			bool var = false;
			for (int i = 0; i < lestToken.Count; i++)
			{
				Token token = lestToken[i];
				if (token.Type == TokenType.VarDelimiter)
					var = false;
				else if (token.Type == TokenType.LeftRange && var)
				{
					Logger.LogInfo("Todo: 函数数组传参待实现", Global.LogLevel.Todo);
					while (lestToken[i].Type != TokenType.RightRange)
						i++;
				}
				else if (var == false)
				{
					var = true;
					if (token.Type == TokenType.VarName)
						functionCall.ParamName.Add(token.Value);
					else
						throw new Exception(Logger.LogMessage("错误的变量名称"));
				}
				else
					throw new Exception(Logger.LogMessage($"错误的函数入参列表 : {token}"));
			}
			return functionCall;
		}

		public static VariableDefineNode ParseVariableDefine(Statement statement, ConditionalNode? preWhile)
		{
			//* 变量定义
			// List<Token> tokens = statement.Tokens;
			return ParseMultiVar(statement.Tokens.GetEnumerator());
		}

		public static VariableDefineNode ParseSingleVar(IEnumerator<Token> tokens)
		{
			VariableDefineNode ret = new();
			VarTypeNode info = VarType(tokens);

			List<Token> tempToken = [];
			while (tokens.MoveNext())
			{
				Token token = tokens.Current;
				if (token.Type != TokenType.VarDelimiter)
					tempToken.Add(token);

				if (token.Type == TokenType.VarDelimiter)
					break;
			}

			MoeVariable variable = new()
			{
				Name = tempToken[0].Value,
				Access = info.Access,
				Type = info.Type,
				Dimension = VarDimension(tempToken[1..].GetEnumerator()),
			};
			ret.Variables.Add(variable);
			tempToken.Clear();
			return ret;
		}

		public static VariableDefineNode ParseMultiVar(IEnumerator<Token> tokens)
		{
			VariableDefineNode ret = new();
			VarTypeNode info = VarType(tokens);

			List<Token> tempToken = [];
			while (tokens.MoveNext())
			{
				Token token = tokens.Current;
				if (token.Type != TokenType.VarDelimiter)
					tempToken.Add(token);

				if (token.Type == TokenType.VarDelimiter)
				{
					MoeVariable variable = new()
					{
						Name = tempToken[0].Value,
						Access = info.Access,
						Type = info.Type,
						Dimension = VarDimension(tempToken[1..].GetEnumerator()),
					};
					ret.Variables.Add(variable);
					tempToken.Clear();
				}
			}
			if (tempToken.Count != 0)
			{
				MoeVariable variable = new()
				{
					Name = tempToken[0].Value,
					Access = info.Access,
					Type = info.Type,
					Dimension = VarDimension(tempToken[1..].GetEnumerator()),
				};
				ret.Variables.Add(variable);
				tempToken.Clear();
			}
			return ret;
		}

		public static VarTypeNode VarType(IEnumerator<Token> tokens)
		{
			if (tokens.MoveNext() == false)
				return new();
			Token access = tokens.Current;
			if (access.Type != TokenType.VarAccess)
				throw new Exception(Logger.LogMessage($"{access}"));

			if (tokens.MoveNext() == false)
				throw new Exception(Logger.LogMessage($""));
			Token type = tokens.Current;
			if (type.Type != TokenType.VarType)
				throw new Exception(Logger.LogMessage($"{type}"));

			VarTypeNode varInfo = new()
			{
				Access = access.Value switch
				{
					"var" => MoeVariableAccess.Variable,
					"const" => MoeVariableAccess.Const,
					"static" => MoeVariableAccess.Static,
					_ => MoeVariableAccess.Error,
				},
				Type = type.Value switch
				{
					"int" => MoeVariableType.Int,
					"double" => MoeVariableType.Double,
					"string" => MoeVariableType.String,
					_ => MoeVariableType.Error,
				},
			};

			return varInfo;
		}

		public static List<int> VarDimension(IEnumerator<Token> tokens)
		{
			int state = 0; // 0 -> [
						   // 1 -> IntNumber
						   // 2 -> ]

			List<int> dimension = [];
			while (tokens.MoveNext())
			{
				Token token = tokens.Current;
				if (state == 0 && token.Type == TokenType.LeftRange)
					state = 1;
				else if (state == 1 && token.Type == TokenType.IntNumber)
				{
					int size = Convert.ToInt32(token.Value);
					dimension.Add(size);
					state = 2;
				}
				else if (state == 2 && token.Type == TokenType.RightRange)
					state = 0;
				else
					throw new Exception(Logger.LogMessage("错误的多维数组申明： 维度大小必须为整数： " + token));
			}

			return dimension;
		}
		public static ExpressionNode MathExpression(IExtendEnumerator<Token> tokens)
		{

			List<ExpressionToken> math = [];

			int opCount = 1; // 默认最前面有一个 + ，这样可以解决 opCount = 0 不能进入名称处理的问题

			while (tokens.MoveNext())
			{
				var token = tokens.Current;
				if (token.Type == TokenType.IntNumber && opCount != 0)
				{
					math.Add(new()
					{
						Type = OperatorType.NUM,
						Number = int.Parse(token.Value),
					});
					opCount = 0;
				}
				else if (token.Type == TokenType.FloatNumber && opCount != 0)
				{
					string number = "";
					number += token.Value;
					math.Add(new()
					{
						Type = OperatorType.NUM,
						Number = double.Parse(number),
					});
					opCount = 0;
				}
				else if (token.Type == TokenType.VarName && opCount != 0)
				{
					VariableInfo variable = new() { Name = token.Value, };
					while (tokens.TryGetNext(out Token? t))
					{
						if (t!.Type == TokenType.LeftRange)
							variable.Index.Add(RangeExpression(tokens));
						else
							break;
					}
					math.Add(new()
					{
						Type = OperatorType.VAR,
						Var = variable,
					});
					opCount = 0;
				}
				else if (token.Type == TokenType.LeftRange || token.Type == TokenType.RightRange)
					break;
				else if (token.Type == TokenType.LeftParen && opCount != 0)
				{
					math.Add(new() { Type = OperatorType.LeftParen, });
					opCount = 1; // 左括号后面必须接前置运算符或者变量
				}
				else if (token.Type == TokenType.RightParen && opCount == 0)
				{
					math.Add(new() { Type = OperatorType.RightParen, });
					opCount = 0; // 右括号后面必须接运算符
				}
				else if (token.Type == TokenType.Operator)
				{
					string value = token.Value;

					if (opCount == 1 && (value != "-" || value != "~" || value != "!"))
						throw new Exception(Logger.LogMessage("前置运算符过多"));

					math.Add(new()
					{
						Type = value switch
						{
							"+" => OperatorType.ADD,
							"-" => opCount == 1 ? OperatorType.Minus : OperatorType.SUB,
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
							_ => throw new Exception(Logger.LogMessage("错误的运算符")),
						},
					});
					opCount++;
				}
				else
					throw new Exception(Logger.LogMessage("错误的算数表达式书写: " + token));
			}
			return new() { Tokens = math };
		}

		public static ExpressionNode RangeExpression(IExtendEnumerator<Token> tokens)
		{
			if (tokens.MoveNext() == false)
				throw new Exception(Logger.LogMessage($"没有更多token"));
			if (tokens.Current.Type != TokenType.LeftRange)
				throw new Exception(Logger.LogMessage($"{tokens.Current}"));

			ExpressionNode expNode = MathExpression(tokens);

			if (tokens.Current.Type != TokenType.RightRange)
				throw new Exception(Logger.LogMessage($"{tokens.Current}"));

			return expNode;
		}
	}
}