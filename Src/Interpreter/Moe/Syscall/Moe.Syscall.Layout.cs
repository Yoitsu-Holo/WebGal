using WebGal.API;
using WebGal.API.Data;
using FileInfo = WebGal.API.Data.FileInfo;

namespace WebGal.MeoInterpreter;

public partial class MoeInterpreter
{
	public partial class Syscall
	{
		#region Syscall
		public static void RegLayout(MoeVariable layout)
			=> RawRegLayout(layout);
		public static void SetLayout(MoeVariable layout)
			=> RawSetLayout(layout);
		public static void RegLayer(MoeVariable layout, MoeVariable layer, MoeVariable type, MoeVariable posx, MoeVariable posy, MoeVariable width, MoeVariable height)
			=> RawRegLayer(layout, layer, type, posx, posy, width, height);
		public static void SetImageBox(MoeVariable layout, MoeVariable layer, MoeVariable image, MoeVariable subx, MoeVariable suby, MoeVariable width, MoeVariable height)
			=> RawSetImageBox(layout, layer, image, subx, suby, width, height);
		public static void SetTextBox(MoeVariable layout, MoeVariable layer, MoeVariable text, MoeVariable font, MoeVariable size)
			=> RawSetTextBox(layout, layer, text, font, size);
		public static void SetColorBox(MoeVariable layout, MoeVariable layer, MoeVariable a, MoeVariable r, MoeVariable g, MoeVariable b)
			=> RawSetColorBox(layout, layer, a, r, g, b);
		public static void SetButtonBox(MoeVariable layout, MoeVariable layer, MoeVariable normal, MoeVariable hover, MoeVariable pressed, MoeVariable focused)
			=> RawSetButtonBox(layout, layer, normal, hover, pressed, focused);
		public static void SetSliderBox(MoeVariable layout, MoeVariable layer, MoeVariable track, MoeVariable normal, MoeVariable hover, MoeVariable pressed, MoeVariable focused)
			=> RawSetSliderBox(layout, layer, track, normal, hover, pressed, focused);
		#endregion


		#region RawSyscall
		private static void RawRegLayout(int layout)
		{
			LayerIdInfo layoutInfo = new() { LayoutID = layout, };
			Driver.RegisteLayout(layoutInfo);
		}

		private static void RawSetLayout(int layout)
		{
			LayerIdInfo layoutInfo = new() { LayoutID = layout, };
			Driver.SetActiveLayout(layoutInfo);
		}

		private static void RawRegLayer(int layout, int layer, string type, int posx, int posy, int width, int height)
		{
			LayerBox layerBox = new()
			{
				Attribute = new()
				{
					ID = new() { LayoutID = layout, LayerID = layer, },
					Type = type,
					Position = new() { X = posx, Y = posy },
					Size = new() { X = width, Y = height },
				}
			};
			Driver.RegisteLayer(layerBox);
		}

		private static async void RawSetImageBox(int layout, int layer, string image, int subx, int suby, int width, int height)
		{
			ImageBoxInfo imageBox = new()
			{
				ID = new() { LayoutID = layout, LayerID = layer, },
				Image = new()
				{
					ImageName = _elfHeader.ImageFiles[image].Name,
					SubRect = new() { X = subx, Y = suby, W = width, H = height },
				},
			};
			FileInfo file = new()
			{
				Type = FileType.Image,
				Name = _elfHeader.ImageFiles[image].Name,
				URL = _elfHeader.ImageFiles[image].URL,
			};
			if (Driver.CheckFile(file).Type == ResponseType.Fail)
				await Driver.PullFileAsync(file);
			Driver.SetImageBoxInfo(imageBox);
		}

		private static async void RawSetTextBox(int layout, int layer, string text, string font, int size)
		{
			TextBoxInfo textBox = new()
			{
				ID = new() { LayoutID = layout, LayerID = layer, },
				Text = text,
				Font = font,
				FontSize = size,
			};
			FileInfo file = new()
			{
				Type = FileType.Font,
				Name = _elfHeader.BinFiles[font].Name,
				URL = _elfHeader.BinFiles[font].URL,
			};
			if (Driver.CheckFile(file).Type == ResponseType.Fail)
				await Driver.PullFileAsync(file);
			Driver.SetTextBoxInfo(textBox);
		}

		private static void RawSetColorBox(int layout, int layer, int a, int r, int g, int b)
		{
			ColorBoxInfo colorBox = new()
			{
				ID = new() { LayoutID = layout, LayerID = layer, },
				A = (byte)a,
				R = (byte)r,
				G = (byte)g,
				B = (byte)b,
			};
			Driver.SetColorBoxInfo(colorBox);
		}

		private static async void RawSetButtonBox(int layout, int layer, string normal, string hover, string pressed, string focused)
		{
			await Driver.PullFileAsync(new FileInfo()
			{
				Type = FileType.Image,
				Name = _elfHeader.ImageFiles[normal].Name,
				URL = _elfHeader.ImageFiles[normal].URL,
			});
			await Driver.PullFileAsync(new FileInfo()
			{
				Type = FileType.Image,
				Name = _elfHeader.ImageFiles[hover].Name,
				URL = _elfHeader.ImageFiles[hover].URL,
			});
			await Driver.PullFileAsync(new FileInfo()
			{
				Type = FileType.Image,
				Name = _elfHeader.ImageFiles[pressed].Name,
				URL = _elfHeader.ImageFiles[pressed].URL,
			});
			await Driver.PullFileAsync(new FileInfo()
			{
				Type = FileType.Image,
				Name = _elfHeader.ImageFiles[focused].Name,
				URL = _elfHeader.ImageFiles[focused].URL,
			});

			ButtonBoxInfo button = new()
			{
				ID = new() { LayoutID = layout, LayerID = layer, },
				NormalImage = new() { ImageName = normal },
				HoverImage = new() { ImageName = hover },
				PressedImage = new() { ImageName = pressed },
				FocusedImage = new() { ImageName = focused },
			};
			Driver.SetButtonBoxInfo(button);
		}


		private static async void RawSetSliderBox(int layout, int layer, string track, string normal, string hover, string pressed, string focused)
		{
			await Driver.PullFileAsync(new FileInfo()
			{
				Type = FileType.Image,
				Name = _elfHeader.ImageFiles[track].Name,
				URL = _elfHeader.ImageFiles[track].URL,
			});
			await Driver.PullFileAsync(new FileInfo()
			{
				Type = FileType.Image,
				Name = _elfHeader.ImageFiles[normal].Name,
				URL = _elfHeader.ImageFiles[normal].URL,
			});
			await Driver.PullFileAsync(new FileInfo()
			{
				Type = FileType.Image,
				Name = _elfHeader.ImageFiles[hover].Name,
				URL = _elfHeader.ImageFiles[hover].URL,
			});
			await Driver.PullFileAsync(new FileInfo()
			{
				Type = FileType.Image,
				Name = _elfHeader.ImageFiles[pressed].Name,
				URL = _elfHeader.ImageFiles[pressed].URL,
			});
			await Driver.PullFileAsync(new FileInfo()
			{
				Type = FileType.Image,
				Name = _elfHeader.ImageFiles[focused].Name,
				URL = _elfHeader.ImageFiles[focused].URL,
			});

			SliderBoxInfo button = new()
			{
				ID = new() { LayoutID = layout, LayerID = layer, },
				TrackImage = new() { ImageName = track },
				NormalImage = new() { ImageName = normal },
				HoverImage = new() { ImageName = hover },
				PressedImage = new() { ImageName = pressed },
				FocusedImage = new() { ImageName = focused },
			};
			Driver.SetSliderBoxInfo(button);
		}
		#endregion
	}
}
