namespace WebGal.Handler;

public interface IEvent
{
	public event EventHandler<EventArgs> EventOccurred; // 事件发生时的事件
	public void TriggerEvent(); // 触发事件的方法
};
