using SkiaSharp;
using WebGal.Event;
using WebGal.Types;

namespace WebGal.Animations;

interface IController
{
	public SKBitmap Draw();
	public void ProcessMouseEvent(MouseEvent mouseEvent);
	public void ProcessKeyboardEvent(MouseEvent mouseEvent);

	public void SetPostion(IVector postion);
	public IVector GetPositon();

	public void SetText(string s);
	public string GetText();


	public void SetSize(IVector size);
	public IVector GetSize();

	public void SetVisible(bool visible);
	public bool IsVisible();

	public void SetEnable(bool enable);
	public bool IsEnable();
}