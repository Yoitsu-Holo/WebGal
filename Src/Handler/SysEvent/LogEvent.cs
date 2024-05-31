namespace WebGal.Handler.Event;

public class LogEventHandler : ActionBase
{
	public override bool DoAction(EventArgs eventArgs)
	{
		Console.WriteLine(eventArgs.ToString());
		return false;
	}
}