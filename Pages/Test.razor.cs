using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks.Dataflow;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using SkiaSharp;
using SkiaSharp.Views.Blazor;
using WebGal.Global;
using WebGal.Services;
using WebGal.Services.Module;

namespace WebGal.Pages;

public partial class Test : IDisposable
{
	[Parameter] public string? Game { get; set; }

	[Inject] private GameManager Manager { get; set; } = null!;


	private Dictionary<string, string> _loopAudios = new();
	private Dictionary<string, string> _oneShotAudios = new();

	SKBitmap bitmap = new(100, 100);

	protected override void OnInitialized()
	{
		Console.WriteLine(DateTimeOffset.Now.Ticks / 10000L);
		SKColor[] pixs = bitmap.Pixels;
		for (int row = 0, it = 0; row < 100; row++)
		{
			for (int col = 0; col < 100; col++)
			{
				int dx = 49 - row;
				int dy = 49 - col;
				double delta = Math.Sqrt(dx * dx + dy * dy);
				pixs[it] = new SKColor(0, 180, 255, (byte)Math.Max(0, 255 - delta * 7));
				it++;
			}
		}
		bitmap.Pixels = pixs;
		Console.WriteLine(DateTimeOffset.Now.Ticks / 10000L);
	}

	protected override async Task OnParametersSetAsync()
	{
		await Manager.DoTest(Game);
		Manager.SetMediaList(_loopAudios, _oneShotAudios);
		Manager.LoadMedia();
	}


	protected override void OnAfterRender(bool firstRender)
	{
		if (!firstRender)
			return;
	}

	private void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
	{
		Console.WriteLine(Game);
		SKCanvas canvas = e.Surface.Canvas;
		Manager.SetTargetCanvas(canvas);

		Console.WriteLine("Index");//!
		Manager.GetFrame(DateTimeOffset.Now.Ticks / 10000L, true);
		canvas.DrawBitmap(bitmap, new SKPoint(_mousePos.X - 50, _mousePos.Y - 50));

		#region Test 
		#endregion

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
		_mousePos = ((int)e.OffsetX, (int)e.OffsetY);
		_mousePos.X = Math.Max(0, Math.Min(1279, _mousePos.X));
		_mousePos.Y = Math.Max(0, Math.Min(719, _mousePos.Y));
	}

	private void OnClick(MouseEventArgs e)
	{
		// _clickPos = $"{e.OffsetX}, {e.OffsetY}";
		Manager.OnClick(new SKPoint((float)e.OffsetX, (float)e.OffsetY));
	}
	#region Debug
	private string _text = "", _clickPos = "";
	private (int X, int Y) _mousePos;
	private int _frameCount = 0, _fps = 0, _lastSec;
	private bool disposedValue;

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				// TODO: 释放托管状态(托管对象)
			}


			// TODO: 释放未托管的资源(未托管的对象)并重写终结器
			// TODO: 将大型字段设置为 null
			disposedValue = true;
		}
	}

	// // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
	// ~Test()
	// {
	//     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
	//     Dispose(disposing: false);
	// }

	public void Dispose()
	{
		// 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
	#endregion
};