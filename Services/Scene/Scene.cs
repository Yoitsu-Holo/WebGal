using SkiaSharp;
using WebGal.Global;

namespace WebGal.Services.Module;

/// <summary>
/// 一系列场景数据，包含多个图层。
/// 被 Scene Manager 设置，被 Render 使用。
/// </summary>
public class Scene
{
	public SKSizeI Resolution { set; get; } = new(
		SceneConfig.DefualtWidth,
		SceneConfig.DefualtHeight
	);

	public SortedDictionary<int, Layer> Layers = new();
	private readonly Dictionary<string, int> _layersId = new();
	private readonly Dictionary<int, string> _layersName = new();
	private int _layerCount = 0;

	public void PushLayer(string name, Layer layer)
	{
		_layersId[name] = _layerCount;
		_layersName[_layerCount] = name;
		Layers[_layerCount] = layer;

		_layerCount++;
	}

	public void Clear()
	{
		Layers.Clear();
		_layersName.Clear();
		_layersId.Clear();
		_layerCount = 0;
	}
}