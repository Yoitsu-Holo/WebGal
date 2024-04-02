namespace WebGal.Libs.Base;
using WebGal.Types;

public record struct TextPadding(
	int Top,
	int Bottom,
	int Left,
	int Right
);

public class TextBoxStyle
{
	public int MarginTop { get; set; } // 行高，顶部，像素
	public int MarginBottom { get; set; } // 行高，底部，像素
	public TextPadding Padding { get; set; } // 文本框内边距，像素
}