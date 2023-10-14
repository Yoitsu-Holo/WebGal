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

	protected override void OnInitialized()
	{
	}

	protected override async Task OnParametersSetAsync()
	{
		//todo 未解决循环卡顿问题
		_loopAudios.Clear();
		_oneShotAudios.Clear();
		Manager.SetMediaList(_loopAudios, _oneShotAudios);
		await Manager.DoTest(Game);
		await Manager.LoadMedia();
	}

	private async void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
	{
		SKCanvas canvas = e.Surface.Canvas;
		Manager.Render(canvas, DateTimeOffset.Now.Ticks / 10000L);

		int sec = DateTimeOffset.UtcNow.Second;
		if (sec != _lastSec)
		{
			_lastSec = sec;
			_fps = _frameCount;
			_frameCount = 0;
			await InvokeAsync(StateHasChanged);
		}
		_frameCount++;
	}

	private void OnMouseMove(MouseEventArgs e)
	{
		_mousePos = ((int)e.OffsetX, (int)e.OffsetY);
		_mousePos.X = Math.Max(0, Math.Min(1279, _mousePos.X));
		_mousePos.Y = Math.Max(0, Math.Min(719, _mousePos.Y));
	}

	private async Task OnClick(MouseEventArgs e)
	{
		Console.WriteLine(e.Button);
		if (e.Button == 0L)
			await OnLeftClick(e);
		if (e.Button == 2L)
			await OnRightClick(e);
		await InvokeAsync(StateHasChanged);
	}

	private async Task OnLeftClick(MouseEventArgs e)
	{
		await Manager.OnClickAsync(new SKPointI((int)e.OffsetX, (int)e.OffsetY));
	}

	private async Task OnRightClick(MouseEventArgs e)
	{
		Console.WriteLine("Right Click");
		// await Manager.OnClickAsync(new SKPoint((float)e.OffsetX, (float)e.OffsetY));
	}

	#region Debug
	private (int X, int Y) _mousePos;
	private int _frameCount = 0, _fps = 0, _lastSec;
	#endregion
};