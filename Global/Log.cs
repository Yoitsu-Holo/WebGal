using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WebGal.MeoInterpreter;

namespace WebGal.Global;

public enum LogLevel
{
	Info,
	Todo,
	Warning,
	Error,
};

public class Log
{
#pragma warning disable CA2211 // 非常量字段应当不可见
	public static IJSRuntime? JsRuntime;
#pragma warning restore CA2211 // 非常量字段应当不可见

	public static string LogMessage(object obj,
		[CallerFilePath] string filePath = "",
		[CallerMemberName] string memberName = "",
		[CallerLineNumber] int lineNumber = 0)
	{
		string ret = $"In File: {filePath.Split('\\')[^1]}\tName: {memberName}\tLine: {lineNumber}\n";
		ret += $"{MsgGen(obj)}";
		return ret;
	}

	public static async void LogInfo(object obj, LogLevel logLevel = LogLevel.Info,
		[CallerFilePath] string filePath = "",
		[CallerMemberName] string memberName = "",
		[CallerLineNumber] int lineNumber = 0)
	{
		string msg = $"In File: {filePath.Split('\\')[^1]}\tName: {memberName}\tLine: {lineNumber}\n";
		msg += $"{MsgGen(obj)}";

		if (JsRuntime == null)
			throw new Exception("JS Runtime 未被设置");

		if (logLevel == LogLevel.Info)
			await JsRuntime.InvokeVoidAsync("consoleLogger.logInfo", msg);
		else if (logLevel == LogLevel.Todo)
			await JsRuntime.InvokeVoidAsync("consoleLogger.logTodo", msg);
		else if (logLevel == LogLevel.Warning)
			await JsRuntime.InvokeVoidAsync("consoleLogger.logWarning", msg);
		else if (logLevel == LogLevel.Error)
			await JsRuntime.InvokeVoidAsync("consoleLogger.logError", msg);
		else
			await JsRuntime.InvokeVoidAsync("consoleLogger.logError", msg);
	}

	private static string MsgGen(object obj)
	{
		string s = "";
		s += $"\t{obj}";
		if (obj is MoeVariable v)
		{
			s += "\n";
			if (v.Type == MoeVariableType.Int || v.Type == MoeVariableType.Double)
				for (int i = 0; i < v.Size; i++)
					s += $"\t\tobj[{i}]: {v[i]}{(i % 5 == 4 ? "\n" : "")}";
		}
		return s;
	}
}