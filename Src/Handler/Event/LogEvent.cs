namespace WebGal.Handler.Event;

public class LogEventHandler : IAction
{
	public bool ActionStatus { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

	public void Action(object? sender, EventArgs eventArgs)
	{
		Console.WriteLine(eventArgs.ToString());
	}

	public void RegistEvent(IEvent e)
	{
		e.Subscribers += Action;
	}
}