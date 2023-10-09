using SkiaSharp;

namespace WebGal.Libs;

public class ResourceManager
{
	private readonly HttpClient _httpClient;
	private readonly Dictionary<string, SKBitmap> _imageList = new();
	private readonly Dictionary<string, byte[]> _audioList = new();
	private readonly Dictionary<string, string> _scriptList = new();
	public string basePath { get; set; } = "/Demo/";

	public ResourceManager(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	/// <summary>
	/// 传入名字和路径，获取对应图片
	/// </summary>
	/// <param name="name"></param>
	/// <param name="path"></param>
	/// <returns></returns>
	public async Task PullImageAsync(string name, string path)
	{
		using var stream = await _httpClient.GetStreamAsync(basePath + path);
		_imageList[name] = SKBitmap.Decode(stream);
	}

	/// <summary>
	/// 传入名字和路径，获取对应音频
	/// </summary>
	/// <param name="name"></param>
	/// <param name="path"></param>
	/// <returns></returns>
	public async Task PullAudioAsync(string name, string path)
	{
		_audioList[name] = await _httpClient.GetByteArrayAsync(basePath + path);
	}

	/// <summary>
	/// 传入名字和路径，获取对应脚本
	/// </summary>
	/// <param name="name"></param>
	/// <param name="path"></param>
	/// <returns></returns>
	public async Task PullScriptAsync(string name = "main", string path = "/Demo/")
	{
		_scriptList[name] = await _httpClient.GetStringAsync(basePath + path);
	}

	/// <summary>
	/// 使用名字获取一个图片
	/// </summary>
	/// <param name="name"></param>
	/// <returns>SKBitmap图片</returns>
	/// <exception cref="Exception"></exception>
	public SKBitmap GetImage(string name)
	{
		if (_imageList.ContainsKey(name))
			return _imageList[name];
		throw new Exception($"Image \"{name}\" not find");
	}

	/// <summary>
	/// 使用名字获取一个音频
	/// </summary>
	/// <param name="name"></param>
	/// <returns>音频字节数组</returns>
	/// <exception cref="Exception"></exception>
	public byte[] GetAudio(string name)
	{
		if (_audioList.ContainsKey(name))
			return _audioList[name];
		throw new Exception($"Audio \"{name}\" not find");
	}

	/// <summary>
	/// 使用名字获取一个脚本
	/// </summary>
	/// <param name="name"></param>
	/// <returns>代码文本</returns>
	/// <exception cref="Exception"></exception>
	public string GetScript(string name = "main")
	{
		if (_scriptList.ContainsKey(name))
			return _scriptList[name];
		throw new Exception($"Script \"{name}\" not find");
	}

	public void Clear()
	{
		_imageList.Clear();
		_audioList.Clear();
		_scriptList.Clear();
	}


	#region Debug
	[Obsolete("Debug Only")] public void PushImageAsync(string name, SKBitmap image) => _imageList[name] = image;
	[Obsolete("Debug Only")] public void PushAudioAsync(string name, byte[] audio) => _audioList[name] = audio;
	[Obsolete("Debug Only")] public void PushScriptAsync(string name = "main", string script = "") => _scriptList[name] = script;
	#endregion
}