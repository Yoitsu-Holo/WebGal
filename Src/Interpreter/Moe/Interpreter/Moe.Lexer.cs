namespace WebGal.MeoInterpreter;

/*
词法分析（Lexical Analysis）： 首先需要将代码块中的源代码分解为词法单元，例如标识符、关键字、运算符等。你可以使用词法分析器（Lexer）来实现这一步骤。
语法分析（Syntax Analysis）： 接下来，将词法单元按照语法规则组合成语法结构，例如表达式、语句、函数定义等。你可以使用语法分析器（Parser）来实现这一步骤。
语义分析（Semantic Analysis）： 在语法分析的基础上，进一步检查代码块是否符合语义规则，例如变量是否被正确声明、函数是否被正确调用等。这一步骤可以在语法分析器中进行。
构建抽象语法树（Abstract Syntax Tree，AST）： 将语法分析得到的语法结构转换为抽象语法树，以便后续的处理和分析。抽象语法树可以方便地表示代码的结构和逻辑关系。
错误处理（Error Handling）： 在分析过程中，需要及时发现和处理语法错误、词法错误以及语义错误，并给出相应的提示和反馈信息。
*/


public partial class MoeInterpreter
{
	// 词法解析阶段，完成后即构建代码块和语句信息

	public class Lexer
	{
		public HashSet<string> AccessSet = [
			"var","static","const",
		];

		public HashSet<string> TypeSet = [
			"void","int","double","string","dictionary",
		];

		public HashSet<string> KeywordsSet = [
			"func","return",
			"if","elif","else","while","continue","break",
		];

		public HashSet<string> OperatorSet = [
			"=",
			"+","-","*","/","%","^^",
			"~","|","&","^","<<",">>",

			"<",">",">=","<=","==",
			"||","&&","!",
		];


		private int _line = 0;
		private int _inputPos = 0;
		private int _simpleTokenPos = 0;
		private int _complexTokenPos = 0;

		private List<string> _input = [];
		public List<SimpleToken> SimpleTokens = [];
		public List<ComplexToken> ComplexTokens = [];
		public Statement CodeStatement = new();

		public Lexer(string input) => SetInput(input);
		public Lexer(List<string> input) => SetInput(input);

		public void SetInput(string input) => SetInput(new List<string>(input.Split('\n', StringSplitOptions.TrimEntries)));

		public void SetInput(List<string> input)
		{
			Clear();
			_input = input;
		}

		public void Clear()
		{
			_line = _inputPos = _simpleTokenPos = _complexTokenPos = 0;

			_input = [];
			SimpleTokens = [];
			ComplexTokens = [];
			CodeStatement = new();
		}

		public void AddInput(string input)
		{
			List<string> tempInput = new(input.Split('\n', StringSplitOptions.TrimEntries));
			_input.AddRange(tempInput);
		}

		public void Parse()
		{
			ParseSimpleTokens();
			ParseComplexTokens();
			CodeStatement = ParseStatement();
		}

		public void ParseSimpleTokens()
		{
			while (true)
			{
				SimpleToken token = NextSimpleToken();
				if (token.Type == SimpleTokenType.Error)
					throw new Exception("Error Toke: " + token);

				SimpleTokens.Add(token);

				if (token.Type == SimpleTokenType.EOF)
					break;
			}
		}

		public void ParseComplexTokens()
		{
			while (true)
			{
				ComplexToken token = NextComplexToken();
				if (token.Type == ComplexTokenType.Error)
					throw new Exception("Error Toke: " + token);

				ComplexTokens.Add(token);

				if (token.Type == ComplexTokenType.EOF)
					break;
			}
		}

		/// <summary>
		/// 重建语句，将同一个语句内的多个token合并，并且保留代码块信息
		/// </summary>
		/// <param name="baseCodeBlock"></param>
		/// <param name="deep"></param>
		/// <returns></returns>
		private Statement ParseStatement()
		{
			Statement statement = new() { IsCodeblock = true, CodeBlock = [new()], };
			while (_complexTokenPos < ComplexTokens.Count)
			{
				ComplexToken token = ComplexTokens[_complexTokenPos++];

				if (token.Type == ComplexTokenType.RightCodeBlock) // 代码块结束
					break;

				if (token.Type == ComplexTokenType.EOF) // 不加入当前
					break;

				if (token.Type == ComplexTokenType.LeftCodeBlock) // 代码块开始
				{
					_complexTokenPos++; //跳过左括号
					if (statement.CodeBlock[^1].CodeBlock.Count != 0 || statement.CodeBlock[^1].Tokens.Count != 0)
						statement.CodeBlock.Add(new());

					statement.CodeBlock[^1] = ParseStatement();
					statement.CodeBlock.Add(new());
					continue;
				}

				if (token.Type == ComplexTokenType.LineEnd) // 不加入当前，并且新开一个代码块
				{
					if (statement.CodeBlock[^1].CodeBlock.Count != 0 || statement.CodeBlock[^1].Tokens.Count != 0)
						statement.CodeBlock.Add(new());
					continue;
				}

				statement.CodeBlock[^1].Tokens.Add(token); // 直接加入
			}
			if (statement.CodeBlock[^1].CodeBlock.Count == 0 && statement.CodeBlock[^1].Tokens.Count == 0)
				statement.CodeBlock.RemoveAt(statement.CodeBlock.Count - 1);
			return statement;
		}



		public SimpleToken NextSimpleToken()
		{
			// 判断溢出
			if (_inputPos >= _input[_line].Length)
			{
				if (_line >= _input.Count)
				{

					return new() { Type = SimpleTokenType.Error, };
				}
				_line++;
				_inputPos = 0;
			}

			// 判断文件结束
			SimpleToken ret = new() { Line = _line, Type = SimpleTokenType.Void };
			if (_line >= _input.Count)
			{
				ret.Type = SimpleTokenType.EOF;
				return ret;
			}

			// 在此执行标记化逻辑
			int start = _inputPos;

			if (_input[_line] == "" || char.IsWhiteSpace(_input[_line][_inputPos]))
			{
				//^ 空白占位符
				_inputPos++;
				return NextSimpleToken();
			}
			else if (char.IsLetter(_input[_line][_inputPos]) || _input[_line][_inputPos] == '_')
			{
				//^ 处理 名称 和 关键字
				while (_inputPos < _input[_line].Length && (char.IsLetterOrDigit(_input[_line][_inputPos]) || _input[_line][_inputPos] == '_'))
					_inputPos++;

				string value = _input[_line][start.._inputPos];

				if (KeywordsSet.Contains(value))
				{
					ret.Type = value switch
					{
						"func" => SimpleTokenType.Function,
						"retuen" => SimpleTokenType.Retuen,
						"while" => SimpleTokenType.WHILE,
						"continue" => SimpleTokenType.CONTINUE,
						"break" => SimpleTokenType.BREAK,
						"if" => SimpleTokenType.IF,
						"elif" => SimpleTokenType.ELIF,
						"else" => SimpleTokenType.ELSE,
						_ => SimpleTokenType.Error,
					};
				}

				else if (AccessSet.Contains(value))
					ret.Type = SimpleTokenType.VarAccess;
				else if (TypeSet.Contains(value))
					ret.Type = SimpleTokenType.VarType;
				else
					ret.Type = SimpleTokenType.Name;
			}
			else if (char.IsDigit(_input[_line][_inputPos]))
			{
				//^ 处理数字
				while (_inputPos < _input[_line].Length && char.IsDigit(_input[_line][_inputPos]))
					_inputPos++;
				// 默认为整数
				ret.Type = SimpleTokenType.Number;
			}
			else if (OperatorSet.Contains(_input[_line][start..(_inputPos + 1)]))
			{
				//^ 处理运算符
				while (_inputPos < _input[_line].Length && OperatorSet.Contains(_input[_line][start..(_inputPos + 1)]))
					_inputPos++;

				string opString = _input[_line][start..(_inputPos + 1)];
				if (opString == "=")
					ret.Type = SimpleTokenType.AssignmentOperator;
				else
					ret.Type = SimpleTokenType.Operator;
			}
			else if (_input[_line][_inputPos] == ';')
			{
				//^ 处理分隔符
				ret.Type = SimpleTokenType.LineEnd;
				_inputPos++;
			}
			else if (_input[_line][_inputPos] == '.')
			{
				//^ 处理小数点 (伦理，应该不会出现单独的点)
				ret.Type = SimpleTokenType.Point;
				_inputPos++;
			}
			else if (_input[_line][_inputPos] == ',')
			{
				//^ 处理小数点
				ret.Type = SimpleTokenType.VarDelimiter;
				_inputPos++;
			}
			else if (_input[_line][_inputPos] == '(' || _input[_line][_inputPos] == ')')
			{
				//^ 处理括号
				ret.Type = _input[_line][_inputPos] switch
				{
					'(' => SimpleTokenType.LeftParen,
					')' => SimpleTokenType.RightParen,
					_ => throw new Exception(_input[_line] + " : " + _input[_line][_inputPos]),
				};
				_inputPos++;
			}
			else if (_input[_line][_inputPos] == '{' || _input[_line][_inputPos] == '}')
			{
				//^ 处理代码块
				ret.Type = _input[_line][_inputPos] switch
				{
					'{' => SimpleTokenType.LeftCodeBlock,
					'}' => SimpleTokenType.RightCodeBlock,
					_ => throw new Exception(_input[_line] + " : " + _input[_line][_inputPos]),
				};
				_inputPos++;
			}
			else if (_input[_line][_inputPos] == '[' || _input[_line][_inputPos] == ':' || _input[_line][_inputPos] == ']')
			{
				//^ 处理范围
				ret.Type = _input[_line][_inputPos] switch
				{
					'[' => SimpleTokenType.LeftRange,
					':' => SimpleTokenType.RangeDelimiter,
					']' => SimpleTokenType.RightRange,
					_ => throw new Exception(_input[_line] + " : " + _input[_line][_inputPos]),
				};
				_inputPos++;
			}
			else if (_input[_line][_inputPos] == '\"')
			{
				while (_inputPos < _input[_line].Length && _input[_line][_inputPos] != '\"')
				{
					if (_input[_line][_inputPos] == '\\')
						_inputPos++; //跳过下一个
					_inputPos++;
				}

				ret.Type = SimpleTokenType.String;
				_inputPos++;
			}
			else
			{
				//^ 处理其他 Token，默认为错误
				ret.Type = SimpleTokenType.Error;
				_inputPos++;
			}

			ret.Value = _input[_line][start.._inputPos];
			return ret;
		}

		public ComplexToken NextComplexToken()
		{
			if (_simpleTokenPos >= SimpleTokens.Count)
				return new() { Type = ComplexTokenType.Error };

			ComplexToken token = new() { Line = SimpleTokens[_simpleTokenPos].Line, Type = ComplexTokenType.Void, };

			//^ 变量定义
			if (SimpleTokens[_simpleTokenPos].Type == SimpleTokenType.VarAccess)
			{
				token.Type = ComplexTokenType.VarAccess;
				token.Tokens.Add(SimpleTokens[_simpleTokenPos]);
			}
			else if (SimpleTokens[_simpleTokenPos].Type == SimpleTokenType.VarType)
			{
				token.Type = ComplexTokenType.VarType;
				token.Tokens.Add(SimpleTokens[_simpleTokenPos]);
			}
			else if (SimpleTokens[_simpleTokenPos].Type == SimpleTokenType.VarDelimiter)
				token.Type = ComplexTokenType.VarDelimiter;

			//^ 变量大小
			else if (SimpleTokens[_simpleTokenPos].Type == SimpleTokenType.LeftRange)
			{
				token.Type = ComplexTokenType.VarRange;

				_simpleTokenPos++; // 跳过左边界
				bool numberFlag = true;
				while (SimpleTokens[_simpleTokenPos].Type != SimpleTokenType.RightRange)
				{
					if (!numberFlag && SimpleTokens[_simpleTokenPos].Type == SimpleTokenType.RangeDelimiter)
						numberFlag = true;
					else if (numberFlag && SimpleTokens[_simpleTokenPos].Type == SimpleTokenType.Number)
					{
						token.Tokens.Add(SimpleTokens[_simpleTokenPos]);
						numberFlag = false;
					}
					else
						throw new Exception($"Error token: {token}");
					_simpleTokenPos++;
				}
			}

			//^ 名称处理
			else if (SimpleTokens[_simpleTokenPos].Type == SimpleTokenType.Name)
			{
				token.Type = ComplexTokenType.VarName;
				token.Tokens.Add(SimpleTokens[_simpleTokenPos]);

				if (_simpleTokenPos + 1 < SimpleTokens.Count && SimpleTokens[_simpleTokenPos + 1].Type == SimpleTokenType.LeftParen)
					token.Type = ComplexTokenType.FuncName;
			}

			//^ 数字
			else if (SimpleTokens[_simpleTokenPos].Type == SimpleTokenType.Number)
			{
				token.Type = ComplexTokenType.IntNumber;
				token.Tokens.Add(SimpleTokens[_simpleTokenPos]);

				if (_simpleTokenPos + 2 < SimpleTokens.Count)
				{
					if (SimpleTokens[_simpleTokenPos + 1].Type == SimpleTokenType.Point && SimpleTokens[_simpleTokenPos + 2].Type == SimpleTokenType.Number)
					{
						token.Tokens.Add(SimpleTokens[_simpleTokenPos + 1]);
						token.Tokens.Add(SimpleTokens[_simpleTokenPos + 2]);
					}
				}
			}

			//^ 运算符
			else if (SimpleTokens[_simpleTokenPos].Type == SimpleTokenType.Operator)
			{
				token.Type = ComplexTokenType.Operator;
				token.Tokens.Add(SimpleTokens[_simpleTokenPos]);
			}
			else if (SimpleTokens[_simpleTokenPos].Type == SimpleTokenType.AssignmentOperator)
			{
				token.Type = ComplexTokenType.AssignmentOperator;
				token.Tokens.Add(SimpleTokens[_simpleTokenPos]);
			}

			//^ 关键字
			else if (SimpleTokens[_simpleTokenPos].Type == SimpleTokenType.Function)
				token.Type = ComplexTokenType.Function;
			else if (SimpleTokens[_simpleTokenPos].Type == SimpleTokenType.Retuen)
				token.Type = ComplexTokenType.Return;
			else if (SimpleTokens[_simpleTokenPos].Type == SimpleTokenType.IF)
				token.Type = ComplexTokenType.IF;
			else if (SimpleTokens[_simpleTokenPos].Type == SimpleTokenType.ELIF)
				token.Type = ComplexTokenType.ELIF;
			else if (SimpleTokens[_simpleTokenPos].Type == SimpleTokenType.ELSE)
				token.Type = ComplexTokenType.ELSE;
			else if (SimpleTokens[_simpleTokenPos].Type == SimpleTokenType.WHILE)
				token.Type = ComplexTokenType.WHILE;
			else if (SimpleTokens[_simpleTokenPos].Type == SimpleTokenType.CONTINUE)
				token.Type = ComplexTokenType.CONTINUE;
			else if (SimpleTokens[_simpleTokenPos].Type == SimpleTokenType.BREAK)
				token.Type = ComplexTokenType.BREAK;

			//^ 结构
			else if (SimpleTokens[_simpleTokenPos].Type == SimpleTokenType.LeftParen)
				token.Type = ComplexTokenType.LeftParen;
			else if (SimpleTokens[_simpleTokenPos].Type == SimpleTokenType.RightParen)
				token.Type = ComplexTokenType.RightParen;
			else if (SimpleTokens[_simpleTokenPos].Type == SimpleTokenType.LeftCodeBlock)
				token.Type = ComplexTokenType.LeftCodeBlock;
			else if (SimpleTokens[_simpleTokenPos].Type == SimpleTokenType.RightCodeBlock)
				token.Type = ComplexTokenType.RightCodeBlock;

			//^ 终结符处理
			else if (SimpleTokens[_simpleTokenPos].Type == SimpleTokenType.LineEnd)
				token.Type = ComplexTokenType.LineEnd;
			else if (SimpleTokens[_simpleTokenPos].Type == SimpleTokenType.EOF)
				token.Type = ComplexTokenType.EOF;
			else
				throw new Exception(" ??? " + SimpleTokens[_simpleTokenPos]);

			_simpleTokenPos++;
			return token;
		}
	}
}
