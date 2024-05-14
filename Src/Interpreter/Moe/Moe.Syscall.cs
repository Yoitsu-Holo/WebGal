using WebGal.Global;

namespace WebGal.MeoInterpreter;

public partial class MoeInterpreter
{
	public partial class Syscall
	{
		public static void Log(MoeVariable variable)
		{
			Console.WriteLine("Syscall.Log");
			string s = "";
			s += $"\t{variable}";
			if (variable.Type == MoeVariableType.Int || variable.Type == MoeVariableType.Double)
				for (int i = 0; i < variable.Size; i++)
					s += $"{(i % 5 == 0 ? "\n" : "")}\t\tobj[{i}]: {variable[i]}";
			Logger.LogInfo(s, Global.LogLevel.Info);
		}
	}
}
