using WebGal.API;
using WebGal.API.Data;
using FileInfo = WebGal.API.Data.FileInfo;

namespace WebGal.MeoInterpreter;

public partial class MoeInterpreter
{
	public partial class Syscall
	{
		public static async void BGM(MoeVariable file)
		{
			FileInfo fileInfo = new()
			{
				Type = FileType.Audio,
				Name = _elfHeader.Files[file].Name,
				URL = _elfHeader.Files[file].URL,
			};
			if (Driver.CheckFile(fileInfo).Type == ResponseType.Fail)
				await Driver.PullFileAsync(fileInfo);
			AudioSimpleInfo audioInfo = new()
			{
				ID = new() { ContextID = 0, NodeID = 0 },

				AudioName = file,
				Loop = true,
				Start = true,
				Volume = 600,
			};
			await Driver.SetAudioSimpleInfoAsync(audioInfo);
		}

		public static async void VO(MoeVariable file)
		{
			FileInfo fileInfo = new()
			{
				Type = FileType.Audio,
				Name = _elfHeader.Files[file].Name,
				URL = _elfHeader.Files[file].URL,
			};
			if (Driver.CheckFile(fileInfo).Type == ResponseType.Fail)
				await Driver.PullFileAsync(fileInfo);
			AudioSimpleInfo audioInfo = new()
			{
				ID = new() { ContextID = 0, NodeID = 1 },

				AudioName = file,
				Loop = false,
				Start = true,
				Volume = 600,
			};
			await Driver.SetAudioSimpleInfoAsync(audioInfo);
		}

		public static async void SE(MoeVariable file)
		{
			FileInfo fileInfo = new()
			{
				Type = FileType.Audio,
				Name = _elfHeader.Files[file].Name,
				URL = _elfHeader.Files[file].URL,
			};
			if (Driver.CheckFile(fileInfo).Type == ResponseType.Fail)
				await Driver.PullFileAsync(fileInfo);
			AudioSimpleInfo audioInfo = new()
			{
				ID = new() { ContextID = 0, NodeID = 2 },

				AudioName = file,
				Loop = false,
				Start = true,
				Volume = 600,
			};
			await Driver.SetAudioSimpleInfoAsync(audioInfo);
		}

		public static void BG(MoeVariable file, MoeVariable subx, MoeVariable suby, MoeVariable width, MoeVariable height)
		{ SetImageBox(0, 0, file, subx, suby, width, height); }

		public static void TEXT(MoeVariable name, MoeVariable text)
		{
			SetTextBox(0, 3, text, "simhei", 30);
			SetTextBox(0, 4, name, "simhei", 30);
		}
	}
}