using System.Runtime.CompilerServices;
using WebGal.Extend.Collections;
using WebGal.Global;

namespace WebGal.MeoInterpreter;

public partial class MoeInterpreter
{
	public class ExpressionsExecutor
	{
		/// <summary> 返回类型为 int / float / string 的结果 </summary>
		public static object Parse(ExpressionNode expression) => Parse(expression.Tokens);

		/// <summary> 返回类型为 int / float / string 的结果 </summary>
		public static object Parse(List<ExpressionToken> expression)
		{
			DoubleEndEnumerator<ExpressionToken> tokens = new(expression);
			tokens.MoveNext();
			object result = Level15(tokens);
			if (tokens.IsEnd == false)
				throw new Exception(Logger.LogMessage($"Unexpected token {tokens}"));
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static object Level14(IExtendEnumerator<ExpressionToken> tokens)
		{
			return Level13(tokens);
		}


		/// <summary>
		///;	?:		三元条件
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static object Level13(IExtendEnumerator<ExpressionToken> tokens)
		{
			return Level12(tokens);
		}

		/// <summary>
		///;	||		逻辑或
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static object Level1(IExtendEnumerator<ExpressionToken> tokens)
		{
			OperatorType opType = tokens.IsEnd ? new() : tokens.Current.Type;
			if (opType == OperatorType.Number)
			{
				return ConsumeToken(OperatorType.Number, tokens).Number;
			}
			else if (opType == OperatorType.String)
			{
				return ConsumeToken(OperatorType.String, tokens).String;
			}
			else if (opType == OperatorType.Variable)
			{
				VariableInfo variableInfo = tokens.Current.Var;
				ConsumeToken(OperatorType.Variable, tokens);

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
			// 			else if (opType == OperatorType.String)
			// {

			// }
			else if (opType == OperatorType.LeftParen)
			{
				ConsumeToken(OperatorType.LeftParen, tokens);
				object result = Level15(tokens);
				ConsumeToken(OperatorType.RightParen, tokens);
				return result;
			}
			else
				throw new Exception($"Unexpected token: {tokens.Current}");
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static object Calc(object v1, object v2, OperatorType type)
		{
			int v1i, v2i;
			float v1f, v2f;
			string v1s, v2s;
			int flag = 0;

			{
				if (v1 is int vv1)
					(v1i, v1f, v1s) = (vv1, vv1, vv1.ToString());
				else if (v1 is float vv2)
					(v1i, v1f, v1s, flag) = ((int)vv2, vv2, vv2.ToString(), 1);
				else if (v1 is string vv3)
					(v1i, v1f, v1s, flag) = (0, 0, vv3, 2);
				else throw new Exception(Logger.LogMessage($"非 int/float/string 类型 {v1.GetType()}"));
			}
			{
				if (v2 is int vv1 && flag != 2)
					(v2i, v2f, v2s) = (vv1, vv1, vv1.ToString());
				else if (v2 is float vv2 && flag != 2)
					(v2i, v2f, v2s, flag) = ((int)vv2, vv2, vv2.ToString(), 1);
				else if (v2 is string vv3)
					(v2i, v2f, v2s, flag) = (0, 0, vv3, 2);
				else throw new Exception(Logger.LogMessage($"非 int/float/string 类型 {v2.GetType()}"));
			}

			if (flag == 0)
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
			else if (flag == 1)
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
			else if (flag == 2)
			{
				return type switch
				{
					OperatorType.ADD => v1s + v2s,
					OperatorType.EQ => v1s == v2s ? 1 : 0,
					OperatorType.NEQ => v1s != v2s ? 1 : 0,
					_ => throw new Exception(Logger.LogMessage($"运算符 {type} 未在浮点数实现")),
				};
			}
			else
				throw new Exception("");
		}
	}
}