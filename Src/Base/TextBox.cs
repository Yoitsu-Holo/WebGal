using WebGal.Types;
using SkiaSharp;
using WebGal.Global;

namespace WebGal.Libs.Base;

public class TextBox
{
	public string Text { get; set; } = "";
	public SKPaint TextPaint { get; set; } = LayerConfig.DefaultTextPaint;
	public TextBoxStyle BoxStyle { get; set; } = new();

	private List<string> _textLine = new();

	public void TextRender()
	{
		if (BoxStyle.BoxSize.Y == 0)
		{
			_textLine.Add(Text);
			return;
		}

		int lineWidth = BoxStyle.BoxSize.Width - (BoxStyle.Padding.Left + BoxStyle.Padding.Right);

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

		// foreach (var str in _textLine)
		// {
		// 	Console.WriteLine(str);
		// }
	}

	public IEnumerable<(IVector, string)> GetTextLines()
	{
		IVector startPos = BoxStyle.BoxPos + new IVector(BoxStyle.Padding.Top, BoxStyle.Padding.Left);
		for (int i = 0; i < _textLine.Count; i++)
		{
			startPos.Y += BoxStyle.MarginTop;
			startPos.Y += (int)TextPaint.TextSize;
			string str = _textLine[i];
			yield return (startPos, str);
			startPos.Y += BoxStyle.MarginBottom;
		}
	}
}

public record struct TextPadding(
	int Top,
	int Bottom,
	int Left,
	int Right
);