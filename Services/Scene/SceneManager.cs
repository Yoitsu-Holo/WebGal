using WebGal.Services.Module;

namespace WebGal.Services;

public class SceneManager
{
	private readonly Dictionary<string, Scene> _scenes = new();

	public void PushScene(string sceneName, Scene scene)
	{
		_scenes[sceneName] = scene;
	}

	public Scene LoadScene(string sceneName)
	{
		if (_scenes.ContainsKey(sceneName))
			return _scenes[sceneName];
		throw new Exception("no key");
	}

	public void RemoveScene(string sceneName)
	{
		_scenes.Remove(sceneName);
	}
}