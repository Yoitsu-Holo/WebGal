using SkiaSharp;
using WebGal.Global;

namespace WebGal.Libs.Base;

public class LayerText
{
	public string Text { get; set; } = "NULL";
	public SKPoint Pos { get; set; } = new SKPoint(0, 0);
	// 默认黑色
	public SKPaint Paint { get; set; } = LayerConfig.DefaultTextPaint;
}