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
			"var","func","return",
			"if","elif","else","while","continue","break",
		];

		public HashSet<string> OperatorSet = [
			"=",
			"+","-","*","/","%","^^",
			"~","|","&","^","<<",">>",

			"<",">",">=","<=","==",
			"||","&&","!",
		];

		private HashSet<string> MathOperatorSet = [
			"+","-","*","/","%","^^",
			"&","|","^","~",
			"<<",">>",
		];

		private HashSet<string> LogicOperatorSet = [
			"==","!=",">=","<=",">","<",
			"&&","||","^","!",
		];


		private readonly List<string> _input = [];
		private int _position = 0;
		private int _tokenPos = 0;
		private int _line = 0;

		public List<SimpleToken> SimpleTokens = [];
		public List<ComplexToken> ComplexTokens = [];
		public CodeBlock GlobleCodeBlocks = new();
		public Statement GlobleStatements = new();

		[Obsolete]
		public List<ComplexToken> GlobleTokens = [];

		public Lexer(string input) => _input = new(input.Split('\n', defaultStringSplitOptions));
		public Lexer(List<string> input) => _input = input;

		public void AddInput(string input)
		{
			List<string> tempInput = new(input.Split('\n', defaultStringSplitOptions));
			_input.AddRange(tempInput);
		}

		public void Parse()
		{
			GlobleCodeBlocks = ParseCodeblock();
			GlobleStatements = RebuildStatement(GlobleCodeBlocks);
		}

		public SimpleToken GetNextSimpleToken()
		{
			SimpleToken ret = new() { Line = -1, Type = SimpleTokenType.Error };

			if (_position >= _input[_line].Length)
			{
				if (_input.Count <= _line)
					return ret;
				_line++;
				_position = 0;
			}

			if (_line >= _input.Count)
			{
				ret.Type = SimpleTokenType.EOF;
				return ret;
			}

			ret.Line = _line;

			// 在此执行标记化逻辑
			int start = _position;

			if (char.IsWhiteSpace(_input[_line][_position]))
			{
				//^ 空白占位符
				_position++;
				return GetNextSimpleToken();
			}
			else if (char.IsLetter(_input[_line][_position]) || _input[_line][_position] == '_')
			{
				//^ 处理 名称 和 关键字
				while (_position < _input[_line].Length && (char.IsLetterOrDigit(_input[_line][_position]) || _input[_line][_position] == '_'))
					_position++;

				string value = _input[_line][start.._position];

				if (KeywordsSet.Contains(value))
					ret.Type = SimpleTokenType.Keyword;
				else if (AccessSet.Contains(value))
					ret.Type = SimpleTokenType.Access;
				else if (TypeSet.Contains(value))
					ret.Type = SimpleTokenType.Type;
				else
					ret.Type = SimpleTokenType.Name;
			}
			else if (char.IsDigit(_input[_line][_position]))
			{
				//^ 处理数字
				while (_position < _input[_line].Length && char.IsDigit(_input[_line][_position]))
					_position++;
				ret.Type = SimpleTokenType.Number;
			}
			else if (OperatorSet.Contains(_input[_line][_position].ToString()))
			{
				// _position++;
				//^ 处理运算符
				while (_position < _input[_line].Length && OperatorSet.Contains(_input[_line][start..(_position + 1)]))
					_position++;
				ret.Type = SimpleTokenType.Operator;
			}
			else if (_input[_line][_position] == ';')
			{
				//^ 处理分隔符
				_position++;
				ret.Type = SimpleTokenType.LineEnd;
			}
			else if (_input[_line][_position] == '.')
			{
				//^ 处理小数点
				_position++;
				ret.Type = SimpleTokenType.Point;
			}
			else if (_input[_line][_position] == '(' || _input[_line][_position] == ')')
			{
				//^ 处理括号
				_position++;
				ret.Type = SimpleTokenType.LeftCodeBlock;
				ret.Type = _input[_line][_position] switch
				{
					'(' => SimpleTokenType.LeftParen,
					')' => SimpleTokenType.RightParen,
					_ => throw new NotImplementedException(),
				};
			}
			else if (_input[_line][_position] == '{' || _input[_line][_position] == '}')
			{
				//^ 处理代码块
				_position++;
				ret.Type = SimpleTokenType.LeftCodeBlock;
				ret.Type = _input[_line][_position] switch
				{
					'{' => SimpleTokenType.LeftCodeBlock,
					'}' => SimpleTokenType.RightCodeBlock,
					_ => throw new NotImplementedException(),
				};
			}
			else if (_input[_line][_position] == '[' || _input[_line][_position] == ']' || _input[_line][_position] == ':')
			{
				//^ 处理代码块
				_position++;
				ret.Type = SimpleTokenType.LeftCodeBlock;
				ret.Type = _input[_line][_position] switch
				{
					'[' => SimpleTokenType.LeftRange,
					':' => SimpleTokenType.RangeDelimiter,
					']' => SimpleTokenType.RightRange,
					_ => throw new NotImplementedException(),
				};
			}
			else if (_input[_line][_position] == '\"')
			{
				bool isEscape = true;
				while (_position < _input[_line].Length && (_input[_line][_position] != '\"' || isEscape))
				{
					if (isEscape)
						isEscape = false;
					if (_input[_line][_position] == '\\')
						isEscape = true;
					_position++;
				}
				_position++;

				ret.Type = SimpleTokenType.String;
			}
			else
			{
				//^ 处理其他 Token，默认为错误
				_position++;
				ret.Type = SimpleTokenType.Error;
			}

			ret.Value = _input[_line][start.._position];
			return ret;
		}


		public ComplexToken GetNextComplexToken()
		{
			if (ComplexTokens.Count > _tokenPos)
				return ComplexTokens[_tokenPos++];
			throw new IndexOutOfRangeException();
		}

		public void ParseSimpleTokens()
		{
			while (true)
			{
				SimpleToken token = GetNextSimpleToken();
				if (token.Type == SimpleTokenType.Error)
					throw new Exception("Error Toke: " + token.Value);

				SimpleTokens.Add(token);

				if (token.Type == SimpleTokenType.EOF)
					break;
			}
		}

		public void ParseComplexTokens()
		{
			for (int i = 0; i < SimpleTokens.Count; i++)
			{
				SimpleToken ptoken = SimpleTokens[i];
				ComplexToken token = new() { Line = ptoken.Line, Value = ptoken.Value, Type = ComplexTokenType.VariableAccess, }; ;

				if (ptoken.Type == SimpleTokenType.Access)
					token.Type = ComplexTokenType.VariableAccess;
				else if (ptoken.Type == SimpleTokenType.Type)
					token.Type = ComplexTokenType.VaribaleType;
				else if (ptoken.Type == SimpleTokenType.LeftParen)
					token.Type = ComplexTokenType.LeftParen;
				else if (ptoken.Type == SimpleTokenType.RightParen)
					token.Type = ComplexTokenType.RightParen;
				else if (ptoken.Type == SimpleTokenType.LeftRange)
					token.Type = ComplexTokenType.LeftRange;
				else if (ptoken.Type == SimpleTokenType.RangeDelimiter)
					token.Type = ComplexTokenType.RangeDelimiter;
				else if (ptoken.Type == SimpleTokenType.RightRange)
					token.Type = ComplexTokenType.RangeDelimiter;
				else if (ptoken.Type == SimpleTokenType.LeftCodeBlock)
					token.Type = ComplexTokenType.LeftCodeBlock;
				else if (ptoken.Type == SimpleTokenType.RightCodeBlock)
					token.Type = ComplexTokenType.RightCodeBlock;
				else if (ptoken.Type == SimpleTokenType.LineEnd)
					token.Type = ComplexTokenType.LineEnd;
				else if (ptoken.Type == SimpleTokenType.EOF)
					token.Type = ComplexTokenType.EOF;
				else if (ptoken.Type == SimpleTokenType.Operator) // 细分运算符
				{
					if (ptoken.Value == "=")
						token.Type = ComplexTokenType.AssignmentOperator;
					else if (MathOperatorSet.Contains(ptoken.Value) && LogicOperatorSet.Contains(ptoken.Value))
						token.Type = ComplexTokenType.BothOperator;
					else if (MathOperatorSet.Contains(ptoken.Value))
						token.Type = ComplexTokenType.MathOperator;
					else if (LogicOperatorSet.Contains(ptoken.Value))
						token.Type = ComplexTokenType.LogicOperator;
					else
						throw new Exception($"Unknow Operator : {ptoken.Value}");
				}

				else if (ptoken.Type == SimpleTokenType.Keyword) // 细分关键字
				{
					token.Type = token.Value switch
					{
						"if" => ComplexTokenType.IF,
						"elif" => ComplexTokenType.ELIF,
						"else" => ComplexTokenType.ELSE,
						"while" => ComplexTokenType.WHILE,
						"break" => ComplexTokenType.BREAK,
						"continue" => ComplexTokenType.CONTINUE,
						_ => throw new Exception($"Unknow Keyword : {ptoken.Value}"),
					};
				}

				else if (ptoken.Type == SimpleTokenType.Name) // 可能为函数名称
				{
					if (SimpleTokens.Count > i + 1 && SimpleTokens[i + 1].Value == "(")
						token.Type = ComplexTokenType.FuncName;
					else
						token.Type = ComplexTokenType.VarName;
				}
				else if (ptoken.Type == SimpleTokenType.Number) // 可能为小数
				{
					if (SimpleTokens.Count > i + 2 && SimpleTokens[i + 1].Type == SimpleTokenType.Point && SimpleTokens[i + 2].Type == SimpleTokenType.Number)
					{
						token.Type = ComplexTokenType.FloatNumber;
						token.Value = SimpleTokens[i].Value + "." + SimpleTokens[i + 2].Value;
						i += 2;
					}
					else
						token.Type = ComplexTokenType.IntNumber;
				}
				else if (ptoken.Type == SimpleTokenType.String) // 需要转义
				{
					token.Value = "";
					string raw = ptoken.Value[1..^1];
					bool escape = false;
					foreach (char c in raw)
					{
						if (escape)
						{
							escape = false;
							token.Value += c;
							continue;
						}
						if (c == '\\')
						{
							escape = true;
							continue;
						}
						token.Value += c;
					}
					token.Type = ComplexTokenType.ConstString;
				}
				else
					throw new Exception($"Line: {ptoken.Line}\nUnknow Type : {ptoken.Type}\tValue : {ptoken.Value}");

				ComplexTokens.Add(token);
			}
		}

		/// <summary>
		/// 解析代码块，将token按照代码块来组合
		/// </summary>
		/// <param name="baseCodeBlock"></param>
		private CodeBlock ParseCodeblock()
		{
			CodeBlock ret = new();
			do
			{
				ComplexToken token = GetNextComplexToken();
				CodeBlock codeblock = new() { Token = token };
				if (codeblock.Token.Type == ComplexTokenType.Void || codeblock.Token.Type == ComplexTokenType.EOF)
					break;

				if (codeblock.Token.Type == ComplexTokenType.LeftCodeBlock)
				{
					ret.CodeBlocks.Add(ParseCodeblock());
					ret.CodeBlocks[^1].IsCodeBlock = true;
				}
				else if (codeblock.Token.Type == ComplexTokenType.RightCodeBlock)
					break;
				else
					ret.CodeBlocks.Add(codeblock);
			}
			while (true);
			return ret;
		}

		/// <summary>
		/// 重建语句，将同一个语句内的多个token合并，并且保留代码块信息
		/// </summary>
		/// <param name="baseCodeBlock"></param>
		/// <param name="deep"></param>
		/// <returns></returns>
		private static Statement RebuildStatement(CodeBlock baseCodeBlock, int deep = 0)
		{
			Statement ret = new() { Deep = deep };
			Statement temp = new() { Deep = deep };

			foreach (var codeBlock in baseCodeBlock.CodeBlocks)
			{
				if (codeBlock.Token.Type == ComplexTokenType.LineEnd)
				{
					ret.Statements.Add(temp);
					temp = new() { Deep = deep };
					continue;
				}

				if (codeBlock.IsCodeBlock)
				{
					Statement codeBlockState = RebuildStatement(codeBlock, deep + 1);
					temp.Statements.AddRange(codeBlockState.Statements);
					ret.Statements.Add(temp);
					temp = new() { Deep = deep };
					continue;
				}

				temp.Tokens.Add(codeBlock.Token);
			}

			return ret;
		}
	}
}
