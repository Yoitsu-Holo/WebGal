using System.Reflection;
using WebGal.Layer;

namespace WebGal.Global;

class LayerBoxRegister
{
	private static readonly Dictionary<string, Type> _types = [];

	static LayerBoxRegister()
	{
		var query = from type in Assembly.GetExecutingAssembly().GetTypes()
					where type.IsClass && type.Namespace == "WebGal.Layer" && typeof(ILayer).IsAssignableFrom(type)
					select type;

		foreach (var type in query)
			_types.Add(type.Name, type);
	}

	public static void Dump()
	{
		foreach (var (_, type) in _types)
			Console.WriteLine(type.FullName);
	}

	public static void Clear() => _types.Clear();

	public static ILayer GetLayerBox(string className)
	{
		if (_types.TryGetValue(className, out Type? value))
			return Activator.CreateInstance(value) as ILayer ?? new LayerBase();
		return new LayerBase();
	}
}