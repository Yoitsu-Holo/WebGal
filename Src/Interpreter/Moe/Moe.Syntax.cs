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
	public enum TokenType
	{
		Void, // 空串
		Identifier, // 标识符
		Name, // 名称
		Number, // 数字
		Keyword, // 字符
		Operator, // 运算符
		Delimiter, // 分隔符
		Label, // 标记
		Error, // 错误
	}

	public class Token(TokenType tokenType = TokenType.Void, string value = "")
	{
		public TokenType Type { get; set; } = tokenType;
		public string Value { get; set; } = value;
	}

	public class Lexer(string input)
	{

		public HashSet<string> keywordsSet = [
			"var","int","double","string","dictionary",
			"func","return",
			"if","else","while","goto",
			// 这些是函数，不属于关键字
			// "say","ch","st","bg","anime",
			// "bgm","voice","audio",
			// "choice","tag",
			// "play","end","pause","delay",
		];

		public HashSet<string> identifierSet = [
			"(",")",
			"=","+","-","*","-","%",
			"++","--","+=","-=","*=","/=","%=",
			"~","|","&","^","<<",">>",
			"||","&&",
			"^^",
			"\\",
		];

		private readonly string _input = input;
		private int _position = 0;

		public Token GetNextToken()
		{
			Token ret = new();
			if (_position >= _input.Length)
				return ret;

			// 在此执行标记化逻辑
			int start = _position;

			if (char.IsWhiteSpace(_input[_position]))
			{
				// 空白占位符
				_position++;
				ret = GetNextToken();
			}
			else if (char.IsLetter(_input[_position]) || _input[_position] == '_')
			{
				// 处理 名称 和 关键字
				while (_position < _input.Length && (char.IsLetterOrDigit(_input[_position]) || _input[_position] == '_'))
					_position++;

				ret.Value = _input[start.._position];
				if (keywordsSet.Contains(ret.Value))
					ret.Type = TokenType.Keyword;
				else
					ret.Type = TokenType.Name;
			}
			else if (char.IsDigit(_input[_position]))
			{
				// 处理数字

				// 整数部分
				while (_position < _input.Length && char.IsDigit(_input[_position]))
					_position++;
				// 小数部分
				if (_position < _input.Length && _input[_position] == '.')
					_position++;
				while (_position < _input.Length && char.IsDigit(_input[_position]))
					_position++;

				ret = new Token(TokenType.Number, _input[start.._position]);
			}
			else if (_input[_position] == ':' || _input[_position] == '@')
			{
				// 处理标签
				_position++;
				ret = new Token(TokenType.Label, _input[start.._position]);
			}
			else if (_input[_position] == '.' || _input[_position] == ';')
			{
				// 处理分隔符
				_position++;
				ret = new Token(TokenType.Delimiter, _input[start.._position]);
			}
			else
			{
				// Handle operators and other tokens
				// This is just a placeholder implementation
				_position++;
				ret = new Token(TokenType.Operator, _input[start.._position].ToString());
			}

			return ret;
		}
	}
	public class Parser
	{
		private readonly Lexer _lexer;
		private Token _currentToken;

		public Parser(Lexer lexer)
		{
			_lexer = lexer;
			_currentToken = new();
		}

		public void ParseCodeBlock()
		{
			// Parse statements inside the code block
			Console.WriteLine("{");
			_currentToken = _lexer.GetNextToken(); // Consume '{'
			while (_currentToken.Type != TokenType.Void && _currentToken.Value != "}")
			{
				// Parse individual statements inside the code block
				if (_currentToken.Value == "{")
				{
					// Nested code block
					ParseCodeBlock();
				}
				else
				{
					// Handle other tokens inside the code block
					// You may need to recursively call other parsing methods here
					// Console.WriteLine("Token inside code block: " + _currentToken.Value);
					Console.WriteLine(_currentToken.Type + ": " + _currentToken.Value);
					_currentToken = _lexer.GetNextToken();
				}
			}
			Console.WriteLine("}");
			if (_currentToken != null)
			{
				_currentToken = _lexer.GetNextToken(); // Consume '}'
			}
		}
	}
}

/*
{
	Keyword: var
	Keyword: int
	Name: x
	Delimiter: ;
	Name: x
	Operator: =
	Number: 10
	Delimiter: ;
	Keyword: while
	Operator: (
	Name: x
	Operator: >
	Number: 0
	Operator: )
	{
		Name: x
		Operator: =
		Name: x
		Operator: -
		Number: 100.1
		Delimiter: ;
		{
			Name: y__y
			Operator: =
			Number: 100.0
			Delimiter: ;
		}
		Keyword: if
		Operator: (
		Name: x
		Operator: >
		Number: 1000
		Operator: )
	}
	Keyword: goto
	Name: end
	Delimiter: ;
	Name: end
	Label: :
}
*/