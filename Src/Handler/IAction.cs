namespace WebGal.Handler;

/// <summary>
/// 对于一个可以响应事件的类，必须继承该接口。
/// 注册事件时必须将事件注册到对应类的该接口上。
/// </summary>
public interface IAction
{
	public bool DoAction(EventArgs eventArgs); // 执行动作的方法
}

public class ActionBase : IAction
{
	public Func<EventArgs, bool>? Action;
	public virtual bool DoAction(EventArgs eventArgs)
	{
		if (Action == null)
			return false;
		return Action(eventArgs);
	}
}