using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using SkiaSharp;
using SkiaSharp.Views.Blazor;
using WebGal.Services;

namespace WebGal.Pages;

public partial class Test
{
	[Parameter]
	public string? Game { get; set; }

	[Inject]
	private GameManager Manager { get; set; } = null!;

	[Inject]
	private IJSRuntime JS { get; set; } = null!;

	[Inject]
	private HttpClient httpClient { get; set; } = null!;

	private Dictionary<string, string> _loopAudios = new();
	private Dictionary<string, string> _oneShotAudios = new();

	protected override async Task OnInitializedAsync()
	{
		await Manager.DoTest();
		Manager.LoadMedia(_loopAudios, _oneShotAudios);

		// var audioFile = await httpClient.GetByteArrayAsync("/Data/Test/pack/sound/bgm/bgm02_b.ogg");
		// Console.WriteLine(Convert.ToBase64String(audioFile));
		// var audio = await JS.InvokeAsync<string>("audioOggToLink", audioFile);
		// _loopAudios.Add("bgm1", audio);
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