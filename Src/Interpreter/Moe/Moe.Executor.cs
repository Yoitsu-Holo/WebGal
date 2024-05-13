using System.Reflection;
using WebGal.Extend.Collections;
using WebGal.Global;

namespace WebGal.MeoInterpreter;

public partial class MoeInterpreter
{

	public static object Call(FuncntionNode function, List<MoeVariable> paramList) // 调用函数
	{
		FunctionHeader header = function.Header;
		ProgramNode body = function.Body;
		MoeStackFrame frame = new();
		ActiveTasks.Push(frame);

		if (header.CallParam.Count != paramList.Count)
		{
			Logger.LogInfo($"参数列表数量不匹配 {function}", Global.LogLevel.Error);
			return new();
		}

		for (int i = 0; i < header.CallParam.Count; i++)
		{
			if (header.CallParam[i].Type == paramList[i].Type)
			{
				frame.LVariable[header.CallParam[i].Name] = (MoeVariable)paramList[i].Clone();
			}
			else
			{
				Logger.LogInfo($"参数列表类型不匹配 {function}", Global.LogLevel.Error);
				return new();
			}
		}

		frame.PC.Push(0);
		frame.CodeBlock.Push(body);
		frame.BlockVarName.Push([]);

		Run();

		return new();
	}

	public static object Call(FunctionCallNode funcntion) // 调用函数
	{
		if (ActiveTasks.Count == 0)
			throw new Exception(Logger.LogMessage("任务栈未初始化"));
		List<MoeVariable> paramList = [];
		foreach (string varName in funcntion.ParamName)
		{
			if (GVariables.TryGetValue(varName, out MoeVariable? gvalue))
				paramList.Add(gvalue);
			else if (ActiveTasks.Peek().LVariable.TryGetValue(varName, out MoeVariable? lvalue))
				paramList.Add(lvalue);
			else
				Logger.LogInfo($"静态参数传递未完全实现", Global.LogLevel.Todo);
		}

		// 系统保留
		if (funcntion.FunctionName[0] == '_')
		{
			UserCall(funcntion.FunctionName[1..], paramList);
			return new();
		}

		return Call(Functions[funcntion.FunctionName], paramList);
	}

	public static void UserCall(string syscall, List<MoeVariable> paramList)
	{
		if (syscall.Length == 0)
		{
			Logger.LogInfo($"错误的用户函数名称{syscall}", Global.LogLevel.Warning);
			return;
		}
		if (syscall[0] == '_')
		{
			SysCall(syscall[1..], paramList);
			return;
		}
		InnerCall(syscall, paramList);
	}

	public static void SysCall(string syscall, List<MoeVariable> paramList)
	{
		if (syscall.Length == 0)
		{
			Logger.LogInfo($"错误的系统内建函数名称{syscall}", Global.LogLevel.Warning);
			return;
		}
		InnerCall(syscall, paramList);
	}

	public static void InnerCall(string syscall, List<MoeVariable> paramList)
	{
		Type type = typeof(Syscall);
		MethodInfo? method = type.GetMethod(syscall, BindingFlags.Public | BindingFlags.Static, [typeof(List<MoeVariable>)]);
		if (method is null)
		{
			Logger.LogInfo($"未知的系统内建函数名称{syscall}", Global.LogLevel.Warning);
			return;
		}
		method.Invoke(null, [paramList]);
	}

	public static void Run() //运行代码块
	{
		while (ActiveTasks.Count > 0)
		{
			MoeStackFrame frame = ActiveTasks.Peek();
			if (frame.PC.Count == 0)
				break;

			int index = frame.PC.Peek();
			ProgramNode now = frame.CodeBlock.Peek();

			if (index >= now.Statements.Count) // 到达代码块结尾，跳出代码块
			{
				foreach (var varName in frame.BlockVarName.Peek())
					frame.LVariable.Remove(varName);
				frame.PC.Pop();
				frame.CodeBlock.Pop();
				frame.BlockVarName.Pop();
				continue;
			}

			ASTNode ast = now.Statements[index];

			frame.PC.Pop();
			frame.PC.Push(index + 1);  // 放入下一条语句

			if (ast.ASTType != ASTNodeType.Void)
				Execute(ast, frame); // 执行当前语句
		}
	}

	public static void Execute(ASTNode ast, MoeStackFrame frame)
	{
		if (ast.ASTType == ASTNodeType.Void)
			return;
		if (ast.ASTType == ASTNodeType.Error)
		{
			Logger.LogInfo($"语法解析树中存在错误节点", Global.LogLevel.Error);
			return;
		}


		if (ast.ASTType == ASTNodeType.Program && ast.Program is not null)
		{
			frame.PC.Push(0);
			frame.CodeBlock.Push(ast.Program);
			frame.BlockVarName.Push([]);
		}
		else if (ast.ASTType == ASTNodeType.VariableDeclaration && ast.VarDefine is not null)
		{
			foreach (var variable in ast.VarDefine.Variables)
			{
				if (frame.BlockVarName.Peek().Contains(variable.Name) || GVariables.ContainsKey(variable.Name))
				{
					Logger.LogInfo($"变量名称重复定义 {variable}", Global.LogLevel.Error);
					continue;
				}

				frame.BlockVarName.Peek().Add(variable.Name);
				frame.LVariable.Add(variable.Name, (MoeVariable)variable.Clone());
				frame.LVariable[variable.Name].Init();
			}
		}
		else if (ast.ASTType == ASTNodeType.Assignment && ast.Assignment is not null)
		{
			var LeftVar = ast.Assignment.LeftVar;
			ExpressionNode? RightExp = ast.Assignment.RightExp;
			MoeVariable? Left = null;
			object Right = new();

			if (Left is null)
				GVariables.TryGetValue(LeftVar.Name, out Left);
			if (Left is null)
				frame.LVariable.TryGetValue(LeftVar.Name, out Left);

			if (Left is null)
			{
				Logger.LogInfo($"未找到变量定义 {Left}", Global.LogLevel.Warning);
				return;
			}

			if (Left.Access == MoeVariableAccess.Const)
			{
				Logger.LogInfo($"不能对于常量修改 {Left}", Global.LogLevel.Warning);
				return;
			}

			if (RightExp is not null)
				Right = ExpressionsExecutor.Parse(RightExp);
			else if (ast.Assignment.FuncCall is not null)
				Right = Call(ast.Assignment.FuncCall);

			List<int> index = [];
			foreach (var exp in LeftVar.Index)
				index.Add((int)ExpressionsExecutor.Parse(exp));

			if (Right is int && Left.Type == MoeVariableType.Int)
				Left[index] = Right;
			else if (Right is double && Left.Type == MoeVariableType.Double)
				Left[index] = Right;
			else if (Right is string && Left.Type == MoeVariableType.String)
				Logger.LogInfo("暂未实现字符串赋值解析", Global.LogLevel.Todo);
			else
				Logger.LogInfo("变量类型不匹配", Global.LogLevel.Error);

		}
		else if (ast.ASTType == ASTNodeType.Conditional && ast.IfCase is not null)
		{
			IfCaseNode ifCase = ast.IfCase;
			for (int i = 0; i < ifCase.If.Count; i++)
			{
				ConditionalNode conditional = ifCase.If[i];
				object result = ExpressionsExecutor.Parse(conditional.Conditional);

				if (result is not int && result is not double)
				{
					Logger.LogInfo($"条件表达式值只接受数学表达式", Global.LogLevel.Error);
					continue;
				}

				if (result is int resultInt && resultInt == 0)
					continue;
				if (result is double resultDouble && resultDouble == 0)
					continue;

				// Run(frame, conditional.Program);
				frame.PC.Push(0);
				frame.CodeBlock.Push(conditional.Program);
				frame.BlockVarName.Push([]);
				break;
			}
		}
		else if (ast.ASTType == ASTNodeType.Loop && ast.Loop is not null)
		{
			ConditionalNode loop = ast.Loop.Loop;

			object result = ExpressionsExecutor.Parse(loop.Conditional);

			if (result is not int && result is not double)
			{
				Logger.LogInfo($"条件表达式值只接受数学表达式", Global.LogLevel.Error);
				return;
			}

			if (result is int resultInt && resultInt == 0)
				return;
			if (result is double resultDouble && resultDouble == 0)
				return;

			int index = frame.PC.Pop();
			frame.PC.Push(index - 1);

			frame.PC.Push(0);
			frame.CodeBlock.Push(loop.Program);
			frame.BlockVarName.Push([]);

			// Log.LogInfo("循环解析", Global.LogLevel.Todo);
		}
		else if (ast.ASTType == ASTNodeType.LoopControl && ast.LoopControl is not null)
		{
			Logger.LogInfo("循环控制解析", Global.LogLevel.Todo);
		}
		else if (ast.ASTType == ASTNodeType.FunctionCall && ast.FunctionCall is not null)
		{
			Call(ast.FunctionCall);
		}
		else
			Logger.LogInfo("这个 ast 节点总得有点错 {ast}", Global.LogLevel.Error);
	}


	public class ExpressionsExecutor
	{
		// ^ ================================================================
		// private ExpressionNode exp = null!;
		// private List<ExpressionToken> _tokens => exp.Tokens;
		// private int index;
		// // private ExpressionToken CurrentToken => exp.Tokens[index];

		// private ExpressionToken CurrentToken => index < _tokens.Count ? _tokens[index] : new();

		/// <summary>
		/// 返回类型为 int 或者 double 的结果
		/// </summary>
		/// <param name="tokens"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public static object Parse(ExpressionNode expression) => Parse(expression.Tokens);

		public static object Parse(List<ExpressionToken> expression)
		{
			DoubleEndEnumerator<ExpressionToken> tokens = new(expression);
			tokens.MoveNext();
			object result = Level15(tokens);
			if (tokens.IsEnd == false)
				throw new Exception(Logger.LogMessage($"Unexpected token {tokens}"));
			return result;
		}

		private static ExpressionToken ConsumeToken(OperatorType type, IExtendEnumerator<ExpressionToken> tokens)
		{
			if (tokens.IsEnd)
				throw new Exception($"已到达表达式结尾");
			ExpressionToken token = tokens.Current;
			tokens.MoveNext();
			if (token.Type != type)
				throw new Exception($"Unexpected token: {token.Type} != {type}");
			return token;
		}

		/// <summary>
		///;	,		逗号
		/// </summary>
		private static object Level15(IExtendEnumerator<ExpressionToken> tokens)
		{
			// return Level14();
			return Level12(tokens);
		}

		/// <summary>
		///;	=		简单赋值
		///;	+= -=	以和及差赋值
		///;	*= /= %=	以积、商及余数赋值
		///;	<<= >>=	以逐位左移及右移赋值
		///;	&= ^= |=	以逐位与、异或及或赋值
		/// </summary>
		private static object Level14(IExtendEnumerator<ExpressionToken> tokens)
		{
			return Level13(tokens);
		}


		/// <summary>
		///;	?:		三元条件
		/// </summary>
		private static object Level13(IExtendEnumerator<ExpressionToken> tokens)
		{
			return Level12(tokens);
		}

		/// <summary>
		///;	||		逻辑或
		/// </summary>
		private static object Level12(IExtendEnumerator<ExpressionToken> tokens)
		{
			object result = Level11(tokens);
			OperatorType opType = tokens.IsEnd ? new() : tokens.Current.Type;
			while (opType == OperatorType.OR && tokens.IsEnd == false)
			{
				ConsumeToken(opType, tokens);
				object value = Level11(tokens);
				result = Calc(result, value, opType);
				opType = tokens.IsEnd ? new() : tokens.Current.Type;
			}
			return result;
		}

		/// <summary>
		///;	&&		逻辑与
		/// </summary>
		private static object Level11(IExtendEnumerator<ExpressionToken> tokens)
		{
			object result = Level10(tokens);
			OperatorType opType = tokens.IsEnd ? new() : tokens.Current.Type;
			while (opType == OperatorType.AND && tokens.IsEnd == false)
			{
				ConsumeToken(opType, tokens);
				object value = Level10(tokens);
				result = Calc(result, value, opType);
				opType = tokens.IsEnd ? new() : tokens.Current.Type;
			}
			return result;
		}

		/// <summary>
		///;	|		逐位或（包含或）
		/// </summary>
		private static object Level10(IExtendEnumerator<ExpressionToken> tokens)
		{
			object result = Level9(tokens);
			OperatorType opType = tokens.IsEnd ? new() : tokens.Current.Type;
			while (opType == OperatorType.bOR && tokens.IsEnd == false)
			{
				ConsumeToken(opType, tokens);
				object value = Level9(tokens);
				result = Calc(result, value, opType);
				opType = tokens.IsEnd ? new() : tokens.Current.Type;
			}
			return result;
		}

		/// <summary>
		///;	^		逐位异或（排除或）
		/// </summary>
		private static object Level9(IExtendEnumerator<ExpressionToken> tokens)
		{
			object result = Level8(tokens);
			OperatorType opType = tokens.IsEnd ? new() : tokens.Current.Type;
			while (opType == OperatorType.XOR && tokens.IsEnd == false)
			{
				ConsumeToken(opType, tokens);
				object value = Level8(tokens);
				result = Calc(result, value, opType);
				opType = tokens.IsEnd ? new() : tokens.Current.Type;
			}
			return result;
		}

		/// <summary>
		///;	&		逐位与
		/// </summary>
		private static object Level8(IExtendEnumerator<ExpressionToken> tokens)
		{
			object result = Level7(tokens);
			OperatorType opType = tokens.IsEnd ? new() : tokens.Current.Type;
			while (opType == OperatorType.bAND && tokens.IsEnd == false)
			{
				ConsumeToken(opType, tokens);
				object value = Level7(tokens);
				result = Calc(result, value, opType);
				opType = tokens.IsEnd ? new() : tokens.Current.Type;
			}
			return result;
		}

		/// <summary>
		///;	== !=	分别为 = 与 ≠ 关系
		/// </summary>
		private static object Level7(IExtendEnumerator<ExpressionToken> tokens)
		{
			object result = Level6(tokens);
			OperatorType opType = tokens.IsEnd ? new() : tokens.Current.Type;
			while ((opType == OperatorType.EQ || opType == OperatorType.NEQ) && tokens.IsEnd == false)
			{
				ConsumeToken(opType, tokens);
				object value = Level6(tokens);
				result = Calc(result, value, opType);
				opType = tokens.IsEnd ? new() : tokens.Current.Type;
			}
			return result;
		}

		/// <summary>
		///;	< <=	分别为 < 与 ≤ 的关系运算符
		///;	> >=	分别为 > 与 ≥ 的关系运算符
		/// </summary>
		private static object Level6(IExtendEnumerator<ExpressionToken> tokens)
		{
			object result = Level5(tokens);
			OperatorType opType = tokens.IsEnd ? new() : tokens.Current.Type;
			while ((opType == OperatorType.GT || opType == OperatorType.LT || opType == OperatorType.EGT || opType == OperatorType.ELT) && tokens.IsEnd == false)
			{
				ConsumeToken(opType, tokens);
				object value = Level5(tokens);
				result = Calc(result, value, opType);
				opType = tokens.IsEnd ? new() : tokens.Current.Type;
			}
			return result;
		}

		/// <summary>
		///; 	<< >>	逐位左移及右移
		/// </summary>
		private static object Level5(IExtendEnumerator<ExpressionToken> tokens)
		{
			object result = Level4(tokens);
			OperatorType opType = tokens.IsEnd ? new() : tokens.Current.Type;
			while ((opType == OperatorType.SHL || opType == OperatorType.SHR) && tokens.IsEnd == false)
			{
				ConsumeToken(opType, tokens);
				object value = Level4(tokens);
				result = Calc(result, value, opType);
				opType = tokens.IsEnd ? new() : tokens.Current.Type;
			}
			return result;
		}

		/// <summary>
		///;	+ -		加法及减法
		/// </summary>
		private static object Level4(IExtendEnumerator<ExpressionToken> tokens)
		{
			object result = Level3(tokens);
			OperatorType opType = tokens.IsEnd ? new() : tokens.Current.Type;
			while ((opType == OperatorType.ADD || opType == OperatorType.SUB) && tokens.IsEnd == false)
			{
				ConsumeToken(opType, tokens);
				object value = Level3(tokens);
				result = Calc(result, value, opType);
				opType = tokens.IsEnd ? new() : tokens.Current.Type;
			}
			return result;
		}


		/// <summary>
		///;	* / %	乘法、除法及余数
		/// </summary>
		private static object Level3(IExtendEnumerator<ExpressionToken> tokens)
		{
			object result = Level2(tokens);
			OperatorType opType = tokens.IsEnd ? new() : tokens.Current.Type;
			while ((opType == OperatorType.MUL || opType == OperatorType.DIV || opType == OperatorType.MOD) && tokens.IsEnd == false)
			{
				ConsumeToken(opType, tokens);
				object value = Level2(tokens);
				result = Calc(result, value, opType);
				opType = tokens.IsEnd ? new() : tokens.Current.Type;
			}
			return result;
		}

		/// <summary>
		///; 	++ --	前缀自增与自减
		///;	+ -		一元加与减
		///;	! ~		逻辑非与逐位非
		///;	(type)	转型
		///;	*		间接（解引用）
		///;	&		取址
		/// </summary>
		private static object Level2(IExtendEnumerator<ExpressionToken> tokens)
		{
			object result = Level1(tokens);
			OperatorType opType = tokens.IsEnd ? new() : tokens.Current.Type;
			if ((opType == OperatorType.Minus || opType == OperatorType.bNOT || opType == OperatorType.NOT) && tokens.IsEnd == false)
			{
				ConsumeToken(opType, tokens);
				object ret = Level1(tokens);
				return Calc(ret, 0, opType);
			}
			return result;
		}

		/// <summary>
		/// () number variable
		/// </summary>
		private static object Level1(IExtendEnumerator<ExpressionToken> tokens)
		{
			OperatorType opType = tokens.IsEnd ? new() : tokens.Current.Type;
			if (opType == OperatorType.NUM)
			{
				return ConsumeToken(OperatorType.NUM, tokens).Number;
			}
			else if (opType == OperatorType.VAR)
			{
				VariableInfo variableInfo = tokens.Current.Var;
				ConsumeToken(OperatorType.VAR, tokens);

				GVariables.TryGetValue(variableInfo.Name, out MoeVariable? variable);
				if (variable is null)
					LVariables.TryGetValue(variableInfo.Name, out variable);

				if (variable is null)
					throw new Exception(Logger.LogMessage($"未找到变量定义 {variableInfo}"));
				// throw new Exception($"Todo {variableInfo}");

				List<int> indexs = [];
				foreach (var item in variableInfo.Index)
				{
					object ret = Parse(item);
					if (ret is int index)
						indexs.Add(index);
					else
						throw new Exception(Logger.LogMessage($"数组下标必须为整数 {ret}"));
				}
				return variable[indexs];
				// return variable[variableInfo.Index];
			}
			else if (opType == OperatorType.LeftParen)
			{
				ConsumeToken(OperatorType.LeftParen, tokens);
				object result = Level15(tokens);
				ConsumeToken(OperatorType.RightParen, tokens);
				return result;
			}
			// else if (opType == OperatorType.LeftParen)
			// {

			// }
			else
				throw new Exception($"Unexpected token: {tokens.Current}");
		}


		private static object Calc(object v1, object v2, OperatorType type)
		{
			int v1i, v2i;
			double v1f, v2f;
			bool flag = true;

			{
				if (v1 is int vv1)
					(v1i, v1f) = (vv1, vv1);
				else if (v1 is double vv2)
					(v1i, v1f, flag) = ((int)vv2, vv2, false);
				else throw new Exception(Logger.LogMessage("非 int 或者 double 类型"));
			}
			{
				if (v2 is int vv1)
					(v2i, v2f) = (vv1, vv1);
				else if (v2 is double vv2)
					(v2i, v2f, flag) = ((int)vv2, vv2, false);
				else throw new Exception(Logger.LogMessage("非 int 或者 double 类型"));
			}

			if (flag)
				return type switch
				{
					OperatorType.ADD => v1i + v2i,
					OperatorType.SUB => v1i - v2i,
					OperatorType.MUL => v1i * v2i,
					OperatorType.DIV => v1i / v2i,
					OperatorType.MOD => v1i % v2i,
					OperatorType.POW => (int)Math.Pow(v1i, v2i),

					OperatorType.bAND => v1i & v2i,
					OperatorType.bOR => v1i | v2i,
					OperatorType.XOR => v1i ^ v2i,

					OperatorType.SHL => v1i << v2i,
					OperatorType.SHR => v1i >> v2i,

					OperatorType.EQ => v1i == v2i ? 1 : 0,
					OperatorType.NEQ => v1i != v2i ? 1 : 0,
					OperatorType.GT => v1i > v2i ? 1 : 0,
					OperatorType.LT => v1i < v2i ? 1 : 0,
					OperatorType.EGT => v1i >= v2i ? 1 : 0,
					OperatorType.ELT => v1i <= v2i ? 1 : 0,

					OperatorType.AND => ((v1i != 0) && (v2i != 0)) ? 1 : 0,
					OperatorType.OR => ((v1i != 0) || (v2i != 0)) ? 1 : 0,

					OperatorType.Minus => -v1i,
					OperatorType.bNOT => ~v1i,
					OperatorType.NOT => (v1i == 0) ? 1 : 0,
					_ => throw new Exception(Logger.LogMessage($"运算符 {type} 未在整数实现")),
				};
			else
				return type switch
				{
					OperatorType.ADD => v1f + v2f,
					OperatorType.SUB => v1f - v2f,
					OperatorType.MUL => v1f * v2f,
					OperatorType.DIV => v1f / v2f,
					OperatorType.MOD => v1f % v2f,
					OperatorType.POW => Math.Pow(v1f, v2f),

					OperatorType.EQ => v1f == v2f ? 1 : 0,
					OperatorType.NEQ => v1f != v2f ? 1 : 0,
					OperatorType.GT => v1f > v2f ? 1 : 0,
					OperatorType.LT => v1f < v2f ? 1 : 0,
					OperatorType.EGT => v1f >= v2f ? 1 : 0,
					OperatorType.ELT => v1f <= v2f ? 1 : 0,

					OperatorType.AND => ((v1f != 0) && (v2f != 0)) ? 1 : 0,
					OperatorType.OR => ((v1f != 0) || (v2f != 0)) ? 1 : 0,

					OperatorType.Minus => -v1f,
					OperatorType.NOT => (v1f == 0) ? 1 : 0,
					// _ => Log.LogInfo($"运算符 {type} 未实现", Global.LogLevel.Todo),
					_ => throw new Exception(Logger.LogMessage($"运算符 {type} 未在浮点数实现")),
				};
		}
	}
}