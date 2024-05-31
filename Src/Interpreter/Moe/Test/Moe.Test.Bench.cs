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
				Logger.LogInfo("Call Banch (10 * 1M):", Global.LogLevel.Todo);
				long tot = 0;
				long min = long.MaxValue;
				long max = long.MinValue;

				for (int cnt = 0; cnt < 10; cnt++)
				{
					long start = NowTime.Minisecond;

					for (int i = 0; i < 1000000; i++)
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

		public static async Task ScriptBench()
		{
			GVariables["a"] = 1;
			GVariables["b"] = 2;
			GVariables["c"] = 0;
			GVariables["a"].Access = MoeVariableAccess.Variable;
			GVariables["b"].Access = MoeVariableAccess.Variable;
			GVariables["c"].Access = MoeVariableAccess.Variable;

			await Task.Run(() => { });
			#region Func Call Test
			{
				Tasks[_activeTask] = new();
				Logger.LogInfo("Func Call Test (10 * 100K):", Global.LogLevel.Todo);
				long tot = 0;
				long min = long.MaxValue;
				long max = long.MinValue;
				var func = _elfHeader.Functions["CALLtest"];

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
			#endregion

			#region A+B Problem
			{
				Tasks[_activeTask] = new();
				Logger.LogInfo("A+B Problem (10 * 100K):", Global.LogLevel.Todo);
				long tot = 0;
				long min = long.MaxValue;
				long max = long.MinValue;
				var func = _elfHeader.Functions["ABtest"];

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
			#endregion

			#region IF Test
			{
				Tasks[_activeTask] = new();
				Logger.LogInfo("IF Test (10 * 100K):", Global.LogLevel.Todo);
				long tot = 0;
				long min = long.MaxValue;
				long max = long.MinValue;
				var func = _elfHeader.Functions["IFtest"];

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
			#endregion

			#region ELSE Test
			{
				Tasks[_activeTask] = new();
				Logger.LogInfo("ELSE Test (10 * 100K):", Global.LogLevel.Todo);
				long tot = 0;
				long min = long.MaxValue;
				long max = long.MinValue;
				var func = _elfHeader.Functions["ELSEtest"];

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
			#endregion

			#region WHILE Test
			{
				Tasks[_activeTask] = new();
				Logger.LogInfo("WHILE Test (10 * 100K):", Global.LogLevel.Todo);
				long tot = 0;
				long min = long.MaxValue;
				long max = long.MinValue;
				var func = _elfHeader.Functions["WHILEtest"];

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
			#endregion
		}
	}
}