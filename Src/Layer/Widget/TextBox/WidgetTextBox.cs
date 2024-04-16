using SkiaSharp;
using WebGal.Global;
using WebGal.Libs.Base;
using WebGal.Types;

namespace WebGal.Layer.Widget;

public class WidgetTextBox : LayerBase
{
	public SKPaint TextPaint { get; set; } = RenderConfig.DefaultTextPaint;
	public TextBoxStyle Style { get; set; } = new();
	private readonly List<string> _textLine = [];

	public virtual void SetFontSize(int fontSize)
	{
		TextPaint.TextSize = fontSize;
		_dirty = true;
	}

	public override void SetColor(SKColor color, int imageId = 0)
	{
		TextPaint.Color = color;
		_dirty = true;
	}

	public virtual void SetFontStyle(SKTypeface typeFace)
	{
		TextPaint.Typeface = typeFace;
		_dirty = true;
	}

	public override void Render(SKCanvas canvas, bool force)
	{
		if (Status == LayerStatus.Unvisable)
			return;

		if (_dirty)
		{
			_textLine.Clear();
			int lineWidth = Size.X - (Style.Padding.Left + Style.Padding.Right);

			string s = "";
			foreach (var c in Text)
			{
				float length = TextPaint.MeasureText(s + c);
				if (length > lineWidth || c == '\n')
				{
					_textLine.Add(s);
					s = "";
				}
				if (c != '\n')
					s += c;
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
		}
	}

	public override void ExecuteAction(EventArgs eventArgs) { }
}