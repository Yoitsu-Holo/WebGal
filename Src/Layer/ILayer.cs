using SkiaSharp;
using WebGal.Event;
using WebGal.Types;

namespace WebGal.Layer;

interface ILayer
{
	// 渲染到指定canvas
	void Render(SKCanvas canvas);

	// 事件处理（）
	void ProcessMouseEvent(MouseEvent mouseEvent);
	void ProcessKeyboardEvent(KeyboardEvent keyboardEvent);

	// 大小处理、位置处理（左上角）
	void SetSize(IVector size);
	IVector GetSize();
	void SetPostion(IVector postion);
	IVector GetPositon();
	IRect GetWindow();

	// 文本处理
	void SetText(string s);
	string GetText();
	void SetTypeface(SKTypeface typeface);
	SKTypeface GetTypeface();

	// 动画处理
	// todo

	//设置名字
	void SetName(string controllerName);
	string GetName();

	// 可见性
	void SetVisible(bool visible);
	bool IsVisible();

	// 功能性
	void SetEnable(bool enable);
	public bool IsEnable();

	// 值处理
	void SetValue(float value);
	float GetValue();
}