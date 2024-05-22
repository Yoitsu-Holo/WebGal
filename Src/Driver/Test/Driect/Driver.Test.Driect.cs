using SkiaSharp;
using WebGal.Animations;
using WebGal.Global;
using WebGal.Handler;
using WebGal.Handler.Event;
using WebGal.Layer.Controller;
using WebGal.Layer.Widget;
using WebGal.Types;

namespace WebGal.API;

public partial class Driver
{
	public partial class Test
	{
		public static async Task DirectTestAsync()
		{
			// await Task.Run(() => { });
			if (!_layoutManager!.Layouts.ContainsKey(_gameLayout))
				_layoutManager.Layouts[_gameLayout] = new();
			await _resourceManager!.PullImageAsync("title", "/Image/title01_chip.png");

			//!
			WidgetImageBox imageLayer1 = new()
			{
				Size = new(40, 40),
				Position = new(400, 400),
				Offset = new(-20, -20),
				Animation = AnimationRegister.GetAnimation("AnimationRotate"),
			};
			imageLayer1.Animation.SetParama(new AnimationRotateData() { Z = 2.5, });

			SKBitmap bitmap = new(40, 40);
			using (var canvas = new SKCanvas(bitmap))
			{
				canvas.DrawRect(0, 0, 40, 40, new SKPaint() { Color = SKColors.Bisque, }); canvas.Flush();
				imageLayer1.SetImage(bitmap);
			}

			_layoutManager.Layouts[_gameLayout].Layers[10] = imageLayer1;

			//!
			WidgetImageBox imageLayer2 = new()
			{
				Size = new(1280, 720),
				Position = new(0, 0),
			};
			imageLayer2.SetImage(_resourceManager!.GetImage("title"), new IRect(0, 0, 1280, 720));

			if (!_layoutManager!.Layouts.ContainsKey(_menuLayout))
				_layoutManager.Layouts[_menuLayout] = new();

			//! 
			ControllerButtom buttom = new()
			{
				Size = new(316, 45),
				Position = new(1280 / 2, 720 / 2),
				Offset = new(-316 / 2, -45 / 2),
			};
			buttom.SetImage(_resourceManager!.GetImage("title"), new IRect(1, 723, 316, 45), 0);
			buttom.SetImage(_resourceManager!.GetImage("title"), new IRect(321, 723, 316, 45), 1);
			buttom.SetImage(_resourceManager!.GetImage("title"), new IRect(641, 723, 316, 45), 2);
			_layoutManager.Layouts[_menuLayout].Layers[1] = buttom;

			_layoutManager.ActiveLayout = _menuLayout;

			HandlerBase handler = new()
			{
				HandlerAction = (value) =>
				{
					Console.WriteLine("Trigger!");
					if (value is MouseEventData mouse)
					{
						if (mouse.Status == MouseStatus.Hold)
							_layoutManager.ActiveLayout = _gameLayout;
					}
				}
			};
			handler.RegistEvent(_layoutManager.Layouts[_menuLayout].Layers[1]);
		}
	}
}