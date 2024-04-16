namespace WebGal.Handler.Event;

public class LogEventData(string message) : EventArgs
{
	public string Message { get; set; } = message;
}

public class LogEventHandler : IAction
{
	public void Action(object? sender, EventArgs eventArgs)
	{
		if (eventArgs is LogEventData eventData)
		{
			Console.WriteLine(eventData.Message);
		}
	}

	public void RegistEvent(IEvent e)
	{
		e.Subscribers += Action;
	}
}