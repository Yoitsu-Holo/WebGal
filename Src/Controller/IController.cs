using SkiaSharp;
using WebGal.Event;
using WebGal.Types;

namespace WebGal.Controller;

interface IController
{
	// 显示
	public SKBitmap Draw();

	// 事件处理（）
	public void ProcessMouseEvent(MouseEvent mouseEvent);
	public void ProcessKeyboardEvent(MouseEvent mouseEvent);

	// 位置处理（左上角）
	public void SetPostion(IVector postion);
	public IVector GetPositon();
	public (IVector, IVector) GetWindow();

	// 文本处理
	public void SetText(string s);
	public string GetText();

	// 大小处理
	public void SetSize(IVector size);
	public IVector GetSize();

	// 可见性
	public void SetVisible(bool visible);
	public bool IsVisible();


	// 功能性
	public void SetEnable(bool enable);
	public bool IsEnable();
}