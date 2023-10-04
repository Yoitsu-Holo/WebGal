using SkiaSharp;

namespace WebGal.Services;

public class ResourceManager
{
	private readonly HttpClient _httpClient;

	private readonly Dictionary<string, SKBitmap> _imageList = new();
	private readonly Dictionary<string, object> _audioList = new();
	private readonly Dictionary<string, string> _scriptList = new();

	public ResourceManager(HttpClient httpClient)
	{
		// throw new NotImplementedException("Todo");
		_httpClient = httpClient;
	}

	public async Task PullImageAsync(string name, string path)
	{
		// _ = _httpClient.GetStreamAsync(path);
		using var stream = await _httpClient.GetStreamAsync(path);
		_imageList[name] = SKBitmap.Decode(stream);
	}

	public async Task PullAudioAsync(string name, string path)
	{
		throw new NotImplementedException("Todo");
		// using var stream = await _httpClient.GetStreamAsync(path);
		// _audioList[name] = new object();
		// return;
	}

	public async Task PullScriptAsync(string name = "main", string path = "/main.wb")
	{
		_scriptList[name] = await _httpClient.GetStringAsync(path);
	}

	public SKBitmap? GetImage(string name)
	{
		if (_imageList.ContainsKey(name))
			return _imageList[name];
		return null;
	}

	public object? GetAudio(string name)
	{
		if (_audioList.ContainsKey(name))
			return _audioList[name];
		return null;
	}

	public string? GetScript(string name = "main")
	{
		if (_scriptList.ContainsKey(name))
			return _scriptList[name];
		return null;
	}

	[Obsolete("Only use in Debug")]
	public async Task PushImageAsync(string name, SKBitmap image)
	{
		_imageList[name] = image;
	}

	[Obsolete("Only use in Debug")]
	public async Task PushAudioAsync(string name, object audio)
	{
		throw new NotImplementedException("Todo");
		// using var stream = await _httpClient.GetStreamAsync(path);
		// _audioList[name] = new object();
		// return;
	}

	[Obsolete("Only use in Debug")]
	public async Task PushScriptAsync(string name = "main", string script = "")
	{
		_scriptList[name] = script;
	}
}