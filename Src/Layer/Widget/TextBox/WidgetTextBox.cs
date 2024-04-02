using SkiaSharp;
using WebGal.Global;
using WebGal.Libs.Base;
using WebGal.Types;

namespace WebGal.Layer.Widget;

public class WidgetTextBox : LayerBase
{
	public SKPaint TextPaint { get; set; } = LayerConfig.DefaultTextPaint;
	public TextBoxStyle Style { get; set; } = new();
	private readonly List<string> _textLine = [];

	public virtual void SetFontSize(int fontSize)
	{
		TextPaint.TextSize = fontSize;
	}

	public override void SetColor(SKColor color, IVector size = default, int imageId = 0)
	{
		TextPaint.Color = color;
	}

	public virtual void SetFontStyle(SKTypeface typeFace)
	{
		TextPaint.Typeface = typeFace;
	}

	public override void Render(SKCanvas canvas, bool force)
	{
		if (Status == LayerStatus.Unvisable)
			return;

		if (_dirty)
		{
			_textLine.Clear();
			int lineWidth = Size.X - (Style.Padding.Left + Style.Padding.Right);
			Console.WriteLine($"{Size.X}:{Size.Y} ");
			string s = "";
			foreach (var c in Text)
			{
				s += c;
				float length = TextPaint.MeasureText(s);
				if (length > lineWidth)
				{
					_textLine.Add((s.Length > 1) ? s[..^1] : s);
					s = (s[^1..] != " ") ? s[^1..] : "";
				}
			}

			if (s.Length != 0)
				_textLine.Add(s);
			_dirty = false;
		}

		IVector startPos = Position + new IVector(Style.Padding.Top, Style.Padding.Left);
		for (int i = 0; i < _textLine.Count; i++)
		{
			startPos.Y += Style.MarginTop;
			startPos.Y += (int)TextPaint.TextSize;
			canvas.DrawText(_textLine[i], startPos, TextPaint);
			startPos.Y += Style.MarginBottom;

			// Console.WriteLine(startPos.X + ": " + startPos.Y);
		}
	}

	public override void ExecuteAction(EventArgs eventArgs) { }
}