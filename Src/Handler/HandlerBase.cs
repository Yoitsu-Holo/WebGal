
namespace WebGal.Handler;

public class HandlerBase : IHandler
{
	private Action<EventArgs>? _myAction;
	public event EventHandler<EventArgs>? Subscribers;

	public void Action(object? sender, EventArgs eventArgs) => _myAction?.Invoke(eventArgs);

	public void RegistEvent(IEvent e) => e.Subscribers += Action;

	public void TriggerEvent(EventArgs args)
	{
		if (Subscribers is not null)
			Subscribers(this, args);
	}

	public void SetAction(Action<EventArgs> myAction) => _myAction = myAction;
}