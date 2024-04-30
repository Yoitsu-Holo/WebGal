namespace WebGal.MeoInterpreter;

public partial class MoeInterpreter
{
	protected static readonly StringSplitOptions defaultStringSplitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
	protected static readonly ElfHeader _elfHeader = new();
	protected static readonly MoeRuntime _runtime = new();

#pragma warning disable CA2211
	protected static int _activeTask;
#pragma warning restore CA2211

	public static Dictionary<string, MoeVariable> GVariables => _runtime.Variables;
	public static Dictionary<int, Stack<MoeStackFrame>> Tasks => _runtime.Tasks;
	public static Stack<MoeStackFrame> ActiveTasks => Tasks[_activeTask];

	public static Dictionary<string, FuncntionNode> Functions => _elfHeader.Functions;
	public static FuncntionNode Start => Functions[_elfHeader.Start];
}