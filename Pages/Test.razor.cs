using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using SkiaSharp.Views.Blazor;
using WebGal.Event;
using WebGal.Global;
using WebGal.Libs.Base;
using WebGal.Services;

namespace WebGal.Pages;

public partial class Test
{
	[Inject] private IJSRuntime JS { get; set; } = null!;
	[Parameter] public string Game { get; set; } = null!;
	[Inject] private GameManager Manager { get; set; } = null!;

	private Dictionary<string, Audio> _loopAudios = new();
	private Dictionary<string, Audio> _oneShotAudios = new();

	public Dictionary<string, Audio> Audio = new();

	private MouseEvent _mouseEvent = new();

	protected override void OnInitialized()
	{
		var audio = new Audio()
		{
			Name = "test",
			URL = "Data/Test1/pack/sound/bgm/song01.ogg",
			Loop = true
		};
		Audio["test"] = audio;
	}

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

		Manager.Render(e.Surface.Canvas, NowTime.Minisecond);

		//! test
		if (_loopAudios.Count != 0)
		{
			float vol = NowTime.Minisecond % 5000;
			vol /= 10000;
			vol += 0.5f;
			await JS.InvokeVoidAsync("setAudioVolume", vol);
		}

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