namespace WebGal.Handler.Event;

public class LogEventHandler : IAction
{
	public void Action(object? sender, EventArgs eventArgs)
	{
		Console.WriteLine(eventArgs.ToString());
	}

	public void RegistEvent(IEvent e)
	{
		e.Subscribers += Action;
	}
}