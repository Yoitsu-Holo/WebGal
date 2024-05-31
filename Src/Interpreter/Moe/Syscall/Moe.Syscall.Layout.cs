using WebGal.API;
using WebGal.API.Data;
using FileInfo = WebGal.API.Data.FileInfo;

namespace WebGal.MeoInterpreter;

public partial class MoeInterpreter
{
	public partial class Syscall
	{
		public static void RegLayout(MoeVariable layout)
		{
			LayerIdInfo layoutInfo = new() { LayoutID = layout, };
			Driver.RegisteLayout(layoutInfo);
		}

		public static void SetLayout(MoeVariable layout)
		{
			LayerIdInfo layoutInfo = new() { LayoutID = layout, };
			Driver.SetActiveLayout(layoutInfo);
		}

		public static void RegLayer(
			MoeVariable layout, MoeVariable layer, MoeVariable type,
			MoeVariable posx, MoeVariable posy, MoeVariable width, MoeVariable height
		)
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

		public static async void SetImageBox(
			MoeVariable layout, MoeVariable layer,
			MoeVariable image,
			MoeVariable subx, MoeVariable suby, MoeVariable width, MoeVariable height
		)
		{
			ImageBoxInfo imageBox = new()
			{
				ID = new() { LayoutID = layout, LayerID = layer, },
				Image = new()
				{
					ImageName = _elfHeader.Files[image].Name,
					SubRect = new() { X = subx, Y = suby, W = width, H = height },
				},
			};
			FileInfo file = new()
			{
				Type = FileType.Image,
				Name = _elfHeader.Files[image].Name,
				URL = _elfHeader.Files[image].URL,
			};
			if (Driver.CheckFile(file).Type == ResponseType.Fail)
				await Driver.PullFileAsync(file);
			Driver.SetImageBoxInfo(imageBox);
		}

		public static async void SetTextBox(
			MoeVariable layout, MoeVariable layer,
			MoeVariable text, MoeVariable font, MoeVariable size
		)
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
				Name = _elfHeader.Files[font].Name,
				URL = _elfHeader.Files[font].URL,
			};
			if (Driver.CheckFile(file).Type == ResponseType.Fail)
				await Driver.PullFileAsync(file);
			Driver.SetTextBoxInfo(textBox);
		}

		public static void SetColorBox(
			MoeVariable layout, MoeVariable layer,
			MoeVariable a, MoeVariable r, MoeVariable g, MoeVariable b
		)
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

		public static async void SetButtonBox(
			MoeVariable layout, MoeVariable layer,
			MoeVariable normal, MoeVariable hover, MoeVariable pressed, MoeVariable focused
		)
		{
			await Driver.PullFileAsync(new FileInfo()
			{
				Type = FileType.Image,
				Name = _elfHeader.Files[normal].Name,
				URL = _elfHeader.Files[normal].URL,
			});
			await Driver.PullFileAsync(new FileInfo()
			{
				Type = FileType.Image,
				Name = _elfHeader.Files[hover].Name,
				URL = _elfHeader.Files[hover].URL,
			});
			await Driver.PullFileAsync(new FileInfo()
			{
				Type = FileType.Image,
				Name = _elfHeader.Files[pressed].Name,
				URL = _elfHeader.Files[pressed].URL,
			});
			await Driver.PullFileAsync(new FileInfo()
			{
				Type = FileType.Image,
				Name = _elfHeader.Files[focused].Name,
				URL = _elfHeader.Files[focused].URL,
			});

			ButtonBoxInfo buttom = new()
			{
				ID = new() { LayoutID = layout, LayerID = layer, },
				NormalImage = new() { ImageName = normal },
				HoverImage = new() { ImageName = hover },
				PressedImage = new() { ImageName = pressed },
				FocusedImage = new() { ImageName = focused },
			};
			Driver.SetButtonBoxInfo(buttom);
		}

		public static async void SetSliderBox(
			MoeVariable layout, MoeVariable layer,
			MoeVariable track, MoeVariable normal, MoeVariable hover, MoeVariable pressed, MoeVariable focused
		)
		{
			await Driver.PullFileAsync(new FileInfo()
			{
				Type = FileType.Image,
				Name = _elfHeader.Files[track].Name,
				URL = _elfHeader.Files[track].URL,
			});
			await Driver.PullFileAsync(new FileInfo()
			{
				Type = FileType.Image,
				Name = _elfHeader.Files[normal].Name,
				URL = _elfHeader.Files[normal].URL,
			});
			await Driver.PullFileAsync(new FileInfo()
			{
				Type = FileType.Image,
				Name = _elfHeader.Files[hover].Name,
				URL = _elfHeader.Files[hover].URL,
			});
			await Driver.PullFileAsync(new FileInfo()
			{
				Type = FileType.Image,
				Name = _elfHeader.Files[pressed].Name,
				URL = _elfHeader.Files[pressed].URL,
			});
			await Driver.PullFileAsync(new FileInfo()
			{
				Type = FileType.Image,
				Name = _elfHeader.Files[focused].Name,
				URL = _elfHeader.Files[focused].URL,
			});

			SliderBoxInfo buttom = new()
			{
				ID = new() { LayoutID = layout, LayerID = layer, },
				TrackImage = new() { ImageName = track },
				NormalImage = new() { ImageName = normal },
				HoverImage = new() { ImageName = hover },
				PressedImage = new() { ImageName = pressed },
				FocusedImage = new() { ImageName = focused },
			};
			Driver.SetSliderBoxInfo(buttom);
		}
	}
}
