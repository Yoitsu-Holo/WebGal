namespace WebGal.MeoInterpreter;

public partial class MoeInterpreter
{
	/// <summary>
	/// 主解释器，所有代码的解释执行都在此
	/// </summary>
	private void MainInterperter()
	{
		while (true)
		{
			_activeTask = _globleSpace.Task[_activeTaskName];
		}
	}
}