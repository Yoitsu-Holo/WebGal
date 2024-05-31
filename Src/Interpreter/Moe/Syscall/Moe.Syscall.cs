using WebGal.Global;

namespace WebGal.MeoInterpreter;

public partial class MoeInterpreter
{
	public partial class Syscall
	{
		/// <summary>
		/// 打印一个 MoeVariable 变量
		/// </summary>
		public static void Log(MoeVariable variable)
		{
			string s = "";
			s += $"\t{variable}";
			if (variable.Type == MoeVariableType.Int || variable.Type == MoeVariableType.Float)
				for (int i = 0; i < variable.Size; i++)
					s += $"{(i % 5 == 0 ? "\n" : "")}\t\tobj[{i}]: {variable[i]}";
			else if (variable.Type == MoeVariableType.String)
				for (int i = 0; i < variable.Size; i++)
					s += $"{(i % 5 == 0 ? "\n" : "")}\t\tobj[{i}]: {variable[i]}";
			Logger.LogInfo(s, Global.LogLevel.Info);
		}

		/// <summary>
		/// 加载一个剧本
		/// </summary>
		/// <param name="opera"> string: 剧本名称 </param>
		public static void LoadOpera(MoeVariable operaID)
		{
		}

		/// <summary>
		/// 加载当前剧本的场景
		/// </summary>
		/// <param name="sceneID"> int: 场景ID </param>
		public static void LoadScene(MoeVariable sceneID)
		{
		}



	}
}
