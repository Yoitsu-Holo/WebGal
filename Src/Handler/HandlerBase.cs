namespace WebGal.Handler;

public interface IHandler : IAction, IEvent { }

public class HandlerBase : IHandler
{
	private readonly ActionBase _actionBase = new();
	private readonly EventBase _eventBase = new();

	public bool ActionStatus { get => _actionBase.ActionStatus; set => _actionBase.ActionStatus = value; }
	public event EventHandler<EventArgs>? Subscribers { add => _eventBase.Subscribers += value; remove => _eventBase.Subscribers -= value; }

	public virtual void DoAction(object? sender, EventArgs eventArgs) => _actionBase.DoAction(sender, eventArgs);
	public virtual void RegisterAction(Action<EventArgs> action) => _actionBase.RegisterAction(action);
	public virtual void RegisterEvent(IEvent e) => _actionBase.RegisterEvent(e);

	public virtual void TriggerEvent(EventArgs args) => _eventBase.TriggerEvent(args);
}