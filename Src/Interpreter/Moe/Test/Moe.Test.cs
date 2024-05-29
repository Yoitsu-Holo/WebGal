using WebGal.API;
using WebGal.API.Data;
using WebGal.Global;
using WebGal.Handler.Event;

namespace WebGal.MeoInterpreter;

public partial class MoeInterpreter
{
	public partial class Test
	{
		public static async Task GameTestAsync()
		{
			// 初始化游戏资源
			await GameStartAsync();
			await Driver.RegisterAudioContextAsync(new AudioIdInfo() { ContextID = 0, });
			await Driver.RegisterAudioNodeAsync(new AudioNodeInfo() { Request = RequestType.Set, ID = new() { ContextID = 0, NodeID = 0, }, Type = AudioNodeType.Simple, });
			await Driver.RegisterAudioNodeAsync(new AudioNodeInfo() { Request = RequestType.Set, ID = new() { ContextID = 0, NodeID = 1, }, Type = AudioNodeType.Simple, });
			await Driver.RegisterAudioNodeAsync(new AudioNodeInfo() { Request = RequestType.Set, ID = new() { ContextID = 0, NodeID = 2, }, Type = AudioNodeType.Simple, });

			// 注册界面
			FormRegister();

			// 测试脚本解析
			Tasks[_activeTask] = new();
			Call(Functions[_elfHeader.Start], []);

			// 加载场景
			ParseScene("sss");
			ParseScene("ch-1");
			SetSceneList("sss");
			LoadScene(0);

			// 注册事件
			Driver.RegisterLayoutAction(
				new() { LayoutID = 0, },
				(value) => { if (value is MouseEventData mouse && mouse.Status == MouseStatus.Up) { OnCLick(); } }
			);
		}
	}
}