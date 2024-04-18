using Microsoft.JSInterop;

namespace WebGal.API;


// 全局驱动程序，也是引擎暴露的接口
public partial class Driver
{
	[JSInvokable]
	public static void Help()
	{
	}
}