using SkiaSharp;
using WebGal.Event;
using WebGal.Types;

namespace WebGal.Controller;

class ControllerBase : IController
{
	protected IVector _positon = new(0, 0);
	protected IVector _size = new IVector(0, 0);
	protected string _text = "ControllerBase";
	protected bool _enable = false;
	protected bool _visible = false;


	public virtual SKBitmap Draw() => throw new NotImplementedException();
	public virtual IVector GetPositon() => _positon;
	public IVector GetSize() => _size;
	public virtual string GetText() => _text;

	public (IVector, IVector) GetWindow()
	{
		throw new NotImplementedException();
	}

	public bool IsEnable() => _enable;
	public bool IsVisible() => _visible;


	public virtual void ProcessKeyboardEvent(MouseEvent mouseEvent) => throw new NotImplementedException();
	public virtual void ProcessMouseEvent(MouseEvent mouseEvent) => throw new NotImplementedException();
	public virtual void SetEnable(bool enable)
	{
		if (enable == true)
			throw new Exception("Can not enable ControllerBase object");
	}

	public virtual void SetPostion(IVector postion) => _positon = postion;

	public virtual void SetSize(IVector size) => _size = size;

	public virtual void SetText(string s) => _text = s;

	public virtual void SetVisible(bool visible) => _visible = visible;
}