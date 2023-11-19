using WebGal.Event;
using WebGal.Types;

namespace WebGal.Animations;

interface IController
{
	public LayerAtrribute GetAttr();
	public void ProcMouse(MouseEvent mouseEvent);
}