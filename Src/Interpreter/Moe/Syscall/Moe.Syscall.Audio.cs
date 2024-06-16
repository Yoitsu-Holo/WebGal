using WebGal.API;
using WebGal.API.Data;
using FileInfo = WebGal.API.Data.FileInfo;

namespace WebGal.MeoInterpreter;

public partial class MoeInterpreter
{
	public partial class Syscall
	{
		#region Syscall
		public static void BGM(MoeVariable file) => RawBGM(file);
		public static void VO(MoeVariable file) => RawVO(file);
		public static void SE(MoeVariable file) => RawSE(file);
		#endregion

		#region RawSyscall
		private static async void RawBGM(MoeVariable file)
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

		private static async void RawVO(MoeVariable file)
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

		private static async void RawSE(MoeVariable file)
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
		#endregion
	}
}
