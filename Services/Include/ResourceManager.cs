using SkiaSharp;

namespace WebGal.Services.Include;

public class ResourceManager(HttpClient httpClient)
{
	private readonly HttpClient _httpClient = httpClient;
	private readonly Dictionary<string, SKBitmap> _imageList = [];
	private readonly Dictionary<string, byte[]> _audioList = [];
	private readonly Dictionary<string, byte[]> _blobList = [];
	private readonly Dictionary<string, SKTypeface> _fontList = [];
	private readonly Dictionary<string, string> _scriptList = [];


	public string BasePath { get; set; } = "/Demo/";

	public async Task PullImageAsync(string name, string path) => _imageList[name] = SKBitmap.Decode(await _httpClient.GetByteArrayAsync(BasePath + path));
	public async Task PullAudioAsync(string name, string path) => _audioList[name] = await _httpClient.GetByteArrayAsync(BasePath + path);
	public async Task PullBlobAsync(string name, string path) => _blobList[name] = await _httpClient.GetByteArrayAsync(BasePath + path);
	public async Task PullFontAsync(string name, string path) => _fontList[name] = SKTypeface.FromStream(await _httpClient.GetStreamAsync(BasePath + path));
	public async Task PullScriptAsync(string name = "main", string path = "main.json") => _scriptList[name] = await _httpClient.GetStringAsync(BasePath + path);


	public SKBitmap GetImage(string name) => _imageList.TryGetValue(name, out SKBitmap? value) ? value : throw new Exception($"Image \"{name}\" not find");
	public byte[] GetAudio(string name) => _audioList.TryGetValue(name, out byte[]? value) ? value : throw new Exception($"Audio \"{name}\" not find");
	public byte[] GetBlob(string name) => _blobList.TryGetValue(name, out byte[]? value) ? value : throw new Exception($"Blob \"{name}\" not find");
	public SKTypeface GetFont(string name) => _fontList.TryGetValue(name, out SKTypeface? value) ? value : throw new Exception($"Font \"{name}\" not find");
	public string GetScript(string name = "main") => _scriptList.TryGetValue(name, out string? value) ? value : throw new Exception($"Script \"{name}\" not find");




	public bool RemoveImage(string name) => _imageList.Remove(name);
	public bool RemoveAudio(string name) => _audioList.Remove(name);
	public bool RemoveBlob(string name) => _blobList.Remove(name);
	public bool RemoveFont(string name) => _fontList.Remove(name);
	public bool RemoveScript(string name) => _scriptList.Remove(name);

	public bool CheckImage(string name) => name is not null && _imageList.ContainsKey(name);
	public bool CheckAudio(string name) => name is not null && _audioList.ContainsKey(name);
	public bool CheckBlob(string name) => name is not null && _blobList.ContainsKey(name);
	public bool CheckFont(string name) => name is not null && _fontList.ContainsKey(name);
	public bool CheckScript(string name) => name is not null && _scriptList.ContainsKey(name);

	public void Clear()
	{
		_imageList.Clear();
		_audioList.Clear();
		_blobList.Clear();
		_fontList.Clear();
		_scriptList.Clear();
	}
}