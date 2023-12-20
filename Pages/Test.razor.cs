using KristofferStrube.Blazor.WebAudio;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using SkiaSharp;
using SkiaSharp.Views.Blazor;
using WebGal.Audio;
using WebGal.Event;
using WebGal.Global;
using WebGal.Libs.Base;
using WebGal.Services;
using WebGal.Types;

namespace WebGal.Pages;

public partial class Test : IDisposable
{
	[Inject] private IJSRuntime jsRuntime { get; set; } = null!;
	[Inject] private HttpClient httpClient { get; set; } = null!;
	[Parameter] public string Game { get; set; } = null!;
	[Inject] private GameManager Manager { get; set; } = null!;

	private MouseEvent _mouseEvent = new();
	private SKTypeface? paintTypeface;

	private AudioSource? _audioSource;
	private AudioGain? _audioGain;
	private AudioSpeeker? _audioSpeeker;
	// private AudioSimple? _audioTest;
	private AudioContext? _context;

	protected override void OnInitialized()
	{
		// _audioTest = new(jsRuntime);
		_audioGain = new(jsRuntime);
		_audioSource = new(jsRuntime);
		_audioSpeeker = new(jsRuntime);
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			var paintBytes = await httpClient.GetByteArrayAsync("/Data/simhei.ttf");
			using var paintStream = new MemoryStream(paintBytes);
			paintTypeface = SKTypeface.FromStream(paintStream);

			// ! test
			var audioBuffer = await httpClient.GetByteArrayAsync("Data/Test1/pack/sound/bgm/bgm02_b.ogg");

			_context = await AudioContext.CreateAsync(jsRuntime);
			await _audioSource!.SetContextAsync(_context);
			await _audioGain!.SetContextAsync(_context);
			await _audioSpeeker!.SetContextAsync(_context);

			await _audioSource.SetAudioBuffer(audioBuffer);

			await _audioSource.ConnectToAsync(_audioGain, new AudioWire(0, 0));
			await _audioGain.ConnectToAsync(_audioSpeeker, new AudioWire(0, 0));

			await _audioSource.SetAudioLoop(true);
			await _audioSource.PlayAsync();

			// await _audioTest!.SetContextAsync(_context);
			// await _audioTest.SetAudioAsync(audioBuffer);
			// await _audioTest.SetLoop(true);
			// await _audioTest.StartAsync();
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

		//! test
		var tm = NowTime.Minisecond;
		float volume = tm % 6000;
		volume += 1000;
		volume /= 7000;

		if (_audioGain is not null)
			try
			{
				await _audioGain.SetGainASync(volume);
			}
			catch { };

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


	// 下方为处理鼠标事件的函数，任何界面事件都应该在界面预处理后传给游戏引擎处理

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

	public async void Dispose()
	{
		if (_context is not null)
			await _context.DisposeAsync();
	}

	#region Debug
	private int _frameCount = 0, _fps = 0, _lastSec;
	#endregion
};