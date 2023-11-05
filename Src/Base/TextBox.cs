using System.Security.AccessControl;
using SkiaSharp;
using WebGal.Global;

namespace WebGal.Libs.Base;

public class TextBox
{
	public string Text { get; set; } = "";
	public IVector BoxPos { get; set; }
	public IVector BoxSize { get; set; } // 文本窗口大小，若为 (0,0) 则直接显示为一行

	public SKPaint TextPaint { get; set; } = LayerConfig.DefaultTextPaint;
	public SKTypeface Typeface { get; set; } = SKTypeface.Default;

	public int MarginTop { get; set; } // 行高，顶部，像素
	public int MarginBottom { get; set; } // 行高，底部，像素
	public TextPadding Padding { get; set; } // 文本框内边距，像素

	private List<string> _textLine = new();

	public void TextRender()
	{
		if (BoxSize.Y == 0)
		{
			_textLine.Add(Text);
			return;
		}

		int lineWidth = BoxSize.Width - (Padding.Left + Padding.Right);
		TextPaint.Typeface = Typeface;

		string s = "";
		foreach (var c in Text)
		{
			s += c;
			float length = TextPaint.MeasureText(s);
			if (length > lineWidth)
			{
				if (s.Length > 1)
					_textLine.Add(s[..^1]);
				else
					_textLine.Add(s);

				if (s[^1..] != " ")
					s = s[^1..];
				else
					s = "";
			}
		}

		if (s.Length != 0)
			_textLine.Add(s);

		foreach (var str in _textLine)
		{
			Console.WriteLine(str);
		}
	}

	public IEnumerable<(IVector, string)> GetTextLines()
	{
		IVector startPos = BoxPos + new IVector(Padding.Top, Padding.Left);
		for (int i = 0; i < _textLine.Count; i++)
		{
			startPos.Y += (int)TextPaint.TextSize;
			string str = _textLine[i];
			yield return (startPos, str);
		}
	}
}

public record struct TextPadding(
	int Top,
	int Bottom,
	int Left,
	int Right
);