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
	public enum TokenType
	{
		Void,       // 空串
		Type,       // 类型
		Name,       // 名称
		Number,     // 数字
		String,     // 字符串
		Keyword,    // 关键字
		Operator,   // 运算符
		Delimiter,  // 分隔符
		CodeBlock,  // 代码块
		Error,      // 错误
	}

	public class SingleToken
	{
		public TokenType Type = TokenType.Void;
		public string Value = "";
		public int Line = 0;

		public override string ToString()
		{
			return new string(Line + ":" + Type.ToString() + ": " + Value + "\n");
		}
	}

	public class CodeBlock
	{
		public SingleToken Token = new();
		public List<CodeBlock> CodeBlocks = [];

		public TokenType Type => Token.Type;
		public string Value => Token.Value;

		public override string ToString()
		{
			string ret = "";
			foreach (var codeBlock in CodeBlocks)
			{
				ret += codeBlock.Token.ToString();
				if (codeBlock.Type == TokenType.CodeBlock)
					ret += "{ 0x" + codeBlock.GetHashCode().ToString("X") + "\n"
					+ codeBlock.ToString() + "} 0x"
					+ codeBlock.GetHashCode().ToString("X") + "\n";
			}
			return ret;
		}
	}

	public class Statement
	{
		public int Deep = 0;
		public List<SingleToken> Tokens = [];
		public List<Statement> Statements = [];

		public override string ToString()
		{
			string ret = "";
			foreach (var statement in Statements)
			{
				if (statement.Tokens.Count != 0)
				{
					ret += new string('\t', statement.Deep);
					foreach (var token in statement.Tokens)
						ret += token.Value + " ";
					ret += "\n";
				}
				// foreach (var state in statement.Statements)
				if (statement.Statements.Count != 0)
					ret += statement.ToString();
			}
			return ret;
		}
	}

	public class Lexer(string input)
	{
		public HashSet<string> typeSet = [
		"int","double","string","dictionary",
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
			"+","-","*","-","%","^^",
			"~","|","&","^","<<",">>",

			"<",">",">=","<=","==",
			"||","&&","!",
		];

		private readonly List<string> _input = new(input.Split('\n', defaultStringSplitOptions));
		private int _position = 0;
		private int _line = 0;

		public CodeBlock GlobleCodeBlocks = new();
		public Statement GlobleStatements = new();

		public void AddInput(string input)
		{
			List<string> tempInput = new(input.Split('\n', defaultStringSplitOptions));
			_input.AddRange(tempInput);
		}

		public void Parse()
		{
			ParseCodeblock(GlobleCodeBlocks);
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
			else if (_position < _input[_line].Length && (_input[_line][_position] == '.' || _input[_line][_position] == ';' || _input[_line][_position] == ','))
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
		private void ParseCodeblock(CodeBlock baseCodeBlock)
		{
			do
			{
				CodeBlock _currentToken = new() { Token = GetNextToken() };

				if (_currentToken.Token.Value == "{")
				{
					_currentToken.Token.Value = "{ ... }";
					baseCodeBlock.CodeBlocks.Add(_currentToken);
					_currentToken.CodeBlocks = [];
					ParseCodeblock(_currentToken);
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

		/// <summary>
		/// 重建语句，将同一个语句内的多个token合并，并且保留代码块信息
		/// </summary>
		/// <param name="baseCodeBlock"></param>
		/// <param name="deep"></param>
		/// <returns></returns>
		private static Statement RebuildStatement(CodeBlock baseCodeBlock, int deep = 0)
		{
			Statement statement = new() { Deep = deep };
			Statement temp = new() { Deep = deep };

			foreach (var codeBlock in baseCodeBlock.CodeBlocks)
			{
				if (codeBlock.Type == TokenType.Delimiter && codeBlock.Value == ";")
				{
					statement.Statements.Add(temp);
					temp = new() { Deep = deep };
					continue;
				}

				if (codeBlock.Type == TokenType.CodeBlock)
				{
					Statement codeBlockState = RebuildStatement(codeBlock, deep + 1);
					foreach (var item in codeBlockState.Statements)
						temp.Statements.Add(item);

					statement.Statements.Add(temp);
					temp = new() { Deep = deep };
					continue;
				}

				temp.Tokens.Add(codeBlock.Token);
			}

			return statement;
		}
	}
}
