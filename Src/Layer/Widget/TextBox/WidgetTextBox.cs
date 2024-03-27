using SkiaSharp;
using WebGal.Global;
using WebGal.Libs.Base;
using WebGal.Types;

namespace WebGal.Layer.Widget;

public class WidgetTextBox : LayerBase
{
	public SKPaint TextPaint { get; set; } = LayerConfig.DefaultTextPaint;
	public TextBoxStyle BoxStyle { get; set; } = new();
	private readonly List<string> _textLine = [];

	public override void SetColor(SKColor color, IVector size = default, int imageId = 0)
	{
		TextPaint.Color = color;
		base.SetColor(color, size, imageId);
	}

	public override void Render(SKCanvas canvas)
	{
		if (Status == LayerStatus.Unvisable)
			return;

		if (_dirty)
		{
			_textLine.Clear();
			if (BoxStyle.BoxSize.Y == 0)
			{
				_textLine.Add(Text);
				return;
			}
			else
			{
				int lineWidth = BoxStyle.BoxSize.Width - (BoxStyle.Padding.Left + BoxStyle.Padding.Right);

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
			}
			_dirty = false;
		}


		IVector startPos = BoxStyle.BoxPos + new IVector(BoxStyle.Padding.Top, BoxStyle.Padding.Left);
		for (int i = 0; i < _textLine.Count; i++)
		{
			startPos.Y += BoxStyle.MarginTop;
			startPos.Y += (int)TextPaint.TextSize;
			canvas.DrawText(_textLine[i], startPos, TextPaint);
			startPos.Y += BoxStyle.MarginBottom;
		}
	}
}