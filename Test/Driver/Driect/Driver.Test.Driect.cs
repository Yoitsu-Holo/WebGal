using SkiaSharp;
using WebGal.Animations;
using WebGal.API.Data;
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
			await _resourceManager!.PullImageAsync("thumb", "/Image/Thumb.png");
			await _resourceManager!.PullImageAsync("track", "/Image/Track.png");

			//!
			WidgetImageBox imageLayer1 = new()
			{
				Size = new(40, 40),
				Position = new(400, 400),
				Offset = new(-20, -20),
			};
			IAnimation animation = AnimationRegister.GetAnimation("AnimationRotate");
			animation.SetParama(new AnimationRotateData() { Z = 2.5f, });
			imageLayer1.AddAnimation(0, animation);
			// imageLayer1.Animation.SetParama();

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
			ControllerButton button = new()
			{
				Size = new(316, 45),
				Position = new(1280 / 2, 720 / 2),
				Offset = new(-316 / 2, -45 / 2),
			};
			button.SetImage(_resourceManager!.GetImage("title"), new IRect(1, 723, 316, 45), 0);
			button.SetImage(_resourceManager!.GetImage("title"), new IRect(321, 723, 316, 45), 1);
			button.SetImage(_resourceManager!.GetImage("title"), new IRect(641, 723, 316, 45), 2);
			_layoutManager.Layouts[_menuLayout].Layers[1] = button;


			//!
			RegisteLayer(new LayerBox()
			{
				Attribute = new()
				{
					ID = new() { LayoutID = _menuLayout, LayerID = 2 },
					Position = new(100, 100),
					Size = new(300, 22),
					Type = "ControllerSliderHorizontal",
				},
			});

			SetSliderBoxInfo(new SliderBoxInfo()
			{
				ID = new() { LayoutID = _menuLayout, LayerID = 2 },
				TrackImage = new() { ImageName = "track", },

				FocusedImage = new() { ImageName = "thumb", },
				HoverImage = new() { ImageName = "thumb", },
				NormalImage = new() { ImageName = "thumb", },
				PressedImage = new() { ImageName = "thumb", },

				ThumbSize = new(12, 22),
				TrackSize = new(300, 22),
			});

			_layoutManager.ActiveLayout = _menuLayout;

			HandlerBase handler = new();

			_layoutManager.Layouts[_menuLayout].Layers[1].RegisterAction(new ActionBase()
			{
				Action = (value) =>
				{
					Console.WriteLine("Trigger!");
					if (value is MouseEventData mouse)
					{
						if (mouse.Status == MouseStatus.Hold)
							_layoutManager.ActiveLayout = _gameLayout;
					}
					return true;
				}
			});
		}
	}
}