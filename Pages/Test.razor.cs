using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SkiaSharp;
using SkiaSharp.Views.Blazor;
using WebGal.Event;
using WebGal.Global;
using WebGal.Services;

namespace WebGal.Pages;

public partial class Test
{
	[Parameter] public string Game { get; set; } = null!;
	[Inject] private GameManager Manager { get; set; } = null!;

	private Dictionary<string, string> _loopAudios = new();
	private Dictionary<string, string> _oneShotAudios = new();

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
		// SKCanvas canvas = e.Surface.Canvas;
		if (_mouseEvent.Status == MouseStatus.Up || _mouseEvent.Status == MouseStatus.Down)
			Console.WriteLine($"{_mouseEvent.Button} -> {_mouseEvent.Status}");

		var mouseEventCopy = _mouseEvent;
		OnMouseSpace();

		await Manager.ProcessMouseEvent(mouseEventCopy);

		Manager.Render(e.Surface.Canvas, NowTime.Minisecond);

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
		_mouseEvent.Position = new SKPointI((int)e.OffsetX, (int)e.OffsetY);
	}

	private void OnMouseUp(MouseEventArgs e)
	{
		_mouseEvent.Status = MouseStatus.Up;

		if (_mouseEvent.Button == MouseButton.Null || _mouseEvent.Button == MouseButton.MouseChord)
			return;

		_mouseEvent.Button = e.Button switch
		{
			0L => (_mouseEvent.Button == MouseButton.RButton) ? MouseButton.MouseChord : MouseButton.LButton,
			1L => MouseButton.MButton,
			2L => (_mouseEvent.Button == MouseButton.LButton) ? MouseButton.MouseChord : MouseButton.RButton,
			_ => MouseButton.Null,
		};
	}

	private void OnMouseDown(MouseEventArgs e)
	{
		_mouseEvent.Status = MouseStatus.Down;
		Console.WriteLine(e.Button);

		_mouseEvent.Button = e.Button switch
		{
			0L => (_mouseEvent.Button == MouseButton.RButton) ? MouseButton.MouseChord : MouseButton.LButton,
			1L => MouseButton.MButton,
			2L => (_mouseEvent.Button == MouseButton.LButton) ? MouseButton.MouseChord : MouseButton.RButton,
			_ => MouseButton.Null,
		};
	}

	private void OnMouseSpace()
	{
		if (_mouseEvent.Status == MouseStatus.Up)
		{
			_mouseEvent.Status = MouseStatus.Release;
			_mouseEvent.Button = MouseButton.Null;
		}
		if (_mouseEvent.Status == MouseStatus.Down)
			_mouseEvent.Status = MouseStatus.Hold;
	}

	#region Debug
	private int _frameCount = 0, _fps = 0, _lastSec;
	#endregion
};