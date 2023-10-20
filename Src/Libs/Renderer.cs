using SkiaSharp;
using WebGal.Libs.Base;

namespace WebGal.Libs;
public class Renderer
{
	private SKCanvas _canvas = null!;

	public void Clear()
	{
		_canvas?.Clear();
	}

	public void Render(SKCanvas canvas, Scene scene, long timeoff, bool force)
	{
		if (_canvas != canvas)
		{
			Clear();
			_canvas = canvas;
		}

		if (scene.HasAnimation(timeoff) || force)
		{
			_canvas.Clear();
			foreach (var (layerId, layer) in scene.Layers)
			{
				layer.GenNextFrame(timeoff);
				if (layer.DynamicAttribute.IsHide)
					goto after;

				if (layer.BackGroundSKBitmap is not null)
					_canvas.DrawBitmap(layer.FrameBuffer, layer.PosAt(timeoff));

				// if ( is List<LayerText> texts)
				foreach (var text in layer.Text)
					_canvas.DrawText(text.Text, layer.AbsolutePos(text.Pos), text.Paint);
				after:

				if (layer.DynamicAttribute != layer.OriginalAttribute)
				{
					layer.DynamicAttribute = layer.OriginalAttribute;
					scene.StateHasChange = true;
				}
			}
			_canvas.Flush();
		}
	}
}
