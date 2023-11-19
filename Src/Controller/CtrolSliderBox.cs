using WebGal.Event;
using WebGal.Types;

namespace WebGal.Animations;

class ControllerSliderBox : IController
{
	private int posX = 0, posY = 0;
	public LayerAtrribute GetAttr()
	{
		return new LayerAtrribute()
		{
			Offset = new(posX, posY),
			IsHide = false
		};
	}

	public void ProcMouse(MouseEvent mouseEvent)
	{
		(posX, posY) = mouseEvent.Position;
	}
}