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
		public HashSet<string> typeSet = [
			"void","int","double","string","dictionary",
		];

		public HashSet<string> keywordsSet = [
			"var","func","return",
			"if","elif","else","while","continue","break",
			"goto","label",
			// 这些是函数，不属于关键字
			// "say","ch","st","bg","anime",
			// "bgm","voice","audio",
			// "choice","tag",
			// "play","end","pause","delay",
		];

		public HashSet<string> operatorSet = [
			"(",")","[","]",
			"=",
			"+","-","*","/","%","^^",
			"~","|","&","^","<<",">>",

			"<",">",">=","<=","==",
			"||","&&","!",
		];

		public HashSet<char> delimiterSet = [
			',','.',
			':',';',
		];


		private readonly List<string> _input = [];
		private int _position = 0;
		private int _line = 0;

		public CodeBlock GlobleCodeBlocks = new();
		public Statement GlobleStatements = new();
		public List<SingleToken> GlobleTokens = [];

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

		public SingleToken GetNextToken()
		{
			SingleToken ret = new() { Line = -1 };
			if (_position >= _input[_line].Length)
			{
				if (_input.Count <= _line)
					return ret;
				_line++;
				_position = 0;
			}
			if (_line >= _input.Count)
				return ret;

			ret.Line = _line;

			// 在此执行标记化逻辑
			int start = _position;

			if (char.IsWhiteSpace(_input[_line][_position]))
			{
				//^ 空白占位符
				_position++;
				return GetNextToken();
			}
			else if (char.IsLetter(_input[_line][_position]) || _input[_line][_position] == '_')
			{
				//^ 处理 名称 和 关键字
				while (_position < _input[_line].Length && (char.IsLetterOrDigit(_input[_line][_position]) || _input[_line][_position] == '_'))
					_position++;

				string value = _input[_line][start.._position];

				if (keywordsSet.Contains(value))
					ret.Type = TokenType.Keyword;
				else if (typeSet.Contains(value))
					ret.Type = TokenType.Type;
				else
					ret.Type = TokenType.Name;
			}
			else if (char.IsDigit(_input[_line][_position]))
			{
				//^ 处理数字
				while (_position < _input[_line].Length && char.IsDigit(_input[_line][_position]))
					_position++;
				ret.Type = TokenType.Number;
			}
			else if (_position < _input[_line].Length && delimiterSet.Contains(_input[_line][_position]))
			{
				//^ 处理分隔符
				_position++;
				ret.Type = TokenType.Delimiter;
			}
			else if (operatorSet.Contains(_input[_line][_position].ToString()))
			{
				// _position++;
				//^ 处理运算符
				while (_position < _input[_line].Length && operatorSet.Contains(_input[_line][start..(_position + 1)]))
					_position++;
				ret.Type = TokenType.Operator;
			}
			else if (_input[_line][_position] == '{' || _input[_line][_position] == '}')
			{
				//^ 处理代码块
				_position++;
				ret.Type = TokenType.CodeBlock;
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

				ret.Type = TokenType.String;
			}
			else
			{
				//^ 处理其他 Token，默认为错误
				_position++;
				ret.Type = TokenType.Error;
			}

			ret.Value = _input[_line][start.._position];
			return ret;
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
				CodeBlock _currentToken = new() { Token = GetNextToken() };
				if (_currentToken.Token.Type == TokenType.Void)
					break;

				GlobleTokens.Add(_currentToken.Token);

				if (_currentToken.Token.Value == "{")
				{
					ret.CodeBlocks.Add(ParseCodeblock());
					ret.CodeBlocks[^1].Token.Type = TokenType.CodeBlock;
				}
				else if (_currentToken.Token.Value == "}")
					break;
				else
					ret.CodeBlocks.Add(_currentToken);

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
				if (codeBlock.Token.Type == TokenType.Delimiter && codeBlock.Token.Value == ";")
				{
					ret.Statements.Add(temp);
					temp = new() { Deep = deep };
					continue;
				}

				if (codeBlock.Token.Type == TokenType.CodeBlock)
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
