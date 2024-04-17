namespace WebGal.Handler;

/// <summary>
/// 对于一个可以发出发出事件的类，必须继承此接口
/// </summary>
public interface IEvent
{
	public event EventHandler<EventArgs> Subscribers; // 订阅者
	public void TriggerEvent(EventArgs args); // 触发事件的方法
}