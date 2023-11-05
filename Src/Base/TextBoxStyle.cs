namespace WebGal.Libs.Base;

public class TextBoxStyle
{
	public IVector BoxPos { get; set; }
	public IVector BoxSize { get; set; } // 文本窗口大小，若为 (0,0) 则直接显示为一行
	public int MarginTop { get; set; } // 行高，顶部，像素
	public int MarginBottom { get; set; } // 行高，底部，像素
	public TextPadding Padding { get; set; } // 文本框内边距，像素
}