using KristofferStrube.Blazor.WebAudio;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using SkiaSharp;
using SkiaSharp.Views.Blazor;
using WebGal.Event;
using WebGal.Global;
using WebGal.Libs.Base;
using WebGal.Services;

namespace WebGal.Pages;

public partial class Test
{
	[Inject] private HttpClient httpClient { get; set; } = null!;
	[Parameter] public string Game { get; set; } = null!;
	[Inject] private GameManager Manager { get; set; } = null!;

	private MouseEvent _mouseEvent = new();
	private SKTypeface? paintTypeface;

	protected override void OnInitialized()
	{
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			var paintBytes = await httpClient.GetByteArrayAsync("/Data/simhei.ttf");
			using var paintStream = new MemoryStream(paintBytes);
			paintTypeface = SKTypeface.FromStream(paintStream);
		}
	}

	protected override async Task OnParametersSetAsync()
	{
		Manager.Clear();
		await Manager.DoTest(Game);
	}

	private async void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
	{
		var mouseEventCopy = _mouseEvent;
		MouseStatusUpdate();
		await Manager.ProcessMouseEvent(mouseEventCopy);

		var canvas = e.Surface.Canvas;
		Manager.Render(canvas, NowTime.Minisecond, true);

		//! test
		TextBox tb = new TextBox
		{
			Text = "这是一段中文文本测试，测试包含ascii可打印字符的显示、换行，以及中文字体的加载 1234567890 ABCDEFGHIJKLMNOPQRSTUVWXYZ abcdefghijklmnopqrstuvwxyz ,.:+-=_!@#$%^&*'\"`~ <>()[]{} /|\\",
			BoxStyle = new()
			{
				BoxSize = new IVector(1080, 400),
				BoxPos = new(100, 100),
				MarginBottom = 20
			},
			TextPaint = new()
			{
				Color = SKColors.Bisque,
				IsAntialias = true,
				TextSize = 30,
				Typeface = paintTypeface
			}
		};

		canvas.DrawTextBox(tb);
		// Console.WriteLine(await context.GetCurrentTimeAsync());

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
		_mouseEvent.Position = new IVector((int)e.OffsetX, (int)e.OffsetY);
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
		// Console.WriteLine(e.Button);

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

	#region Debug
	private int _frameCount = 0, _fps = 0, _lastSec;
	#endregion
};