namespace WebGal.Event;

public class MouseEvent
{
	public void MousePress() { }            //当用户按下鼠标按钮时触发。
	public void MouseRelease() { }          //当用户释放鼠标按钮时触发。
	public void MouseClick() { }            //按下和释放鼠标按钮的组合动作。
	public void MouseDoubleCLick() { }      //用户在短时间内连续点击两次鼠标按钮时触发。
	public void MouseMove() { }             //当鼠标在窗口或控件内部移动时触发。
	public void MouseWheel() { }            //用户滚动鼠标滚轮时触发。
	public void MouseHover() { }            //当鼠标停留在控件上方一段时间时触发。
}