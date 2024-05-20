namespace WebGal.Handler;

public interface IHandler : IEvent, IAction
{
	public Action<EventArgs>? HandlerAction { get; set; }
}