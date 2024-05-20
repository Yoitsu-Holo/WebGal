namespace WebGal.Handler;

public class HandlerBase : IHandler
{
	public virtual Action<EventArgs>? HandlerAction { get; set; }


	public virtual bool ActionStatus { get; set; }

	public virtual void RegistEvent(IEvent e) => e.Subscribers += Action;

	public virtual void Action(object? sender, EventArgs eventArgs)
	{
		ActionStatus = false;
		HandlerAction?.Invoke(eventArgs);
		TriggerEvent(eventArgs);
		ActionStatus = true;
	}


	public event EventHandler<EventArgs>? Subscribers;

	public virtual void TriggerEvent(EventArgs args)
	{
		if (Subscribers is null) return;
		Subscribers(this, args);
	}
}