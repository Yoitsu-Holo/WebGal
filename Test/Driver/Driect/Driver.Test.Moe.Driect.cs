using WebGal.MeoInterpreter;

namespace WebGal.API;

public partial class Driver
{
	public partial class Test
	{
		public static async Task MoeTest()
		{
			await MoeInterpreter.GameTestAsync();
		}
	}
}