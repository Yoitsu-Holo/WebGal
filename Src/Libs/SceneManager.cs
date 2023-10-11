using WebGal.Libs.Base;

namespace WebGal.Libs;
/// <summary>
/// 简单的键值对映射 [string]name -> [Scene]scene
/// </summary>
public class SceneManager
{
	private readonly Dictionary<string, Scene> _scenes = new();

	public readonly Queue<string> SceneNameList = new();
	public bool ContainsScene(string sceneName) => _scenes.ContainsKey(sceneName);


	/// <summary>
	/// 放入一个场景
	/// </summary>
	/// <param name="sceneName"></param>
	/// <param name="scene"></param>
	public void PushScene(string sceneName, Scene scene) => _scenes[sceneName] = scene;

	/// <summary>
	/// 通过名字获取一个场景
	/// </summary>
	/// <param name="sceneName"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	public Scene LoadScene(string sceneName) => _scenes.ContainsKey(sceneName) ? _scenes[sceneName] : throw new Exception("LoadScene: no key");


	/// <summary>
	/// 通过场景名字删除场景
	/// </summary>
	/// <param name="sceneName"></param>
	public bool RemoveScene(string sceneName) => _scenes.ContainsKey(sceneName) ? _scenes.Remove(sceneName) : throw new Exception("RemoveScene: no key");

	public void Clear()
	{
		_scenes.Clear();
		SceneNameList.Clear();
	}
}