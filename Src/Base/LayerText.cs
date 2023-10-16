using SkiaSharp;
using WebGal.Global;

namespace WebGal.Libs.Base;

public class LayerText
{
	public string Text { get; set; } = "";
	public SKPointI Pos { get; set; } = new SKPointI(0, 0);
	// 默认黑色
	public SKPaint Paint { get; set; } = LayerConfig.DefaultTextPaint;
}