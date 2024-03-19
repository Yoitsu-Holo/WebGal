using WebGal.Layer;

namespace WebGal.Global;

class ControllerRegister
{
	public static ILayer GetController(string ControllerName) => ControllerName switch
	{
		"brownian" => new ControllerSliderHorizontal(),
		_ => new LayerBase(),
	};
}