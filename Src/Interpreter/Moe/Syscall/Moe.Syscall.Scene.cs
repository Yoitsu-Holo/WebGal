using System.Text.Json;
using WebGal.API;
using WebGal.API.Data;
using FileInfo = WebGal.API.Data.FileInfo;

namespace WebGal.MeoInterpreter;

public partial class MoeInterpreter
{
	public partial class Syscall
	{
		#region Syscall
		public static void ParseSceneList(MoeVariable sceneName) => RawParseSceneList(sceneName);
		public static void SetSceneList(MoeVariable sceneName) => RawSetSceneList(sceneName);
		public static void OnCLick() => RawOnCLick();
		public static void LoadScene(MoeVariable sceneid) => RawLoadScene(sceneid);
		public static void BG(MoeVariable file, MoeVariable subx, MoeVariable suby, MoeVariable width, MoeVariable height) => RawBG(file, subx, suby, width, height);
		public static void TEXT(MoeVariable name, MoeVariable text) => RawTEXT(name, text);
		#endregion

		#region RawSyscall

		private static void RawParseSceneList(string sceneName)
		{
			Response response;
			FileInfo elfFile = new()
			{
				Type = FileType.Script,
				Name = _elfHeader.TextFiles[sceneName].Name,
				URL = _elfHeader.TextFiles[sceneName].URL,
			};

			response = Driver.GetScriptAsync(elfFile);
			if (response.Type != ResponseType.Success) throw new Exception(response.Message);

			SceneList scenes = JsonSerializer.Deserialize<SceneList>(response.Message);

			_scenes[_elfHeader.TextFiles[sceneName].Name] = scenes;
		}

		private static void RawSetSceneList(string sceneName) => _activeSceneList = _scenes[sceneName];

		private static void RawOnCLick()
		{
			_activeSceneList.SceneId = _activeSceneList.NextSceneID;
			LoadScene(_activeSceneList.SceneId);
		}

		private static void RawLoadScene(int sceneid = 0)
		{
			Scene scene = _activeSceneList.Scenes[sceneid];

			foreach (Behave behave in scene.Behaves)
			{
				FunctionCallNode funcCall = new()
				{
					CallType = FuncCallType.Keyword,
					FunctionName = behave.Func,
				};

				foreach (var (key, value) in behave.Param)
				{
					ExpressionNode exp = new();
					if (value is int ivalue)
						exp.Tokens.Add(new() { Type = OperatorType.Number, Number = ivalue });
					else if (value is float fvalue)
						exp.Tokens.Add(new() { Type = OperatorType.Number, Number = fvalue });
					else if (value is string svalue)
						exp.Tokens.Add(new() { Type = OperatorType.String, String = svalue });
					else
						throw new Exception("错误的输入类型");
					funcCall.KeywordParams[key] = exp;
				}

				Call(funcCall);
			}
		}

		private static void RawBG(string file, int subx, int suby, int width, int height)
		{ SetImageBox(0, 0, file, subx, suby, width, height); }

		private static void RawTEXT(string name, string text)
		{
			SetTextBox(0, 3, text, "simhei", 30);
			SetTextBox(0, 4, name, "simhei", 30);
		}
		#endregion
	}
}