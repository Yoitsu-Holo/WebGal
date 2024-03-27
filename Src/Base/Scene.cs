using WebGal.Layer;

namespace WebGal.Libs.Base;

/// <summary>
/// 场景，由多个Layer构成，每一个Layer可以是图片、色块、文本框等基本组件，也可以是控制组件。
/// 这里只作为数据存储
/// </summary>
public class Scene
{
	public SortedDictionary<int, ILayer> Layers { get; set; } = [];
	public void Clear() => Layers.Clear();
}

