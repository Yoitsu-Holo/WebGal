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

public class Logger
{
#pragma warning disable CA2211 // 非常量字段应当不可见
	public static IJSRuntime? JsRuntime;
#pragma warning restore CA2211 // 非常量字段应当不可见

	public static string LogMessage(string log,
		[CallerFilePath] string filePath = "",
		[CallerMemberName] string memberName = "",
		[CallerLineNumber] int lineNumber = 0)
	{
		string msg = $"In File: {filePath.Split('\\')[^1]}\tName: {memberName}\tLine: {lineNumber}\n";
		msg += $"{log}";
		return msg;
	}

	public static async void LogInfo(string log, LogLevel logLevel = LogLevel.Info,
		[CallerFilePath] string filePath = "",
		[CallerMemberName] string memberName = "",
		[CallerLineNumber] int lineNumber = 0)
	{
		string msg = "";
		if (logLevel != LogLevel.Info)
			msg += $"In File: {filePath.Split('\\')[^1]}\tName: {memberName}\tLine: {lineNumber}\n";
		msg += $"{log}";

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
}