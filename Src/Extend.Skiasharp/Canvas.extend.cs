using SkiaSharp;
using WebGal.Libs.Base;

public static class CanvasExtend
{
	public static void DrawTextBox(this SKCanvas canvas, TextBox textBox)
	{
		textBox.TextRender();
		foreach (var (pos, text) in textBox.GetTextLines())
			canvas.DrawText(text, pos, textBox.TextPaint);
	}
}