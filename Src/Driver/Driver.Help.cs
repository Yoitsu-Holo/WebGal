using System.Reflection;
using Microsoft.JSInterop;
using WebGal.Global;

namespace WebGal.API;


// 全局驱动程序，也是引擎暴露的接口
public partial class Driver
{
	[JSInvokable]
	public static void Help()
	{
		MethodInfo[] methodInfos = typeof(Test).GetMethods(BindingFlags.Public | BindingFlags.Static);

		foreach (MethodInfo methodInfo in methodInfos)
		{
			string s = "";
			s += $"Method Name: {methodInfo.Name}";
			ParameterInfo[] parameterInfos = methodInfo.GetParameters();
			if (parameterInfos.Length != 0)
				s += $"\n\tParameters:  {string.Join(", ", parameterInfos.Select(p => p.ParameterType.Name + " " + p.Name))}";
			Logger.LogInfo(s, Global.LogLevel.Info);
		}
	}
}