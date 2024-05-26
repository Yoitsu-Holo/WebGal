using System.Text.Json;
using System.Text.Json.Serialization;
using SkiaSharp;

namespace WebGal.Global;

public enum CallMode
{
	Direct,
	API,
}

/// <summary>
/// 全局驱动调用方式：
/// Direct：直接通过C#调用; API：通过暴露的javascript接口调用.
/// </summary>
public static class DriverCallMode
{
	public const CallMode Mode = CallMode.Direct;
}

public static class RenserConfig
{
#pragma warning disable CA2211 // 非常量字段应当不可见
	public static bool ForceRender = false;
#pragma warning restore CA2211 // 非常量字段应当不可见
}

public static class SceneConfig
{
	#region Default Settings
	public const int DefaultWidth = 1280;
	public const int DefaultHeight = 720;
	public const SKColorType DefaultColorType = SKColorType.Rgba8888;
	public const SKAlphaType DefaultAlphaType = SKAlphaType.Unpremul;
	#endregion
}

public static class RenderConfig
{
	#region Default Settings
	public static SKPaint DefaultTextPaint => new()
	{
		Color = SKColors.Black,
		TextSize = 30,
		FakeBoldText = true,
		IsAntialias = true
	};

	public static SKPaint DefaultPaint => new()
	{
		Color = SKColors.Black,
		TextSize = 30,
		FakeBoldText = true,
		IsAntialias = true
	};
	public const SKColorType DefaultColorType = SKColorType.Rgba8888;
	public const SKAlphaType DefaultAlphaType = SKAlphaType.Unpremul;
	#endregion
}

public static class JsonConfig
{
	#region Default Settings
	public static readonly JsonSerializerOptions Options = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // 驼峰命名
		PropertyNameCaseInsensitive = true, // 不区分大小写
											// WriteIndented = true, // 缩进
		Converters =
		{
			new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
		}
	};
	#endregion
}