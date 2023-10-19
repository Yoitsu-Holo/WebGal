using System.Runtime.Versioning;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SkiaSharp;
using SkiaSharp.Views.Blazor;
using WebGal.Event;
using WebGal.Global;
using WebGal.Libs.Base;
using WebGal.Services;

namespace WebGal.Pages;

public partial class Test
{
	[Parameter] public string Game { get; set; } = null!;
	[Inject] private GameManager Manager { get; set; } = null!;

	private Dictionary<string, string> _loopAudios = new();
	private Dictionary<string, string> _oneShotAudios = new();

	private SKPointI _mousePos;

	private MouseEvent _mouseEvent = new();

	protected override void OnInitialized()
	{
	}

	protected override async Task OnParametersSetAsync()
	{
		//todo 未解决循环卡顿问题
		_loopAudios.Clear();
		_oneShotAudios.Clear();
		Manager.SetMediaList(_loopAudios, _oneShotAudios);
		Manager.Clear();
		await Manager.DoTest(Game);
		await Manager.LoadMedia();
	}

	private async void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
	{
		Console.WriteLine($"{_mouseEvent.Button}:{_mouseEvent.Status}");
		SKCanvas canvas = e.Surface.Canvas;
		Manager.OnMouceMoveOn(_mousePos);
		Manager.Render(canvas, NowTime.Minisecond);

		int sec = DateTimeOffset.UtcNow.Second;
		if (sec != _lastSec)
		{
			_lastSec = sec;
			_fps = _frameCount;
			_frameCount = 0;
			await InvokeAsync(StateHasChanged);
		}
		_frameCount++;
		OnMouseSpace();
	}

	private void OnMouseMove(MouseEventArgs e)
	{
		// _mousePos.X = Math.Max(0, Math.Min(1279, _mousePos.X));
		// _mousePos.Y = Math.Max(0, Math.Min(719, _mousePos.Y));
		_mousePos = new SKPointI((int)e.OffsetX, (int)e.OffsetY);
	}

	private void OnMouseUp(MouseEventArgs e)
	{
		_mouseEvent.Status = MouseStatus.ButtonUp;

		if (_mouseEvent.Button == MouseButton.Space || _mouseEvent.Button == MouseButton.MouseChord)
			return;

		_mouseEvent.Button = e.Button switch
		{
			0L => (_mouseEvent.Button == MouseButton.RButton) ? MouseButton.MouseChord : MouseButton.LButton,
			1L => MouseButton.MButton,
			2L => (_mouseEvent.Button == MouseButton.LButton) ? MouseButton.MouseChord : MouseButton.RButton,
			_ => MouseButton.Space,
		};
	}

	private void OnMouseDown(MouseEventArgs e)
	{
		_mouseEvent.Status = MouseStatus.ButtonDown;
		Console.WriteLine(e.Button);

		_mouseEvent.Button = e.Button switch
		{
			0L => (_mouseEvent.Button == MouseButton.RButton) ? MouseButton.MouseChord : MouseButton.LButton,
			1L => MouseButton.MButton,
			2L => (_mouseEvent.Button == MouseButton.LButton) ? MouseButton.MouseChord : MouseButton.RButton,
			_ => MouseButton.Space,
		};
	}

	private void OnMouseSpace()
	{
		if (_mouseEvent.Status == MouseStatus.ButtonUp)
			_mouseEvent.Button = MouseButton.Space;
	}

	#region Debug
	private int _frameCount = 0, _fps = 0, _lastSec;
	#endregion
};