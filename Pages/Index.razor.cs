using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SkiaSharp;
using SkiaSharp.Views.Blazor;
using WebGal.Services;

namespace WebGal.Pages;

public partial class Index
{
	[Parameter]
	public string? Game { get; set; }

	[Inject]
	private GameManager Manager { get; set; } = null!;

	protected override void OnParametersSet()
	{
		Game ??= "Yuan Shen";
	}

	protected override void OnAfterRender(bool firstRender)
	{
		if (!firstRender)
			return;
		Manager.DoTest();
	}

	private void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
	{
		SKCanvas canvas = e.Surface.Canvas;
		canvas.DrawBitmap(Manager.GetFrame(DateTimeOffset.Now.Ticks / 10000L), new SKPoint(0, 0));

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

	#region Debug
	private string _text = "", _mousePos = "", _clickPos = "";
	private int _frameCount = 0, _fps = 0, _lastSec;
	#endregion
};