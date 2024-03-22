using WebGal.Layer;

namespace WebGal.Libs.Base;

/// <summary>
/// 特化的 [Layer] 数组，只用于表示 ADV 剧情
/// </summary>
public class Scene
{
	public SortedDictionary<int, ILayer> Layers { get; set; } = [];
	public void Clear() => Layers.Clear();
}

