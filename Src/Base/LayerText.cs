using SkiaSharp;
using WebGal.Global;
using WebGal.Types;

namespace WebGal.Libs.Base;

public class LayerText
{
	public string Text { get; set; } = "";
	public IVector Pos { get; set; }
	public SKPaint Paint { get; set; } = LayerConfig.DefaultTextPaint;
}