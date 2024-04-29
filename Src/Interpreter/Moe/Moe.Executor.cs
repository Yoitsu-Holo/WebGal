using WebGal.Global;

namespace WebGal.MeoInterpreter;

public partial class MoeInterpreter
{
	public class ExpressionsExecutor
	{
		// ^ ================================================================
		private ExpressionNode exp = null!;
		private int index;
		// private ExpressionToken CurrentToken => exp.Tokens[index];

		private ExpressionToken CurrentToken => exp.Tokens[index];

		/// <summary>
		/// 返回类型为 int 或者 double 的结果
		/// </summary>
		/// <param name="tokens"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public object Parse(ExpressionNode tokens)
		{
			exp = tokens;
			index = 0;
			object result = Level15();
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

		/// <summary>
		///;	,		逗号
		/// </summary>
		private object Level15()
		{
			return Level14();
		}

		/// <summary>
		///;	=		简单赋值
		///;	+= -=	以和及差赋值
		///;	*= /= %=	以积、商及余数赋值
		///;	<<= >>=	以逐位左移及右移赋值
		///;	&= ^= |=	以逐位与、异或及或赋值
		/// </summary>
		private object Level14()
		{
			return Level13();
		}


		/// <summary>
		///;	?:		三元条件
		/// </summary>
		private object Level13()
		{
			return Level12();
		}

		/// <summary>
		///;	||		逻辑或
		/// </summary>
		private object Level12()
		{
			object result = Level11();
			OperatorType opType = CurrentToken.Type;
			while (opType == OperatorType.OR && index < exp.Tokens.Count)
			{
				ConsumeToken(opType);
				object value = Level11();
				result = Calc(result, value, opType);
			}
			return result;
		}

		/// <summary>
		///;	&&		逻辑与
		/// </summary>
		private object Level11()
		{
			object result = Level10();
			OperatorType opType = CurrentToken.Type;
			while (opType == OperatorType.AND && index < exp.Tokens.Count)
			{
				ConsumeToken(opType);
				object value = Level10();
				result = Calc(result, value, opType);
			}
			return result;
		}

		/// <summary>
		///;	|		逐位或（包含或）
		/// </summary>
		private object Level10()
		{
			object result = Level9();
			OperatorType opType = CurrentToken.Type;
			while (opType == OperatorType.bOR && index < exp.Tokens.Count)
			{
				ConsumeToken(opType);
				object value = Level9();
				result = Calc(result, value, opType);
			}
			return result;
		}

		/// <summary>
		///;	^		逐位异或（排除或）
		/// </summary>
		private object Level9()
		{
			object result = Level8();
			OperatorType opType = CurrentToken.Type;
			while (opType == OperatorType.XOR && index < exp.Tokens.Count)
			{
				ConsumeToken(opType);
				object value = Level8();
				result = Calc(result, value, opType);
			}
			return result;
		}

		/// <summary>
		///;	&		逐位与
		/// </summary>
		private object Level8()
		{
			object result = Level7();
			OperatorType opType = CurrentToken.Type;
			while (opType == OperatorType.bAND && index < exp.Tokens.Count)
			{
				ConsumeToken(opType);
				object value = Level7();
				result = Calc(result, value, opType);
			}
			return result;
		}

		/// <summary>
		///;	== !=	分别为 = 与 ≠ 关系
		/// </summary>
		private object Level7()
		{
			object result = Level6();
			OperatorType opType = CurrentToken.Type;
			while ((opType == OperatorType.EQ || opType == OperatorType.NEQ) && index < exp.Tokens.Count)
			{
				ConsumeToken(opType);
				object value = Level6();
				result = Calc(result, value, opType);
			}
			return result;
		}

		/// <summary>
		///;	< <=	分别为 < 与 ≤ 的关系运算符
		///;	> >=	分别为 > 与 ≥ 的关系运算符
		/// </summary>
		private object Level6()
		{
			object result = Level5();
			OperatorType opType = CurrentToken.Type;
			while ((opType == OperatorType.GT || opType == OperatorType.LT || opType == OperatorType.EGT || opType == OperatorType.ELT) && index < exp.Tokens.Count)
			{
				ConsumeToken(opType);
				object value = Level5();
				result = Calc(result, value, opType);
			}
			return result;
		}

		/// <summary>
		///; 	<< >>	逐位左移及右移
		/// </summary>
		private object Level5()
		{
			object result = Level4();
			OperatorType opType = CurrentToken.Type;
			while ((opType == OperatorType.SHL || opType == OperatorType.SHR) && index < exp.Tokens.Count)
			{
				ConsumeToken(opType);
				object value = Level4();
				result = Calc(result, value, opType);
			}
			return result;
		}

		/// <summary>
		///;	+ -		加法及减法
		/// </summary>
		private object Level4()
		{
			object result = Level3();
			OperatorType opType = CurrentToken.Type;
			while ((opType == OperatorType.ADD || opType == OperatorType.SUB) && index < exp.Tokens.Count)
			{
				ConsumeToken(opType);
				object value = Level3();
				result = Calc(result, value, opType);
			}
			return result;
		}


		/// <summary>
		///;	* / %	乘法、除法及余数
		/// </summary>
		private object Level3()
		{
			object result = Level2();
			OperatorType opType = CurrentToken.Type;
			while ((opType == OperatorType.MUL || opType == OperatorType.DIV || opType == OperatorType.MOD) && index < exp.Tokens.Count)
			{
				ConsumeToken(opType);
				object value = Level2();
				result = Calc(result, value, opType);
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
		private object Level2()
		{
			OperatorType opType = CurrentToken.Type;
			if ((opType == OperatorType.Minus || opType == OperatorType.bNOT || opType == OperatorType.NOT) && index < exp.Tokens.Count)
			{
				ConsumeToken(opType);
				object ret = Level1();
				return Calc(ret, 0, opType);
			}
			return Level1();
		}

		/// <summary>
		/// () number variable
		/// </summary>
		private object Level1()
		{
			OperatorType opType = CurrentToken.Type;
			if (opType == OperatorType.NUM)
			{
				return ConsumeToken(OperatorType.NUM).Number;
			}
			else if (opType == OperatorType.LeftParen)
			{
				ConsumeToken(OperatorType.LeftParen);
				object result = Level15();
				ConsumeToken(OperatorType.RightParen);
				return result;
			}
			else
				throw new Exception($"Unexpected token: {opType}");
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
					_ => throw new Exception(Log.LogMessage($"运算符 {type} 未在整数实现")),
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
					_ => throw new Exception(Log.LogMessage($"运算符 {type} 未在浮点数实现")),
				};
		}
	}
}