
using System.Text.Json;
using WebGal.API;
using WebGal.API.Data;
using FileInfo = WebGal.API.Data.FileInfo;

namespace WebGal.MeoInterpreter;

public partial class MoeInterpreter
{
	public static async void LoadScene(string sceneName)
	{
		Response response;
		FileInfo elfFile = new()
		{
			Type = FileType.Script,
			Name = _elfHeader.Files[sceneName].Name,
			URL = _elfHeader.Files[sceneName].URL,
		};

		response = await Driver.PullFileAsync(elfFile);
		if (response.Type != ResponseType.Success) throw new Exception(response.Message);

		response = await Driver.GetScriptAsync(elfFile);
		if (response.Type != ResponseType.Success) throw new Exception(response.Message);

		Console.WriteLine(response.Message);
		SceneList scenes = JsonSerializer.Deserialize<SceneList>(response.Message);
		Console.WriteLine(scenes.SceneName);
		// foreach (Scene scene in scenes.Scenes)
		// {
		// 	Console.WriteLine(scene.SceneID);
		// }
	}
}