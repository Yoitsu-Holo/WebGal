using System.Text.Json;
using WebGal.API;
using WebGal.API.Data;
using FileInfo = WebGal.API.Data.FileInfo;

namespace WebGal.MeoInterpreter;

public partial class MoeInterpreter
{
	public static void ParseScene(string sceneName)
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
		Scenes[_elfHeader.Files[sceneName].Name] = scenes.Scenes;
	}

	public static void LoadScene(string sceneName, int sceneid = 0)
	{
		List<Scene> scenes = Scenes[_elfHeader.Files[sceneName].Name];

		Scene scene = scenes[sceneid];

		foreach (Behave behave in scene.Behaves)
		{
			string funcName = behave.Func;
			Dictionary<string, MoeVariable> paramList = [];
			foreach (var (key, value) in behave.Param)
			{
				paramList[key] = new()
				{
					Dimension = [1],
					Access = MoeVariableAccess.Const,
				};
				if (value is int ivalue)
				{
					paramList[key].Type = MoeVariableType.Int;
					paramList[key].Init();
					paramList[key][0] = ivalue;
				}
				else if (value is double fvalue)
				{
					paramList[key].Type = MoeVariableType.Double;
					paramList[key].Init();
					paramList[key][0] = fvalue;
				}
				else if (value is string svalue)
				{
					paramList[key].Type = MoeVariableType.String;
					paramList[key].Init();
					paramList[key][0] = svalue;
				}
				else
					throw new Exception("错误的输入类型");
			}
			Call(funcName, paramList);
		}
	}
}