using SkiaSharp;

namespace WebGal.Extend;

public static class SKBitmapExtend
{
	public static SKBitmap SubBitmap(this SKBitmap source, SKRectI subRect)
	{
		if (subRect.Width == 0 || subRect.Height == 0)
			subRect = new(0, 0, source.Width, source.Height);
		// 创建一个新的 SKBitmap 对象用于存储裁剪后的图片
		var croppedBitmap = new SKBitmap(subRect.Width, subRect.Height);

		// 创建一个 SKCanvas 对象，并将其绑定到 croppedBitmap 上
		using (var canvas = new SKCanvas(croppedBitmap))
		{
			// 绘制裁剪后的图片
			canvas.DrawBitmap(source, subRect, new SKRect(0, 0, subRect.Width, subRect.Height));
		}
		// 返回裁剪后的 SKBitmap 图片
		return croppedBitmap;
	}
}