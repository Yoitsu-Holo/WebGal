using WebGal.Controller;

namespace WebGal.Global;

class ControllerRegister
{
	public static IController GetController(string ControllerName) => ControllerName switch
	{
		"brownian" => new ControllerSliderBox(),
		_ => new ControllerBase(),
	};
}