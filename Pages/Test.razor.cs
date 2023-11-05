using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using SkiaSharp;
using SkiaSharp.TextBlocks;
using SkiaSharp.Views.Blazor;
using Topten.RichTextKit;
using WebGal.Event;
using WebGal.Global;
using WebGal.Libs.Base;
using WebGal.Services;

namespace WebGal.Pages;

public partial class Test : IAsyncDisposable
{
	[Inject] private HttpClient httpClient { get; set; } = null!;
	[Inject] private IJSRuntime JS { get; set; } = null!;
	[Parameter] public string Game { get; set; } = null!;
	[Inject] private GameManager Manager { get; set; } = null!;

	private Dictionary<string, Audio> _loopAudios = new();
	private Dictionary<string, Audio> _oneShotAudios = new();

	public Dictionary<string, Audio> Audio = new();

	private MouseEvent _mouseEvent = new();

	private IJSObjectReference _module = null!;

	private SKTypeface? paintTypeface;

	protected override void OnInitialized()
	{
		var audio = new Audio()
		{
			Name = "test",
			URL = "Data/Test1/pack/sound/bgm/bgm02_b.ogg",
			Loop = true
		};
		Audio["test"] = audio;
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			_module = await JS.InvokeAsync<IJSObjectReference>("import", "./Pages/Test.razor.js");
			var paintBytes = await httpClient.GetByteArrayAsync("/Data/simhei.ttf");
			using var paintStream = new MemoryStream(paintBytes);
			paintTypeface = SKTypeface.FromStream(paintStream);
		}
	}
	//141538.479
	protected override async Task OnParametersSetAsync()
	{
		//todo 未解决循环卡顿问题
		_loopAudios.Clear();
		_oneShotAudios.Clear();
		Manager.Clear();
		// Manager.SetMediaList(_loopAudios, _oneShotAudios);
		await Manager.DoTest(Game);
		// await Manager.LoadMedia();

		//! test
		foreach (var (_, audio) in Audio)
		{
			if (audio.Loop)
				_loopAudios[audio.Name] = audio;
			else
				_oneShotAudios[audio.Name] = audio;
		}
	}

	private async void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
	{
		var mouseEventCopy = _mouseEvent;
		MouseStatusUpdate();
		await Manager.ProcessMouseEvent(mouseEventCopy);

		var canvas = e.Surface.Canvas;
		canvas.SetTypeface(paintTypeface);
		Manager.Render(canvas, NowTime.Minisecond);

		//! test
		if (_loopAudios.Count != 0)
		{
			float vol = NowTime.Minisecond % 8000;
			vol /= 10000;
			vol += 0.2f;
			await _module.InvokeVoidAsync("setAudioVolume", vol, "test");
			await _module.InvokeVoidAsync("getAudioLength", "test");
		}

		// e.Surface.Canvas.DrawTextBlock("Hello world!", new SKRect(50, 50, 150, 0), new Font(14), SKColors.Black);

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

	public async ValueTask DisposeAsync()
	{
		if (_module is not null)
			await _module.DisposeAsync();
	}

	#region Debug
	private int _frameCount = 0, _fps = 0, _lastSec;
	#endregion
};