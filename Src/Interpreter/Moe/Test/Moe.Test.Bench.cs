using WebGal.API;
using WebGal.API.Data;
using WebGal.Global;
using WebGal.Handler.Event;

namespace WebGal.MeoInterpreter;

public partial class MoeInterpreter
{
	public partial class Test
	{
		public static async Task Benchmark()
		{
			{
				Logger.LogInfo("Call Banch (10 * 100K):", Global.LogLevel.Todo);
				long tot = 0;
				long min = long.MaxValue;
				long max = long.MinValue;

				for (int cnt = 0; cnt < 10; cnt++)
				{
					long start = NowTime.Minisecond;

					for (int i = 0; i < 100000; i++)
						Driver.Empty();

					long end = NowTime.Minisecond;
					long delta = end - start;
					tot += delta;
					min = Math.Min(min, delta);
					max = Math.Max(max, delta);

					Logger.LogInfo($"Start: {start}\tEnd: {end}\tDt: {delta}");
				}

				Logger.LogInfo($"Avg: {tot / 10} Min: {min} Max: {max}");
			}

			{
				Logger.LogInfo("Async Call Banch (10 * 100K):", Global.LogLevel.Todo);
				long tot = 0;
				long min = long.MaxValue;
				long max = long.MinValue;
				List<Task> tasks = [];

				for (int cnt = 0; cnt < 10; cnt++)
				{
					tasks.Clear();
					long start = NowTime.Minisecond;
					for (int i = 0; i < 100000; i++)
						tasks.Add(Driver.EmptyAsync());

					await Task.WhenAll(tasks);

					long end = NowTime.Minisecond;
					long delta = end - start;
					tot += delta;
					min = Math.Min(min, delta);
					max = Math.Max(max, delta);

					Logger.LogInfo($"Start: {start}\tEnd: {end}\tDt: {delta}");
				}

				Logger.LogInfo($"Avg: {tot / 10} Min: {min} Max: {max}");
			}
		}

		public static async Task RenderBench()
		{
			await Task.Run(() => { });
			{
				Logger.LogInfo("Render Test (10):", Global.LogLevel.Todo);

				Driver.RegisterLayout(new LayerIdInfo() { LayoutID = 0, });
				Driver.SetActiveLayout(new LayerIdInfo() { LayoutID = 0, });

				for (int cnt = 0; cnt < 100; cnt++)
				{
					Driver.RegisterLayer(new LayerBox()
					{
						Attribute = new()
						{
							ID = new() { LayoutID = 0, LayerID = cnt, },
							Position = new(5 * cnt, 5 * cnt),
							Size = new(1280, 720),
							Type = "WidgetColorBox",
						}
					});
					Driver.SetColorBoxInfo(new ColorBoxInfo()
					{
						ID = new() { LayoutID = 0, LayerID = cnt, },
						A = 5,
						G = 50,
					});
				}
			}
		}

		public static async Task ABTest()
		{
			Tasks[_activeTask] = new();
			GVariables["a"] = 1;
			GVariables["b"] = 2;
			GVariables["c"] = 3;
			GVariables["a"].Access = MoeVariableAccess.Variable;
			GVariables["b"].Access = MoeVariableAccess.Variable;
			GVariables["c"].Access = MoeVariableAccess.Variable;
			await Task.Run(() => { });
			{
				Logger.LogInfo("A+B Problem (10 * 100K):", Global.LogLevel.Todo);
				long tot = 0;
				long min = long.MaxValue;
				long max = long.MinValue;
				var func = _elfHeader.Functions["test"];

				for (int cnt = 0; cnt < 10; cnt++)
				{
					long start = NowTime.Minisecond;

					for (int i = 0; i < 100000; i++)
						Call(func, []);

					long end = NowTime.Minisecond;
					long delta = end - start;
					tot += delta;
					min = Math.Min(min, delta);
					max = Math.Max(max, delta);

					Logger.LogInfo($"Start: {start}\tEnd: {end}\tDt: {delta}");
				}

				Logger.LogInfo($"Avg: {tot / 10} Min: {min} Max: {max}");
			}
		}
	}
}