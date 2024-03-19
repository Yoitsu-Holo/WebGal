using System.Text.Json;
using System.Text.Json.Serialization;
using SkiaSharp;

namespace WebGal.Global;

public static class SceneConfig
{
	#region Default Settings
	public const int DefaultWidth = 1280;
	public const int DefaultHeight = 720;
	public const SKColorType DefaultColorType = SKColorType.Rgba8888;
	public const SKAlphaType DefaultAlphaType = SKAlphaType.Unpremul;
	#endregion
}

public static class LayerConfig
{
	#region Default Settings
	public static SKPaint DefaultTextPaint => new()
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
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		PropertyNameCaseInsensitive = true,
		WriteIndented = true,
		Converters =
		{
			new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
		}
	};
	#endregion
}