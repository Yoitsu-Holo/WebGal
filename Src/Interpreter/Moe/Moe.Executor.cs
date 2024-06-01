using System.Reflection;
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
			Logger.LogInfo($"参数列表数量不匹配 {function.Header}\n {header.CallParam.Count} : {paramList.Count}", Global.LogLevel.Error);
			return new();
		}

		Console.WriteLine(function.Header);
		for (int i = 0; i < header.CallParam.Count; i++)
		{
			if (header.CallParam[i].Type == paramList[i].Type)
			{
				frame.LVariable[header.CallParam[i].Name] = new();
				frame.LVariable[header.CallParam[i].Name].CloneFrom(paramList[i]);
			}
			else if (paramList[i].Type == MoeVariableType.Void)
				continue;
			else
			{
				Logger.LogInfo($"参数列表类型不匹配 {header.CallParam[i].Type}:{paramList[i].Type}\n{function.Header}", Global.LogLevel.Error);
				return new();
			}
		}

		frame.PC.Push(0);
		frame.CodeBlock.Push(body);
		frame.BlockVarName.Push([]);

		Run(ActiveTasks.Peek());
		MoeVariable returnData = frame.ReturnData;
		ActiveTasks.Pop();
		if (returnData.Size != 0)
			return returnData[0];
		return returnData;
	}

	public static object Call(FunctionCallNode function) // 调用函数
	{
		// 系统调用，只能关键字传参
		if (function.FunctionName[0] == '_')
		{
			Dictionary<string, MoeVariable> paramtersList = [];
			foreach (var (name, exp) in function.KeywordParams)
				paramtersList[name] = ParseCallValue(exp);
			object? obj = UserCall(function.FunctionName[1..], paramtersList);
			return (obj is null) ? new() : obj;
		}

		// 如果为非系统调用，尝试本地调用
		List<MoeVariable> paramList = [];
		FuncntionNode funcntionNode = Functions[function.FunctionName];
		if (function.CallType == FuncCallType.Positional)
		{
			foreach (ExpressionNode exp in function.PositionalParams)
				paramList.Add(ParseCallValue(exp));
		}
		else if (function.CallType == FuncCallType.Keyword)
		{
			List<MoeVariable> CallParam = funcntionNode.Header.CallParam;
			foreach (var param in CallParam)
			{
				string paramName = param.Name;
				if (function.KeywordParams.TryGetValue(paramName, out ExpressionNode? exp))
					paramList.Add(ParseCallValue(exp));
				else
					paramList.Add(new());
			}
		}
		return Call(funcntionNode, paramList);
	}

	public static object? UserCall(string usercall, Dictionary<string, MoeVariable> paramList)
	{
		if (usercall.Length == 0)
		{
			Logger.LogInfo($"错误的用户函数名称{usercall}", Global.LogLevel.Warning);
			return null;
		}
		if (usercall[0] == '_')
		{
			SysCall(usercall[1..], paramList);
			return null;
		}
		Logger.LogInfo("用户函数未实现", Global.LogLevel.Todo);
		return InnerCall(usercall, typeof(Syscall), paramList);
	}

	public static object? SysCall(string syscall, Dictionary<string, MoeVariable> paramList)
	{
		if (syscall.Length == 0)
		{
			Logger.LogInfo($"错误的系统内建函数名称: {syscall}", Global.LogLevel.Warning);
			return null;
		}
		return InnerCall(syscall, typeof(Syscall), paramList);
	}

	public static object? InnerCall(string syscall, Type type, Dictionary<string, MoeVariable> paramList)
	{
		MethodInfo? method = type.GetMethod(syscall, BindingFlags.Public | BindingFlags.Static);
		if (method is null)
		{
			Logger.LogInfo($"未知的系统内建函数名称: {syscall}", Global.LogLevel.Warning);
			return null;
		}
		ParameterInfo[] parameters = method.GetParameters();
		object[] args = new object[parameters.Length];

		for (int i = 0; i < parameters.Length; i++)
		{
			if (paramList.TryGetValue(parameters[i].Name!, out MoeVariable? value))
				args[i] = value;
			else
				args[i] = new MoeVariable();
		}

		return method.Invoke(null, args);
	}

	public static void Run(MoeStackFrame frame) //运行代码块
	{
		while (frame.PC.Count != 0)
		{
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
				MoeVariable lvar = new();
				lvar.CloneFrom(variable);
				frame.LVariable.Add(variable.Name, lvar);
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
			else if (Right is float && Left.Type == MoeVariableType.Float)
				Left[index] = Right;
			else if (Right is string && Left.Type == MoeVariableType.String)
				Left[index] = Right;
			else
				Logger.LogInfo($"变量类型不匹配\nRight Type: {Right.GetType()}\n{Left} <:: {Right}", Global.LogLevel.Error);
		}
		else if (ast.ASTType == ASTNodeType.Conditional && ast.IfCase is not null)
		{
			IfCaseNode ifCase = ast.IfCase;
			for (int i = 0; i < ifCase.If.Count; i++)
			{
				ConditionalNode conditional = ifCase.If[i];
				object result = ExpressionsExecutor.Parse(conditional.Conditional);

				if (result is not int && result is not float)
				{
					Logger.LogInfo($"条件表达式值只接受数学表达式", Global.LogLevel.Error);
					continue;
				}

				if (result is int resultInt && resultInt == 0)
					continue;
				if (result is float resultDouble && resultDouble == 0)
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

			if (result is not int && result is not float)
			{
				Logger.LogInfo($"条件表达式值只接受数学表达式", Global.LogLevel.Error);
				return;
			}

			if (result is int resultInt && resultInt == 0)
				return;
			if (result is float resultDouble && resultDouble == 0)
				return;

			int index = frame.PC.Pop();
			frame.PC.Push(index - 1);

			frame.PC.Push(0);
			frame.CodeBlock.Push(loop.Program);
			frame.BlockVarName.Push([]);
		}
		else if (ast.ASTType == ASTNodeType.LoopControl && ast.LoopControl is not null)
		{
			Logger.LogInfo("循环控制解析", Global.LogLevel.Todo);
		}
		else if (ast.ASTType == ASTNodeType.FunctionCall && ast.FunctionCall is not null)
		{
			Call(ast.FunctionCall);
		}
		else if (ast.ASTType == ASTNodeType.Return && ast.Return is not null)
		{
			object obj = ExpressionsExecutor.Parse(ast.Return.ReturnExp);
			frame.ReturnData = new(obj);
			// Console.WriteLine($"{obj} : {frame.ReturnData[0]}");
		}
		else
			Logger.LogInfo($"这个 ast 节点总得有点错 {ast}", Global.LogLevel.Error);
	}


	public static MoeVariable ParseCallValue(ExpressionNode exp)
	{
		if (exp.IsVarName)
		{
			string varName = exp.Tokens[0].Var.Name;
			if (GVariables.TryGetValue(varName, out MoeVariable? gvalue))
				return gvalue;
			else if (ActiveTasks.Peek().LVariable.TryGetValue(varName, out MoeVariable? lvalue))
				return lvalue;
			else
				Logger.LogInfo($"静态参数传递未完全实现", Global.LogLevel.Todo);
			return new();
		}
		else
		{
			object value = ExpressionsExecutor.Parse(exp);
			MoeVariable variable = new();
			if (value is int vint)
			{
				variable = new(MoeVariableAccess.Variable, MoeVariableType.Int)
				{
					Name = value.GetHashCode().ToString(),
					Dimension = [1],
				};
				variable[0] = vint;
			}
			else if (value is float vfloat)
			{
				variable = new(MoeVariableAccess.Variable, MoeVariableType.Float)
				{
					Name = value.GetHashCode().ToString(),
					Dimension = [1],
				};
				variable[0] = vfloat;
			}
			else if (value is string vstring)
			{
				variable = new(MoeVariableAccess.Variable, MoeVariableType.String)
				{
					Name = value.GetHashCode().ToString(),
					Dimension = [1],
				};
				variable[0] = vstring;
			}
			else
				Logger.LogInfo("未匹配的参数");
			return variable;
		}
	}
}