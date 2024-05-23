namespace WebGal.Layer;

public enum LayerStatus : int
{
	Normal,     // 通常状态
	Hover,      // 悬停状态
	Pressed,    // 按下状态
	Focused,    // 聚焦状态 (突出显示)
	Disable,    // 显示，但是无法操作
	Unvisable,  // 必须最后一位，不会输出任何图像
}