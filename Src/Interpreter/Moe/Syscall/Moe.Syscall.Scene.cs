using System.Text.Json;
using WebGal.API;
using WebGal.API.Data;
using FileInfo = WebGal.API.Data.FileInfo;

namespace WebGal.MeoInterpreter;

public partial class MoeInterpreter
{
	public partial class Syscall
	{
		public static void ParseSceneList(string sceneName)
		{
			Response response;
			FileInfo elfFile = new()
			{
				Type = FileType.Script,
				Name = _elfHeader.Files[sceneName].Name,
				URL = _elfHeader.Files[sceneName].URL,
			};

			response = Driver.GetScriptAsync(elfFile);
			if (response.Type != ResponseType.Success) throw new Exception(response.Message);

			SceneList scenes = JsonSerializer.Deserialize<SceneList>(response.Message);

			_scenes[_elfHeader.Files[sceneName].Name] = scenes;
		}

		public static void SetSceneList(string sceneName) => _activeSceneList = _scenes[sceneName];

		public static void OnCLick()
		{
			_activeSceneList.SceneId = _activeSceneList.NextSceneID;
			LoadScene(_activeSceneList.SceneId);
		}

		public static void LoadScene(int sceneid = 0)
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

		public static void BG(MoeVariable file, MoeVariable subx, MoeVariable suby, MoeVariable width, MoeVariable height)
		{ SetImageBox(0, 0, file, subx, suby, width, height); }

		public static void TEXT(MoeVariable name, MoeVariable text)
		{
			SetTextBox(0, 3, text, "simhei", 30);
			SetTextBox(0, 4, name, "simhei", 30);
		}
	}
}