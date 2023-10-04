
using SkiaSharp;

namespace WebGal.Global;

public static class SceneConfig
{
	#region Defualt Settings
	public const int DefualtWidth = 1280;
	public const int DefualtHeight = 720;
	public const SKColorType DefualtColorType = SKColorType.Rgba8888;
	public const SKAlphaType DefualtAlphaType = SKAlphaType.Unpremul;
	#endregion
}

public static class LayerConfig
{
	#region Defualt Settings
	public readonly static SKPaint DefualtTextPaint = new()
	{
		Color = SKColors.Black,
		TextSize = 30,
		FakeBoldText = true,
		IsAntialias = true
	};
	public const SKColorType DefualtColorType = SKColorType.Rgba8888;
	public const SKAlphaType DefualtAlphaType = SKAlphaType.Unpremul;

	#endregion
}
