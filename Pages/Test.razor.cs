using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using SkiaSharp.Views.Blazor;
using WebGal.API.Data;
using WebGal.Global;
using WebGal.Handler.Event;
using WebGal.Services;
using WebGal.Types;

namespace WebGal.Pages;

public partial class Test// : IDisposable
{
	[Parameter] public string Game { get; set; } = null!;
	[Inject] private GameManager Manager { get; set; } = null!;
	[Inject] private IJSRuntime JSRuntime { get; set; } = null!;
	public class DOMRect
	{
		public double Top { get; set; }
		public double Left { get; set; }
		public double Width { get; set; }
		public double Height { get; set; }
	}
	public class MousePosition
	{
		public double X { get; set; }
		public double Y { get; set; }
	}

	private SKGLView? _skiaView;

	private MouseEventData _mouseEvent = new();

	public double Scale = 1.0;

	public bool RednerFlag = false;

	protected override void OnInitialized()
	{
		// AnimationRegister.Dump();
		// LayerBoxRegister.Dump();
		// _timer = new Timer(_ =>
		// {
		// 	// 注意：因为定时器的回调在另一个线程中运行，所以我们需要调用InvokeAsync来确保在正确的线程上更新UI
		// 	InvokeAsync(() =>
		// 	{
		// 		_skiaView?.Invalidate();
		// 	});
		// }, null, 0, 100);  // 首次执行延迟0毫秒，然后每100毫秒执行一次
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			// await JSRuntime.InvokeVoidAsync("updateCanvasSize", "yourCanvasId", 0.5);
			await Task.Run(() => { });
		}
	}

	protected override async Task OnParametersSetAsync()
	{
		await Manager.Init(Game);
	}

	private async void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
	{
		await OnMouseMoveAsync();
		long startMiniSecond = NowTime.Minisecond;
		var mouseEventCopy = _mouseEvent;
		var canvas = e.Surface.Canvas;

		MouseStatusUpdate();
		await Manager.ProcEvent(mouseEventCopy);
		canvas.Clear();
		Manager.Render(canvas);

		int sec = DateTimeOffset.UtcNow.Second;
		if (sec != _lastSec)
		{
			_lastSec = sec;
			GameStatus.FPS = _frameCount;
			_frameCount = 0;
			await InvokeAsync(StateHasChanged);
		}
		_frameCount++;
		GameStatus.FrameTime = (int)(NowTime.Minisecond - startMiniSecond);
		GameStatus.MouseX = _mouseEvent.Position.X;
		GameStatus.MouseY = _mouseEvent.Position.Y;
	}


	// 下方为处理鼠标事件的函数，任何界面事件都应该在界面预处理后传给游戏引擎处理
	private void OnMouseMove(MouseEventArgs e)
	{
		IVector mousePos = new((int)e.OffsetX, (int)e.OffsetY);
		_mouseEvent.Move = mousePos - _mouseEvent.Position;
		_mouseEvent.Position = mousePos;
	}

	private async Task OnMouseMoveAsync()
	{
		var mousePos = await JSRuntime.InvokeAsync<IVector>("eval", ["window.globalVar"]);
		// IVector mousePos = new((int)e.OffsetX, (int)e.OffsetY);
		_mouseEvent.Move = mousePos - _mouseEvent.Position;
		_mouseEvent.Position = mousePos;
	}

	private void OnMouseUp(MouseEventArgs e)
	{
		_mouseEvent.Status = MouseStatus.Up;

		if (_mouseEvent.Button == MouseButton.Empty || _mouseEvent.Button == MouseButton.MouseChord)
			return;

		_mouseEvent.Button = e.Button switch
		{
			0L => (_mouseEvent.Button == MouseButton.RButton) ? MouseButton.MouseChord : MouseButton.LButton,
			1L => MouseButton.MButton,
			2L => (_mouseEvent.Button == MouseButton.LButton) ? MouseButton.MouseChord : MouseButton.RButton,
			_ => MouseButton.Empty,
		};
	}

	private void OnMouseDown(MouseEventArgs e)
	{
		_mouseEvent.Status = MouseStatus.Down;

		_mouseEvent.Button = e.Button switch
		{
			0L => (_mouseEvent.Button == MouseButton.RButton) ? MouseButton.MouseChord : MouseButton.LButton,
			1L => MouseButton.MButton,
			2L => (_mouseEvent.Button == MouseButton.LButton) ? MouseButton.MouseChord : MouseButton.RButton,
			_ => MouseButton.Empty,
		};
	}

	private void MouseStatusUpdate()
	{
		if (_mouseEvent.Status == MouseStatus.Up)
		{
			_mouseEvent.Status = MouseStatus.Release;
			_mouseEvent.Button = MouseButton.Empty;
		}
		if (_mouseEvent.Status == MouseStatus.Down)
			_mouseEvent.Status = MouseStatus.Hold;
	}

	// public async void Dispose()
	// {
	// 	// if (_context is not null)
	// 	// 	await _context.DisposeAsync();
	// }

	#region Debug
	private int _frameCount = 0, _lastSec;
	#endregion
};