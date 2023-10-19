using SkiaSharp;
using WebGal.Global;

namespace WebGal.Libs.Base;

public class LayerText
{
	public string Text { get; set; } = "";
	public IVector Pos { get; set; }
	// 默认黑色
	public SKPaint Paint { get; set; } = LayerConfig.DefaultTextPaint;
}