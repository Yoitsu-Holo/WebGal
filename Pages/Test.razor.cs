using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SkiaSharp;
using SkiaSharp.Views.Blazor;
using WebGal.Global;
using WebGal.Handler.Event;
using WebGal.Services;
using WebGal.Types;

namespace WebGal.Pages;

public partial class Test// : IDisposable
{
	[Parameter] public string Game { get; set; } = null!;
	[Inject] private GameManager Manager { get; set; } = null!;

	private MouseEventData _mouseEvent = new();
	private SKTypeface? paintTypeface;


	protected override void OnInitialized()
	{
		// _audioTest = new(jsRuntime);
		// _audioGain = new(jsRuntime);
		// _audioSource = new(jsRuntime);
		// _audioSpeeker = new(jsRuntime);
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			await Task.Run(() => { });
			Manager.Init(Game);

			// ! test
			// var audioBuffer = await httpClient.GetByteArrayAsync("Data/Test1/pack/sound/bgm/bgm02_b.ogg");

			// _context = await AudioContext.CreateAsync(jsRuntime);
			// await _audioSource!.SetContextAsync(_context);
			// await _audioGain!.SetContextAsync(_context);
			// await _audioSpeeker!.SetContextAsync(_context);

			// await _audioSource.SetAudioBuffer(audioBuffer);

			// await _audioSource.ConnectToAsync(_audioGain, new AudioWire(0, 0));
			// await _audioGain.ConnectToAsync(_audioSpeeker, new AudioWire(0, 0));

			// await _audioSource.SetAudioLoop(true);
			// await _audioSource.PlayAsync();

			// await _audioTest!.SetContextAsync(_context);
			// await _audioTest.SetAudioAsync(audioBuffer);
			// await _audioTest.SetLoop(true);
			// await _audioTest.StartAsync();
		}
	}

	protected override async Task OnParametersSetAsync()
	{
		await Task.Run(() => { });
		// await Manager.DoTest(Game);
	}

	private async void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
	{
		long startMiniSecond = NowTime.Minisecond;
		var mouseEventCopy = _mouseEvent;
		var canvas = e.Surface.Canvas;

		MouseStatusUpdate();
		await Manager.ProcEvent(mouseEventCopy);
		Manager.Render(canvas);

		//! audio test
		// var tm = NowTime.Minisecond;
		// float volume = tm % 6000;
		// volume += 1000;
		// volume /= 7000;

		// if (_audioGain is not null)
		// 	try { await _audioGain.SetGainASync(volume); }
		// 	catch { };

		int sec = DateTimeOffset.UtcNow.Second;
		if (sec != _lastSec)
		{
			_lastSec = sec;
			_fps = _frameCount;
			_frameCount = 0;
			await InvokeAsync(StateHasChanged);
			// throw new Exception("Test Error");
		}
		_frameTime = (int)(NowTime.Minisecond - startMiniSecond);
		_frameCount++;
	}


	// 下方为处理鼠标事件的函数，任何界面事件都应该在界面预处理后传给游戏引擎处理
	private void OnMouseMove(MouseEventArgs e)
	{
		IVector mousePos = new((int)e.OffsetX, (int)e.OffsetY);
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

	// public async void Dispose()
	// {
	// 	// if (_context is not null)
	// 	// 	await _context.DisposeAsync();
	// }

	#region Debug
	private int _frameCount = 0, _frameTime = 0, _fps = 0, _lastSec;
	#endregion
};