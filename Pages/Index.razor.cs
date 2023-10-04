using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SkiaSharp;
using SkiaSharp.Views.Blazor;
using WebGal.Services;

namespace WebGal.Pages;

public partial class Index
{
	private int _nowDateMillisecond { get => (DateTimeOffset.Now.Minute * 60 + DateTimeOffset.Now.Second) * 1000 + DateTimeOffset.Now.Millisecond; }
	[Parameter]
	public string Game { get; set; } = "Yuan Shen";

	[Inject]
	private GameManager Manager { get; set; } = null!;

	protected override void OnAfterRender(bool firstRender)
	{
		if (!firstRender)
			return;
		Manager.DoTest(_nowDateMillisecond);
	}

	private void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
	{
		SKCanvas canvas = e.Surface.Canvas;
		canvas.DrawBitmap(Manager.GetFrame(_nowDateMillisecond, true), new SKPoint(0, 0));

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
		_mousePos = $"{e.OffsetX}, {e.OffsetY}";
	}

	private void OnClick(MouseEventArgs e)
	{
		_clickPos = $"{e.OffsetX}, {e.OffsetY}";
	}

	private CancellationTokenSource _cts = new();

	#region Debug
	private string _text = "", _mousePos = "", _clickPos = "";
	private int _frameCount = 0, _fps = 0, _lastSec;
	#endregion
};