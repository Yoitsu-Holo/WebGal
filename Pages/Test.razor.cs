using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SkiaSharp;
using SkiaSharp.Views.Blazor;
using WebGal.Services;

namespace WebGal.Pages;

public partial class Test
{
	[Parameter] public string Game { get; set; } = null!;
	[Inject] private GameManager Manager { get; set; } = null!;

	private Dictionary<string, string> _loopAudios = new();
	private Dictionary<string, string> _oneShotAudios = new();

	SKBitmap bitmap = new(100, 100);

	protected override void OnInitialized()
	{
		SKColor[] pixs = bitmap.Pixels;
		for (int row = 0, it = 0; row < 100; row++)
		{
			for (int col = 0; col < 100; col++)
			{
				int dx = 49 - row;
				int dy = 49 - col;
				double delta = Math.Sqrt(dx * dx + dy * dy);
				pixs[it] = new SKColor(0, 180, 255, (byte)Math.Max(0, 255 - delta * 7));
				it++;
			}
		}
		bitmap.Pixels = pixs;
	}

	protected override async Task OnParametersSetAsync()
	{
		await Manager.DoTest(Game);
		Manager.SetMediaList(_loopAudios, _oneShotAudios);
		Manager.LoadMedia();
	}

	private void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
	{
		SKCanvas canvas = e.Surface.Canvas;
		Manager.Render(canvas, DateTimeOffset.Now.Ticks / 10000L);
		// canvas.DrawBitmap(bitmap, new SKPoint(_mousePos.X - 50, _mousePos.Y - 50));

		int sec = DateTimeOffset.UtcNow.Second;
		if (sec != _lastSec)
		{
			_lastSec = sec;
			_fps = _frameCount;
			_frameCount = 0;
			InvokeAsync(StateHasChanged);
		}
		_frameCount++;
	}

	private void OnMouseMove(MouseEventArgs e)
	{
		_mousePos = ((int)e.OffsetX, (int)e.OffsetY);
		_mousePos.X = Math.Max(0, Math.Min(1279, _mousePos.X));
		_mousePos.Y = Math.Max(0, Math.Min(719, _mousePos.Y));
	}

	private void OnClick(MouseEventArgs e)
	{
		// _clickPos = $"{e.OffsetX}, {e.OffsetY}";
		Manager.OnClick(new SKPoint((float)e.OffsetX, (float)e.OffsetY));
	}

	#region Debug
	private string _text = "", _clickPos = "";
	private (int X, int Y) _mousePos;
	private int _frameCount = 0, _fps = 0, _lastSec;
	#endregion
};