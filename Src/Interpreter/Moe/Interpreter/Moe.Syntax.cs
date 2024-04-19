namespace WebGal.MeoInterpreter;

public partial class MoeInterpreter
{
	// 语法解析阶段，完成后即构建AST
	public class Syntax
	{
		private HashSet<string> ArithmeticOperatorSet = [
			"+","-","*","/","%","^^",
		];
		private HashSet<string> LogicOperatorSet = [
			"==","!=",">=","<=",">","<",
			"&&","||","^","!",
		];
		private HashSet<string> BitwiseOperatorSet = [
			"&","|","^","~",
			"<<",">>",
		];
		private HashSet<string> VariableType = [
			"int","double","string",
		];
		private HashSet<string> VariableAccess = [
			"var","const","static","ref,"
		];

		public ProgramNode ProgramBuild(Statement FuncStatement)
		{
			ProgramNode programNode = new();

			foreach (var statement in FuncStatement.Statements)
			{
				var tokens = statement.Tokens;

				ASTNode node = new();

				if (tokens[0].Type == TokenType.Keyword && VariableAccess.Contains(tokens[0].Value))
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

					if (tokens.Count < 5)
						throw new Exception("不完整的函数定义");
					if (tokens[1].Type != TokenType.Type)
						throw new Exception("错误的函数返回值类型: " + tokens[1].Type + " " + tokens[1].Value);
					if (tokens[2].Type != TokenType.Name)
						throw new Exception("错误的函数名称");
					if (tokens[3].Value != "(" || tokens[^1].Value != ")")
						throw new Exception("错误的函数参数列表");

					FunctionDefineNode functionDefine = new()
					{
						ReturnType = tokens[1].Value switch
						{
							"void" => MoeVariableType.Void,
							"int" => MoeVariableType.Int,
							"double" => MoeVariableType.Double,
							"string" => MoeVariableType.String,
							_ => MoeVariableType.Error,
						},
						FuncName = tokens[2].Value,
						Program = ProgramBuild(statement),
					};

					int start = 4;
					for (int end = 4; end < tokens.Count; end++)
					{
						if (tokens[end].Type != TokenType.Delimiter && !(tokens[end].Type == TokenType.Operator && tokens[end].Value == ")"))
							continue;

						MoeVariable variable = SingleVarDec(tokens[start..end]).Variables[0];
						if (variable.Type != MoeVariableType.Void)
							functionDefine.CallType.Add(variable);
						start = end + 1;
					}

					node.FuncDefine = functionDefine;

					programNode.Statements.Add(node);
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
						node.Assignment.FuncCall = new();
					}
					else
					{
						foreach (var item in leastTokens)
						{
							if (item.Type == TokenType.Operator)
							{
								if (LogicOperatorSet.Contains(item.Value))
								{
									node.Assignment.LogicExp = new()
									{
										Expressions = LogicExpression(leastTokens)
									};
									node.Assignment.RightType = ASTNodeType.LogicExpression;
								}
								break;
							}
						}
						node.Assignment.MathExp = new()
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
			if (varNode.Variables.Count > 1)
				throw new Exception("错误的定义多个变量");
			return varNode;
		}

		public VariableDefineNode MultiVarDec(List<SingleToken> tokens)
		{
			VariableDefineNode ret = new();
			if (tokens.Count == 0)
			{
				ret.Variables.Add(new());
				return ret;
			}

			if (tokens.Count < 3)
				throw new Exception("变量定义参数数量过少");

			var info = VarType(tokens[0..2]);

			var lestTokens = tokens[2..];
			int pos = 0;
			List<SingleToken> tempToken = [];
			while (pos < lestTokens.Count)
			{
				if (lestTokens[pos].Type != TokenType.Delimiter)
					tempToken.Add(lestTokens[pos]);
				if (pos + 1 == lestTokens.Count || lestTokens[pos].Type == TokenType.Delimiter)
				{

					MoeVariable variable = new()
					{
						Name = tempToken[0].Value,
						Dimension = VarSize(tempToken),
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

		public VarTypeNode VarType(List<SingleToken> tokens)
		{
			if (tokens[0].Type != TokenType.Keyword || tokens[1].Type != TokenType.Type)
				throw new Exception("");

			VarTypeNode varInfo = new()
			{
				Access = tokens[0].Value switch
				{
					"var" => MoeVariableAccess.Partial,
					"const" => MoeVariableAccess.Const,
					"static" => MoeVariableAccess.Static,
					_ => MoeVariableAccess.Error,
				},
				Type = tokens[1].Value switch
				{
					"int" => MoeVariableType.Int,
					"double" => MoeVariableType.Double,
					"string" => MoeVariableType.String,
					_ => MoeVariableType.Error,
				},
			};

			return varInfo;
		}

		public List<int> VarSize(List<SingleToken> tokens)
		{
			List<int> varDimension = [];
			int varSize = 1;

			string s = "";
			foreach (var token in tokens)
				s += token.Value + " ";

			if (tokens[0].Type != TokenType.Name)
				throw new Exception("错误的变量名称: " + tokens[0].Value);
			if (tokens.Count > 1 && (tokens[1].Value != "[" || tokens[^1].Value != "]"))
				throw new Exception("错误的多维数组申明： 错误的语法格式: " + s);
			if (tokens.Count == 3)
				throw new Exception("错误的多维数组申明： 未声明数组大小: " + s);

			if (tokens.Count > 3)
			{
				for (int i = 2; i < tokens.Count - 1; i++)
				{
					if (i % 2 == 0 && tokens[i].Type == TokenType.Number)
					{
						int size = Convert.ToInt32(tokens[i].Value);
						varSize *= size;
						varDimension.Add(size);
					}
					else if (i % 2 == 1)
					{
						if (tokens[i].Value != ":")
							throw new Exception("错误的多维数组申明： 错误的维度分隔符 " + tokens[i].Value);
					}
					else
						throw new Exception("错误的多维数组申明");
				}
			}
			else
				varDimension.Add(1);
			return varDimension;
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
				else if (tokens[i].Type == TokenType.Operator && tokens[i].Value == "(" && opCount != 0)
				{
					for (int j = tokens.Count - 1; j >= 0; j--)
						if (tokens[j].Type == TokenType.Operator && tokens[j].Value == ")")
						{
							arithmetic.Add(new()
							{
								Type = MathType.EXP,
								Expressions = MathExpression(tokens[(i + 1)..j]),
							});
							i = j;
							opCount = 0;
							break;
						}
					if (opCount != 0)
						throw new Exception("非配对的括号组");
					opCount = 0;
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
					throw new Exception("错误的算数表达式书写: " + tokens[i]);
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