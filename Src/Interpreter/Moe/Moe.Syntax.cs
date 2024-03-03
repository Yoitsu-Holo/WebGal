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
		Type, // 标识符
		Name, // 名称
		Number, // 数字
		Keyword, // 字符
		Operator, // 运算符
		Delimiter, // 分隔符
		Label, // 标记
		CodeBlock, // 代码块
		Error, // 错误
	}

	public class SingleToken
	{
		public TokenType Type = TokenType.Void;
		public string Value = "";
		// public List<SingleToken> CodeBlocks = [];

		public override string ToString()
		{
			return new string(Type.ToString() + ": " + Value + "\n");
		}
	}

	public class CodeBlock
	{
		public SingleToken Token = new();
		public List<CodeBlock> CodeBlocks = [];

		public override string ToString()
		{
			string ret = "";
			foreach (var codeBlock in CodeBlocks)
			{
				ret += codeBlock.Token.ToString();
				if (codeBlock.Token.Type == TokenType.CodeBlock)
					ret += "{ 0x" + codeBlock.GetHashCode().ToString("X") + "\n" + codeBlock.ToString() + "} 0x" + codeBlock.GetHashCode().ToString("X") + "\n";
			}
			return ret;
		}
	}

	public class Statement
	{
		public List<SingleToken> Tokens = [];
		public List<CodeBlock> CodeBlocks = [];
	}

	public class SyntaxBuilder(string input)
	{
		public HashSet<string> typeSet = [
			"int","double","string","dictionary",
		];

		public HashSet<string> keywordsSet = [
			"var","func","return",
			"if","else","while","goto","label",
			// 这些是函数，不属于关键字
			// "say","ch","st","bg","anime",
			// "bgm","voice","audio",
			// "choice","tag",
			// "play","end","pause","delay",
		];

		public HashSet<string> operatorSet = [
			"(",")","[","]","<",">",
			"=","+","-","*","-","%",
			"~","|","&","^",

			"++","--","+=","-=","*=","/=","%=","^^",
			"<<",">>","||","&&",
		];

		private readonly string _input = input;
		private int _position = 0;
		// private Token _currentToken = new();

		// private readonly Token _baseToken = new();

		public CodeBlock GlobleCodeBlocks = new();
		public Statement GlobleStatements = new();

		public SingleToken GetNextToken()
		{
			SingleToken ret = new();
			if (_position >= _input.Length)
				return ret;

			// 在此执行标记化逻辑
			int start = _position;

			if (char.IsWhiteSpace(_input[_position]))
			{
				//^ 空白占位符
				_position++;
				return GetNextToken();
			}
			else if (char.IsLetter(_input[_position]) || _input[_position] == '_')
			{
				//^ 处理 名称 和 关键字
				while (_position < _input.Length && (char.IsLetterOrDigit(_input[_position]) || _input[_position] == '_'))
					_position++;

				string value = _input[start.._position];

				if (keywordsSet.Contains(value))
					ret.Type = TokenType.Keyword;
				else if (typeSet.Contains(value))
					ret.Type = TokenType.Type;
				else
					ret.Type = TokenType.Name;
			}
			else if (char.IsDigit(_input[_position]))
			{
				//^ 处理数字

				// 整数部分
				while (_position < _input.Length && char.IsDigit(_input[_position]))
					_position++;
				// 小数部分
				if (_position < _input.Length && _input[_position] == '.')
					_position++;
				while (_position < _input.Length && char.IsDigit(_input[_position]))
					_position++;
				ret.Type = TokenType.Number;
			}
			else if (_input[_position] == ':' || _input[_position] == '@')
			{
				//^ 处理标签  ':'数组访问标签  '@'函数参数表标签
				// todo 可能会被优化
				_position++;
				ret.Type = TokenType.Label;
			}
			else if (_input[_position] == '.' || _input[_position] == ';')
			{
				//^ 处理分隔符
				_position++;
				ret.Type = TokenType.Delimiter;
			}
			else if (operatorSet.Contains(_input[_position].ToString()))
			{
				//^ 处理运算符
				while (operatorSet.Contains(_input[start..(_position + 1)]))
					_position++;
				ret.Type = TokenType.Operator;
			}
			else if (_input[_position] == '{' || _input[_position] == '}')
			{
				//^ 处理代码块
				_position++;
				ret.Type = TokenType.CodeBlock;
			}
			else
			{
				//^ 处理其他 Token，默认为错误
				_position++;
				ret.Type = TokenType.Error;
			}

			ret.Value = _input[start.._position];
			return ret;
		}

		public void Parse() => DFSParse(GlobleCodeBlocks);

		private void DFSParse(CodeBlock baseCodeBlock)
		{
			// Parse statements inside the code block
			// _currentToken = GetNextToken(); // Consume '{'
			do
			{
				// Parse individual statements inside the code block
				CodeBlock _currentToken = new()
				{
					Token = GetNextToken()
				};
				// Console.WriteLine(_currentToken.Type + ": " + _currentToken.Value);
				if (_currentToken.Token.Value == "{")
				{
					_currentToken.Token.Value = "{}";
					baseCodeBlock.CodeBlocks.Add(_currentToken);
					_currentToken.CodeBlocks = [];
					DFSParse(_currentToken);
					// Tokens.Add(_currentToken);
				}
				else if (_currentToken.Token.Value == "}")
					break;
				else
					baseCodeBlock.CodeBlocks.Add(_currentToken);

				if (_currentToken.Token.Type == TokenType.Void)
					break;
			}
			while (true);
		}
	}
}

/*
Keyword: var
Type: int
Name: x
Label: :
Number: 10
Delimiter: ;
Name: x
Operator: [
Number: 0
Operator: ]
Operator: =
Number: 10
Delimiter: ;
Keyword: while
Operator: (
Name: x
Operator: >
Number: 0
Operator: )
CodeBlock: {}
{cb: 0x7C3CAF5
	Name: x
	Operator: =
	Name: x
	Operator: -
	Number: 100.1
	Delimiter: ;
	CodeBlock: {}
	{cb: 0x3C77FBC7
		Name: y__y
		Operator: =
		Number: 100.0
		Delimiter: .
		Number: 123
		Delimiter: ;
	}cb: 0x3C77FBC7
	Keyword: if
	Operator: (
	Name: x
	Operator: >
	Number: 1000
	Operator: )
	CodeBlock: {}
	{cb: 0x2CCF938B
	}cb: 0x2CCF938B
	Name: 错误
	Delimiter: ;
}cb: 0x7C3CAF5
Keyword: goto
Name: end
Delimiter: ;
Keyword: label
Name: end
Delimiter: ;
*/