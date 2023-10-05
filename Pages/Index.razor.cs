using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using SkiaSharp;
using SkiaSharp.Views.Blazor;
using WebGal.Services;

namespace WebGal.Pages;

public partial class Index
{
	[Parameter]
	public string? Game { get; set; }

	[Inject]
	private IJSRuntime JS { get; set; }

	[Inject]
	private GameManager Manager { get; set; } = null!;

	private Dictionary<string, string> _loopAudios = new();
	private Dictionary<string, string> _oneShotAudios = new();
	private string Audio = "/Data/Test/pack/sound/bgm/bgm02_b.ogg";

	protected override async Task OnInitializedAsync()
	{
		await Manager.DoTest();
		Audio = "/Data/Test/pack/sound/bgm/bgm02_b.ogg";
		_loopAudios.Add("bgm", Audio);
	}

	protected override void OnAfterRender(bool firstRender)
	{
		if (!firstRender)
			return;
	}

	private void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
	{
		SKCanvas canvas = e.Surface.Canvas;
		Manager.GetFrame(canvas, DateTimeOffset.Now.Ticks / 10000L);
		// e.Surface.Draw(canvas, 0, 0, LayerConfig.DefualtTextPaint);
		// canvas.DrawBitmap(Manager.GetFrame(DateTimeOffset.Now.Ticks / 10000L), new SKPoint(0, 0));

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