using SkiaSharp;

public static class SKBitmapExtend
{
	public static SKBitmap CropBitmap(this SKBitmap source, SKRectI cropRect)
	{
		// 创建一个新的 SKBitmap 对象用于存储裁剪后的图片
		var croppedBitmap = new SKBitmap(cropRect.Width, cropRect.Height);

		// 创建一个 SKCanvas 对象，并将其绑定到 croppedBitmap 上
		using (var canvas = new SKCanvas(croppedBitmap))
		{
			// 绘制裁剪后的图片
			canvas.DrawBitmap(source, cropRect, new SKRectI(0, 0, cropRect.Width, cropRect.Height));
		}

		// 返回裁剪后的 SKBitmap 图片
		return croppedBitmap;
	}
}