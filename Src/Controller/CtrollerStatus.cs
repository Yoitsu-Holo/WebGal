namespace WebGal.Controller;
public enum ControllerStatus : int
{
	Normal,
	Hover,
	Pressed,
	Focused,
	Disable, // 必须是最后一位
}