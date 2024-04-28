using WebGal.Global;

namespace WebGal.MeoInterpreter;

public partial class MoeInterpreter
{
	public class ExpressionsExecutor
	{
		// ^ ================================================================
		private readonly ExpressionNode exp;
		private int index;
		// private ExpressionToken CurrentToken => exp.Tokens[index];

		private ExpressionToken CurrentToken => index < exp.Tokens.Count ? exp.Tokens[index] : new ExpressionToken { Type = OperatorType.Void };

		public ExpressionsExecutor(ExpressionNode tokens)
		{
			exp = tokens;
			index = 0;
		}

		public object Parse()
		{
			object result = Level1();
			if (index != exp.Tokens.Count)
				throw new Exception(Log.LogMessage("Unexpected token"));
			return result;
		}


		private ExpressionToken ConsumeToken(OperatorType type)
		{
			if (CurrentToken.Type != type)
			{
				throw new Exception($"Unexpected token: {CurrentToken.Type}");
			}
			index++;
			return exp.Tokens[index - 1];
		}

		private object Level1()
		{
			object result = Level2();
			OperatorType opType = CurrentToken.Type;
			while ((CurrentToken.Type == OperatorType.ADD || CurrentToken.Type == OperatorType.SUB) && index < exp.Tokens.Count)
			{
				ConsumeToken(opType);
				object value = Level2();
				result = Calc(result, value, opType);
			}
			return result;
		}

		private object Level2()
		{
			object result = Level3();
			OperatorType opType = CurrentToken.Type;
			while ((CurrentToken.Type == OperatorType.MUL || CurrentToken.Type == OperatorType.DIV) && index < exp.Tokens.Count)
			{
				ConsumeToken(opType);
				object value = Level3();
				result = Calc(result, value, opType);
			}
			return result;
		}

		private object Level3()
		{
			if (CurrentToken.Type == OperatorType.NUM)
			{
				return ConsumeToken(OperatorType.NUM).Number;
			}
			else if (CurrentToken.Type == OperatorType.LeftParen)
			{
				ConsumeToken(OperatorType.LeftParen);
				object result = Level1();
				ConsumeToken(OperatorType.RightParen);
				return result;
			}
			else
				throw new Exception($"Unexpected token: {CurrentToken.Type}");
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
				else throw new Exception(Log.LogMessage("非 int 或者 double 类型"));
			}
			{
				if (v2 is int vv1)
					(v2i, v2f) = (vv1, vv1);
				else if (v2 is double vv2)
					(v2i, v2f, flag) = ((int)vv2, vv2, false);
				else throw new Exception(Log.LogMessage("非 int 或者 double 类型"));
			}

			Console.WriteLine($"{v1 is int} {v1} {v2 is int} {v2}");
			if (flag)
				return type switch
				{
					OperatorType.ADD => v1i + v2i,
					OperatorType.SUB => v1i - v2i,
					OperatorType.MUL => v1i * v2i,
					OperatorType.DIV => v1i / v2i,
					OperatorType.MOD => v1i % v2i,
					// _ => Log.LogInfo($"运算符 {type} 未实现", Global.LogLevel.Todo),
					_ => throw new Exception(Log.LogMessage($"运算符 {type} 未实现")),
				};
			else
				return type switch
				{
					OperatorType.ADD => v1f + v2f,
					OperatorType.SUB => v1f - v2f,
					OperatorType.MUL => v1f * v2f,
					OperatorType.DIV => v1f / v2f,
					OperatorType.MOD => v1f % v2f,
					// _ => Log.LogInfo($"运算符 {type} 未实现", Global.LogLevel.Todo),
					_ => throw new Exception(Log.LogMessage($"运算符 {type} 未实现")),
				};
		}
	}
}