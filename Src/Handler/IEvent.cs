namespace WebGal.Handler;

public interface IEvent
{
	public bool TriggerEvent(EventArgs args); // 触发事件的方法
	public void ClearSubscribers(); //清除订阅
	public void RegisterAction(IAction action);
}

public class EventBase : IEvent
{
	protected List<Func<EventArgs, bool>> _subscribers = []; // 订阅者

	public void ClearSubscribers() => _subscribers.Clear();

	public bool TriggerEvent(EventArgs args)
	{
		if (_subscribers is null) return false;

		bool result = false;

		foreach (var subscriber in _subscribers)
			result |= subscriber.Invoke(args);

		return result;
	}

	public void RegisterAction(IAction action) => _subscribers.Add(action.DoAction);
}