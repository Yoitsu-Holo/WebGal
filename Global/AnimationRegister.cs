using System.Reflection;
using WebGal.Animations;

namespace WebGal.Global;

class AnimationRegister
{
	private static readonly Dictionary<string, Type> _types = [];

	static AnimationRegister()
	{
		var query = from type in Assembly.GetExecutingAssembly().GetTypes()
					where type.IsClass && type.Namespace == "WebGal.Animations" && typeof(IAnimation).IsAssignableFrom(type)
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

	public static IAnimation GetAnimation(string className)
	{
		if (_types.TryGetValue(className, out Type? value))
			return Activator.CreateInstance(value) as IAnimation ?? new AnimationNothing();
		return new AnimationNothing();
	}

	public static Type GetType(string className)
	{
		if (_types.TryGetValue(className, out Type? value))
			return value;
		return typeof(object);
	}
}