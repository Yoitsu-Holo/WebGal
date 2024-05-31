
namespace WebGal.Handler;

public interface IHandler : IAction, IEvent { }

public class HandlerBase : IHandler
{
	private readonly ActionBase _actionBase = new();
	private readonly EventBase _eventBase = new();

	public virtual bool DoAction(EventArgs eventArgs) => _actionBase.DoAction(eventArgs);

	public virtual void ClearSubscribers() => _eventBase.ClearSubscribers();
	public virtual void RegisterAction(IAction action) => _eventBase.RegisterAction(action);
	public virtual bool TriggerEvent(EventArgs args) => _eventBase.TriggerEvent(args);
}