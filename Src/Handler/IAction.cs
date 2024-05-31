namespace WebGal.Handler;

/// <summary>
/// 对于一个可以响应事件的类，必须继承该接口。
/// 注册事件时必须将事件注册到对应类的该接口上。
/// </summary>
public interface IAction
{
	public bool ActionStatus { get; set; }
	public void RegisterEvent(IEvent e);  // 注册事件
	public void RegisterAction(Action<EventArgs> action);
	public void DoAction(object? sender, EventArgs eventArgs); // 执行动作的方法
}

public class ActionBase : IAction
{
	public bool ActionStatus { get; set; }
	private HashSet<Action<EventArgs>> _actions = [];

	public virtual void DoAction(object? sender, EventArgs eventArgs)
	{
		ActionStatus = false;
		if (_actions.Count == 0)
			return;
		foreach (var action in _actions)
			action.Invoke(eventArgs);
		ActionStatus = true;
	}

	public virtual void RegisterEvent(IEvent eventSender) => eventSender.Subscribers += DoAction;
	public virtual void RegisterAction(Action<EventArgs> action) => _actions.Add(action);
}