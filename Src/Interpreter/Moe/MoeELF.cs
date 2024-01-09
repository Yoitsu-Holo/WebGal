namespace WebGal.MeoInterpreter;

public class ElfHeader
{
	// .file 
	public Dictionary<string, MoeFile> MoeFiles = [];
	// .table [Auto]
	public Dictionary<string, MoeFunction> MoeFunctions = [];
	// .data
	public Dictionary<string, MoeVariable> MoeData = [];
	// .form
	public Dictionary<string, string> MoeFrom = [];
	// .start
	public MoeFunction MoeStart = new();
}