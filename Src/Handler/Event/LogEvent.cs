namespace WebGal.Handler.Event;

public class LogEventHandler : ActionBase
{
	public override void DoAction(object? sender, EventArgs eventArgs) => Console.WriteLine(eventArgs.ToString());
}