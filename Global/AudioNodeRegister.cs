using System.Reflection;
using WebGal.Audio;

namespace WebGal.Global;

class AudioNodeRegister
{
	private static readonly Dictionary<string, Type> _types = [];

	static AudioNodeRegister()
	{
		var query = from type in Assembly.GetExecutingAssembly().GetTypes()
					where
						type.IsClass && type.Namespace?.StartsWith("WebGal.Audio") == true && typeof(IAudio).IsAssignableFrom(type)
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

	public static IAudio GetAudioNode(string className)
	{
		if (_types.TryGetValue(className, out Type? value))
			return Activator.CreateInstance(value) as IAudio ?? new AudioBase();
		return new AudioBase();
	}
}