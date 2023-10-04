using System.Net;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SkiaSharp;
using SkiaSharp.Views.Blazor;
using WebGal.Services;
using WebGal.Services.Debug;

namespace WebGal.Pages;

public partial class Index
{
	[Parameter]
	public string Game { get; set; } = "Yuan Shen";

	private readonly GameManager _game = new();

	protected override void OnAfterRender(bool firstRender)
	{
		if (!firstRender)
			return;
	}

	private void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
	{
		SKCanvas canvas = e.Surface.Canvas;
		canvas.DrawBitmap(_game.GetFrame(DateTimeOffset.UtcNow.Millisecond, true), new SKPoint(0, 0));

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
		// Buffer.ChangedFlag = true;
	}

	#region Debug
	private string _text = "", _mousePos = "", _clickPos = "";
	private int _frameCount = 0, _fps = 0, _lastSec = 0;
	#endregion
};