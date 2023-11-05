using SkiaSharp;

public static class CanvasExtend
{
	private static SKTypeface? _typeface;

	public static void SetTypeface(this SKCanvas canvas, SKTypeface typeface)
	{
		_typeface = typeface;
	}

	public static void DrawTextBox(this SKCanvas canvas, string text, SKRect rect, SKPaint paint)
	{
		canvas.DrawText(text, new SKPoint(rect.Top, rect.Left), paint);

		// var rs = new RichString()
		// 	.Alignment(TextAlignment.Center)
		// 	.FontFamily("Microsoft Yahei UI")
		// 	.MarginBottom(20)
		// 	.Add("Welcome To RichTextKit 正在确定要还原的项目…", fontSize: 24, fontWeight: 700, fontItalic: true)
		// 	.Paragraph().Alignment(TextAlignment.Left)
		// 	.FontSize(18)
		// 	.Add("This is a test string");
		// rs.MaxWidth = 100;
		// rs.MaxHeight = 1000;
		// rs.Paint(e.Surface.Canvas, new SKPoint(50, 50));
	}

	public static void DrawTextBox(this SKCanvas canvas)
	{
		SKRect rect = new();
		using SKPaint testPaint = new()
		{
			Color = SKColors.Bisque,
			IsAntialias = true,
			TextSize = 64,
		};

		string testText = "Hello, World 正在确定要还原的项目…";
		if (_typeface is not null)
			testPaint.Typeface = _typeface;

		testPaint.MeasureText(testText, ref rect);
		Console.WriteLine($"Width={rect.Width}, Height={rect.Height}");

		SKPoint pt = new(20, 100);
		canvas.DrawText(testText, pt, testPaint);

		rect.Offset(pt);
		testPaint.IsStroke = true;
		testPaint.Color = SKColors.Magenta;
		canvas.DrawRect(rect, testPaint);
	}
}