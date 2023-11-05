using SkiaSharp;

namespace WebGal.Libs;
public class ResourceManager
{
	private readonly HttpClient _httpClient;
	private readonly Dictionary<string, SKBitmap> _imageList = new();
	private readonly Dictionary<string, byte[]> _audioList = new();
	private readonly Dictionary<string, SKTypeface> _fontList = new();
	private readonly Dictionary<string, string> _scriptList = new();
	public string basePath { get; set; } = "/Demo/";

	public ResourceManager(HttpClient httpClient) => _httpClient = httpClient;


	public async Task PullImageAsync(string name, string path) => _imageList[name] = SKBitmap.Decode(await _httpClient.GetByteArrayAsync(basePath + path));
	public async Task PullAudioAsync(string name, string path) => _audioList[name] = await _httpClient.GetByteArrayAsync(basePath + path);
	public async Task PullFontAsync(string name, string path) => _fontList[name] = SKTypeface.FromStream(await _httpClient.GetStreamAsync(basePath + path));
	public async Task PullScriptAsync(string name = "main", string path = "main.json") => _scriptList[name] = await _httpClient.GetStringAsync(basePath + path);


	public SKBitmap GetImage(string name) => _imageList.ContainsKey(name) ? _imageList[name] : throw new Exception($"Image \"{name}\" not find");
	public byte[] GetAudio(string name) => _audioList.ContainsKey(name) ? _audioList[name] : throw new Exception($"Audio \"{name}\" not find");
	public SKTypeface GetFont(string name) => _fontList.ContainsKey(name) ? _fontList[name] : throw new Exception($"Font \"{name}\" not find");
	public string GetScript(string name = "main") => _scriptList.ContainsKey(name) ? _scriptList[name] : throw new Exception($"Script \"{name}\" not find");

	public bool RemoveImage(string name) => _imageList.Remove(name);
	public bool RemoveAudio(string name) => _audioList.Remove(name);
	public bool RemoveFont(string name) => _fontList.Remove(name);
	public bool RemoveScript(string name) => _scriptList.Remove(name);

	public void Clear()
	{
		_imageList.Clear();
		_audioList.Clear();
		_fontList.Clear();
		_scriptList.Clear();
	}
}