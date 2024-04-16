namespace WebGal.Handler;

public interface IEvent
{
	public event EventHandler<EventArgs> Subscribers; // 订阅者
	public void TriggerEvent(EventArgs args); // 触发事件的方法
}