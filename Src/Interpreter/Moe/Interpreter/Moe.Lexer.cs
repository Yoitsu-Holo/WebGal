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
			"var","static","const","ref",
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
		private int _complexTokenPos = 0;

		private List<string> _input = [];
		public List<Token> Tokens = [];
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
			_line = _inputPos = _complexTokenPos = 0;

			_input = [];
			Tokens = [];
			// ComplexTokens = [];
			CodeStatement = new();
		}

		public void AddInput(string input)
		{
			List<string> tempInput = new(input.Split('\n', StringSplitOptions.TrimEntries));
			_input.AddRange(tempInput);
		}

		public void Parse()
		{
			ParseTokens();
			CodeStatement = ParseStatement();
		}

		public void ParseTokens()
		{
			while (true)
			{
				Token token = NextToken();
				if (token.Type == TokenType.Error)
					throw new Exception("Error Toke: " + token);

				if (token.Type == TokenType.EOF)
					break;

				Tokens.Add(token);
			}

			for (int i = 0; i < Tokens.Count; i++)
			{
				if (Tokens[i].Type == TokenType.Function && Tokens[i + 2].Type == TokenType.VarName)
					Tokens[i + 2].Type = TokenType.FuncName;
				else if (Tokens.Count > i + 1 && Tokens[i].Type == TokenType.VarName && Tokens[i + 1].Type == TokenType.LeftParen)
					Tokens[i].Type = TokenType.FuncName;
			}
		}

		private Statement ParseStatement()
		{
			Statement statement = new() { IsCodeblock = true, CodeBlock = [new()], };
			while (_complexTokenPos < Tokens.Count)
			{
				Token token = Tokens[_complexTokenPos++];

				if (token.Type == TokenType.RightCodeBlock) // 代码块结束
					break;

				if (token.Type == TokenType.LeftCodeBlock) // 代码块开始
				{
					if (statement.CodeBlock[^1].CodeBlock.Count != 0 || statement.CodeBlock[^1].Tokens.Count != 0)
						statement.CodeBlock.Add(new());

					statement.CodeBlock[^1] = ParseStatement();
					statement.CodeBlock.Add(new());
					continue;
				}

				if (token.Type == TokenType.LineEnd) // 不加入当前，并且新开一个代码块
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

		public Token NextToken()
		{
			// 判断溢出
			if (_inputPos >= _input[_line].Length)
			{
				if (_line >= _input.Count)
				{

					return new() { Type = TokenType.Error, };
				}
				_line++;
				_inputPos = 0;
			}

			// 判断文件结束
			Token ret = new() { Line = _line, Type = TokenType.Void };
			if (_line >= _input.Count)
			{
				ret.Type = TokenType.EOF;
				return ret;
			}

			// 在此执行标记化逻辑
			int start = _inputPos;

			if (_input[_line] == "" || char.IsWhiteSpace(_input[_line][_inputPos]))
			{
				//^ 空白占位符
				_inputPos++;
				return NextToken();
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
						"func" => TokenType.Function,
						"retuen" => TokenType.Return,
						"while" => TokenType.WHILE,
						"continue" => TokenType.CONTINUE,
						"break" => TokenType.BREAK,
						"if" => TokenType.IF,
						"elif" => TokenType.ELIF,
						"else" => TokenType.ELSE,
						_ => TokenType.Error,
					};
				}

				else if (AccessSet.Contains(value))
					ret.Type = TokenType.VarAccess;
				else if (TypeSet.Contains(value))
					ret.Type = TokenType.VarType;
				else
					ret.Type = TokenType.VarName;
			}
			else if (char.IsDigit(_input[_line][_inputPos]))
			{
				//^ 处理数字
				while (_inputPos < _input[_line].Length && char.IsDigit(_input[_line][_inputPos]))
					_inputPos++;
				ret.Type = TokenType.IntNumber;

				if (_input[_line][_inputPos] == '.')
				{
					_inputPos++;
					while (_inputPos < _input[_line].Length && char.IsDigit(_input[_line][_inputPos]))
						_inputPos++;
					ret.Type = TokenType.FloatNumber;
				}
			}
			else if (OperatorSet.Contains(_input[_line][start..(_inputPos + 1)]))
			{
				//^ 处理运算符
				while (_inputPos < _input[_line].Length && OperatorSet.Contains(_input[_line][start..(_inputPos + 1)]))
					_inputPos++;

				string opString = _input[_line][start.._inputPos];
				if (opString == "=")
					ret.Type = TokenType.AssignmentOperator;
				else
					ret.Type = TokenType.Operator;
			}
			else if (_input[_line][_inputPos] == ';')
			{
				//^ 处理分隔符
				ret.Type = TokenType.LineEnd;
				_inputPos++;
			}
			else if (_input[_line][_inputPos] == ',')
			{
				//^ 处理小数点
				ret.Type = TokenType.VarDelimiter;
				_inputPos++;
			}
			else if (_input[_line][_inputPos] == '(' || _input[_line][_inputPos] == ')')
			{
				//^ 处理括号
				ret.Type = _input[_line][_inputPos] switch
				{
					'(' => TokenType.LeftParen,
					')' => TokenType.RightParen,
					_ => throw new NotImplementedException(),
				};
				_inputPos++;
			}
			else if (_input[_line][_inputPos] == '{' || _input[_line][_inputPos] == '}')
			{
				//^ 处理代码块
				ret.Type = _input[_line][_inputPos] switch
				{
					'{' => TokenType.LeftCodeBlock,
					'}' => TokenType.RightCodeBlock,
					_ => throw new NotImplementedException(),
				};
				_inputPos++;
			}
			else if (_input[_line][_inputPos] == '[' || _input[_line][_inputPos] == ']')
			{
				//^ 处理范围
				ret.Type = _input[_line][_inputPos] switch
				{
					'[' => TokenType.LeftRange,
					']' => TokenType.RightRange,
					_ => throw new NotImplementedException(),
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

				ret.Type = TokenType.String;
				_inputPos++;
			}
			else
			{
				//^ 处理其他 Token，默认为错误
				ret.Type = TokenType.Error;
				_inputPos++;
			}

			ret.Value = _input[_line][start.._inputPos];
			return ret;
		}
	}
}
