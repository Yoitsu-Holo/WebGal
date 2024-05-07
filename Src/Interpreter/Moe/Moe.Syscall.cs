using WebGal.Global;

namespace WebGal.MeoInterpreter;

public partial class MoeInterpreter
{
	public partial class Syscall
	{
		public static void Log(List<MoeVariable> vars)
		{
			foreach (var v in vars)
			{
				string s = "";
				s += $"\t{vars}";
				if (v.Type == MoeVariableType.Int || v.Type == MoeVariableType.Double)
					for (int i = 0; i < v.Size; i++)
						s += $"{(i % 5 == 0 ? "\n" : "")}\t\tobj[{i}]: {v[i]}";

				Logger.LogInfo(s, Global.LogLevel.Info);
			}
		}
	}
}
