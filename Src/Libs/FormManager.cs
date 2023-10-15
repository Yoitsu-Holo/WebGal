using WebGal.Libs.Base;

namespace WebGal.Libs;

public class FormManager
{
	private readonly Dictionary<string, SceneManager> _sceneManagers = new();//管理所有的场景，任何一类界面都可以归为一类场景，包括主菜单，设置，游戏内画面等

	private SceneManager? _activeSceneManager;

	public void Clear()
	{
		_sceneManagers.Clear();
	}

	public void RegistScene(string sceneManagerName)
	{
		if (_sceneManagers.ContainsKey(sceneManagerName))
			throw new Exception("_SceneManeger has registered");
		_sceneManagers[sceneManagerName] = new();
	}

	public void AddScene(string sceneManagerName, string sceneName, Scene scene)
	{
		if (!_sceneManagers.ContainsKey(sceneManagerName))
			throw new Exception("_SceneManeger has not registered");
		_sceneManagers[sceneManagerName].PushScene(sceneName, scene);
	}

	public Scene GetScene(string sceneName)
	{
		if (_activeSceneManager is null)
			throw new Exception("Scene manager has not set");
		return _activeSceneManager.LoadScene(sceneName);
	}
}