using SkiaSharp;
using WebGal.Event;
using WebGal.Types;

namespace WebGal.Controller;

interface IController
{
	// 显示
	protected SKBitmap Draw();

	// 事件处理（）
	public void ProcessMouseEvent(MouseEvent mouseEvent);
	public void ProcessKeyboardEvent(KeyboardEvent keyboardEvent);

	// 大小处理、位置处理（左上角）
	public void SetSize(IVector size);
	public IVector GetSize();
	public void SetPostion(IVector postion);
	public IVector GetPositon();
	public IRect GetWindow();

	// 文本处理
	public void SetText(string s);
	public string GetText();
	public void SetTypeface(SKTypeface typeface);
	public SKTypeface GetTypeface();

	//设置名字
	public void SetName(string controllerName);
	public string GetName();

	// 可见性
	public void SetVisible(bool visible);
	public bool IsVisible();

	// 功能性
	public void SetEnable(bool enable);
	public bool IsEnable();
}