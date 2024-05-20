
namespace WebGal.Handler;

public class HandlerBase : IHandler
{
	public virtual bool ActionStatus { get; set; }

	public virtual Action<EventArgs>? HandlerAction { get; set; }

	public virtual event EventHandler<EventArgs>? Subscribers;

	public virtual void Action(object? sender, EventArgs eventArgs)
	{
		ActionStatus = true;
		if (HandlerAction is null) return;
		HandlerAction.Invoke(eventArgs);
	}


	public virtual void RegistEvent(IEvent e) => e.Subscribers += Action;

	public virtual void TriggerEvent(EventArgs args)
	{
		if (Subscribers is null) return;
		Subscribers(this, args);
	}
}