using WebGal.API;
using WebGal.API.Data;
using FileInfo = WebGal.API.Data.FileInfo;

namespace WebGal.MeoInterpreter;

public partial class MoeInterpreter
{
	public partial class Syscall
	{
		#region Syscall
		public static void RegAudioContext(MoeVariable contextID)
			=> RawRegAudioContext(contextID);
		public static void RegAudioNode(MoeVariable contextID, MoeVariable nodeID, MoeVariable type)
			=> RawRegAudioNode(contextID, nodeID, type);
		public static void BGM(MoeVariable file)
			=> RawBGM(file);
		public static void VO(MoeVariable file)
			=> RawVO(file);
		public static void SE(MoeVariable file)
			=> RawSE(file);
		#endregion

		#region RawSyscall
		private static async void RawRegAudioContext(int contextID)
		{
			await Driver.RegisterAudioContextAsync(new AudioIdInfo() { ContextID = contextID, });
		}

		private static async void RawRegAudioNode(int contextID, int nodeID, string type)
		{
			await Task.Run(() => { });  // Make Complier happy

			// await Driver.RegisterAudioNodeAsync(new AudioNodeInfo() { ID = new() { ContextID = contextID, NodeID = nodeID }, Type = type });
		}

		private static async void RawBGM(MoeVariable file)
		{
			FileInfo fileInfo = new()
			{
				Type = FileType.Audio,
				Name = _elfHeader.AudioFiles[file].Name,
				URL = _elfHeader.AudioFiles[file].URL,
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
				Name = _elfHeader.AudioFiles[file].Name,
				URL = _elfHeader.AudioFiles[file].URL,
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
				Name = _elfHeader.AudioFiles[file].Name,
				URL = _elfHeader.AudioFiles[file].URL,
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
