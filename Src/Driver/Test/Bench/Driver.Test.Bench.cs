using WebGal.API.Data;
using WebGal.Global;

namespace WebGal.API;

public partial class Driver
{
	public partial class Test
	{
		public static async Task RenderBench()
		{
			if (_layoutManager is null || _resourceManager is null)
				return;

			// #region ColorBox Render Test (1)
			// {
			// 	Logger.LogInfo("ColorBox Render Test (1):", Global.LogLevel.Todo);

			// 	RegisterLayout(new LayerIdInfo() { LayoutID = 0, });
			// 	SetActiveLayout(new LayerIdInfo() { LayoutID = 0, });

			// 	for (int cnt = 0; cnt < 1; cnt++)
			// 	{
			// 		RegisterLayer(new LayerBox()
			// 		{
			// 			Attribute = new()
			// 			{
			// 				ID = new() { LayoutID = 0, LayerID = cnt, },
			// 				Position = new(5 * cnt, 5 * cnt),
			// 				Size = new(1280, 720),
			// 				Type = "WidgetColorBox",
			// 			}
			// 		});
			// 		SetColorBoxInfo(new ColorBoxInfo()
			// 		{
			// 			ID = new() { LayoutID = 0, LayerID = cnt, },
			// 			A = 5,
			// 			G = 50,
			// 		});
			// 	}

			// 	//跟踪 10s 的帧率
			// 	RenderInfo.SatrtRecord();
			// 	await new PeriodicTimer(TimeSpan.FromMilliseconds(10000)).WaitForNextTickAsync();
			// 	RenderInfo.SatrtRecord(false);
			// 	_layoutManager.Clear();

			// 	Logger.LogInfo($"{string.Join(",", RenderInfo.GetRecord())}");

			// 	RenderInfo.Clear();
			// }
			// #endregion

			// #region ColorBox Render Test (10)
			// {
			// 	Logger.LogInfo("ColorBox Render Test (10):", Global.LogLevel.Todo);

			// 	RegisterLayout(new LayerIdInfo() { LayoutID = 0, });
			// 	SetActiveLayout(new LayerIdInfo() { LayoutID = 0, });

			// 	for (int cnt = 0; cnt < 10; cnt++)
			// 	{
			// 		RegisterLayer(new LayerBox()
			// 		{
			// 			Attribute = new()
			// 			{
			// 				ID = new() { LayoutID = 0, LayerID = cnt, },
			// 				Position = new(5 * cnt, 5 * cnt),
			// 				Size = new(1280, 720),
			// 				Type = "WidgetColorBox",
			// 			}
			// 		});
			// 		SetColorBoxInfo(new ColorBoxInfo()
			// 		{
			// 			ID = new() { LayoutID = 0, LayerID = cnt, },
			// 			A = 5,
			// 			G = 50,
			// 		});
			// 	}

			// 	//跟踪 10s 的帧率
			// 	RenderInfo.SatrtRecord();
			// 	await new PeriodicTimer(TimeSpan.FromMilliseconds(10000)).WaitForNextTickAsync();
			// 	RenderInfo.SatrtRecord(false);
			// 	_layoutManager.Clear();

			// 	Logger.LogInfo($"{string.Join(",", RenderInfo.GetRecord())}");

			// 	RenderInfo.Clear();
			// }
			// #endregion

			// #region ColorBox Render Test (50)
			// {
			// 	Logger.LogInfo("ColorBox Render Test (50):", Global.LogLevel.Todo);

			// 	RegisterLayout(new LayerIdInfo() { LayoutID = 0, });
			// 	SetActiveLayout(new LayerIdInfo() { LayoutID = 0, });

			// 	for (int cnt = 0; cnt < 50; cnt++)
			// 	{
			// 		RegisterLayer(new LayerBox()
			// 		{
			// 			Attribute = new()
			// 			{
			// 				ID = new() { LayoutID = 0, LayerID = cnt, },
			// 				Position = new(5 * cnt, 5 * cnt),
			// 				Size = new(1280, 720),
			// 				Type = "WidgetColorBox",
			// 			}
			// 		});
			// 		SetColorBoxInfo(new ColorBoxInfo()
			// 		{
			// 			ID = new() { LayoutID = 0, LayerID = cnt, },
			// 			A = 5,
			// 			G = 50,
			// 		});
			// 	}

			// 	//跟踪 10s 的帧率
			// 	RenderInfo.SatrtRecord();
			// 	await new PeriodicTimer(TimeSpan.FromMilliseconds(10000)).WaitForNextTickAsync();
			// 	RenderInfo.SatrtRecord(false);
			// 	_layoutManager.Clear();

			// 	Logger.LogInfo($"{string.Join(",", RenderInfo.GetRecord())}");

			// 	RenderInfo.Clear();
			// }
			// #endregion

			// #region ColorBox Render Test (100)
			// {
			// 	Logger.LogInfo("Render Color Test (100):", Global.LogLevel.Todo);

			// 	RegisterLayout(new LayerIdInfo() { LayoutID = 0, });
			// 	SetActiveLayout(new LayerIdInfo() { LayoutID = 0, });

			// 	for (int cnt = 0; cnt < 100; cnt++)
			// 	{
			// 		RegisterLayer(new LayerBox()
			// 		{
			// 			Attribute = new()
			// 			{
			// 				ID = new() { LayoutID = 0, LayerID = cnt, },
			// 				Position = new(5 * cnt, 5 * cnt),
			// 				Size = new(1280, 720),
			// 				Type = "WidgetColorBox",
			// 			}
			// 		});
			// 		SetColorBoxInfo(new ColorBoxInfo()
			// 		{
			// 			ID = new() { LayoutID = 0, LayerID = cnt, },
			// 			A = 5,
			// 			G = 50,
			// 		});
			// 	}

			// 	//跟踪 10s 的帧率
			// 	RenderInfo.SatrtRecord();
			// 	await new PeriodicTimer(TimeSpan.FromMilliseconds(10000)).WaitForNextTickAsync();
			// 	RenderInfo.SatrtRecord(false);
			// 	_layoutManager.Clear();

			// 	Logger.LogInfo($"{string.Join(",", RenderInfo.GetRecord())}");

			// 	RenderInfo.Clear();
			// }
			// #endregion

			// await _resourceManager.PullImageAsync("test", "/Image/bg010a.png");

			// #region ImageBox Render Test (1)
			// {
			// 	Logger.LogInfo("ImageBox Render Test (1):", Global.LogLevel.Todo);

			// 	RegisterLayout(new LayerIdInfo() { LayoutID = 0, });
			// 	SetActiveLayout(new LayerIdInfo() { LayoutID = 0, });

			// 	for (int cnt = 0; cnt < 1; cnt++)
			// 	{
			// 		RegisterLayer(new LayerBox()
			// 		{
			// 			Attribute = new()
			// 			{
			// 				ID = new() { LayoutID = 0, LayerID = cnt, },
			// 				Position = new(5 * cnt, 5 * cnt),
			// 				Size = new(1280, 720),
			// 				Type = "WidgetImageBox",
			// 			}
			// 		});
			// 		SetImageBoxInfo(new ImageBoxInfo()
			// 		{
			// 			ID = new() { LayoutID = 0, LayerID = cnt, },
			// 			Image = new() { ImageName = "test", SubRect = new(), },
			// 		});
			// 	}

			// 	await new PeriodicTimer(TimeSpan.FromMilliseconds(30000)).WaitForNextTickAsync();
			// 	//跟踪 10s 的帧率
			// 	RenderInfo.SatrtRecord();
			// 	await new PeriodicTimer(TimeSpan.FromMilliseconds(10000)).WaitForNextTickAsync();
			// 	RenderInfo.SatrtRecord(false);
			// 	_layoutManager.Clear();

			// 	Logger.LogInfo($"{string.Join(",", RenderInfo.GetRecord())}");

			// 	RenderInfo.Clear();
			// }
			// #endregion

			// #region ImageBox Render Test (5)
			// {
			// 	Logger.LogInfo("ImageBox Render Test (5):", Global.LogLevel.Todo);

			// 	RegisterLayout(new LayerIdInfo() { LayoutID = 0, });
			// 	SetActiveLayout(new LayerIdInfo() { LayoutID = 0, });

			// 	for (int cnt = 0; cnt < 5; cnt++)
			// 	{
			// 		RegisterLayer(new LayerBox()
			// 		{
			// 			Attribute = new()
			// 			{
			// 				ID = new() { LayoutID = 0, LayerID = cnt, },
			// 				Position = new(5 * cnt, 5 * cnt),
			// 				Size = new(1280, 720),
			// 				Type = "WidgetImageBox",
			// 			}
			// 		});
			// 		SetImageBoxInfo(new ImageBoxInfo()
			// 		{
			// 			ID = new() { LayoutID = 0, LayerID = cnt, },
			// 			Image = new() { ImageName = "test", SubRect = new(), },
			// 		});
			// 	}

			// 	await new PeriodicTimer(TimeSpan.FromMilliseconds(30000)).WaitForNextTickAsync();
			// 	//跟踪 10s 的帧率
			// 	RenderInfo.SatrtRecord();
			// 	await new PeriodicTimer(TimeSpan.FromMilliseconds(10000)).WaitForNextTickAsync();
			// 	RenderInfo.SatrtRecord(false);
			// 	_layoutManager.Clear();

			// 	Logger.LogInfo($"{string.Join(",", RenderInfo.GetRecord())}");

			// 	RenderInfo.Clear();
			// }
			// #endregion

			// #region ImageBox Render Test (10)
			// {
			// 	Logger.LogInfo("ImageBox Render Test (10):", Global.LogLevel.Todo);

			// 	RegisterLayout(new LayerIdInfo() { LayoutID = 0, });
			// 	SetActiveLayout(new LayerIdInfo() { LayoutID = 0, });

			// 	for (int cnt = 0; cnt < 10; cnt++)
			// 	{
			// 		RegisterLayer(new LayerBox()
			// 		{
			// 			Attribute = new()
			// 			{
			// 				ID = new() { LayoutID = 0, LayerID = cnt, },
			// 				Position = new(5 * cnt, 5 * cnt),
			// 				Size = new(1280, 720),
			// 				Type = "WidgetImageBox",
			// 			}
			// 		});
			// 		SetImageBoxInfo(new ImageBoxInfo()
			// 		{
			// 			ID = new() { LayoutID = 0, LayerID = cnt, },
			// 			Image = new() { ImageName = "test", SubRect = new(), },
			// 		});
			// 	}

			// 	await new PeriodicTimer(TimeSpan.FromMilliseconds(30000)).WaitForNextTickAsync();
			// 	//跟踪 10s 的帧率
			// 	RenderInfo.SatrtRecord();
			// 	await new PeriodicTimer(TimeSpan.FromMilliseconds(10000)).WaitForNextTickAsync();
			// 	RenderInfo.SatrtRecord(false);
			// 	_layoutManager.Clear();

			// 	Logger.LogInfo($"{string.Join(",", RenderInfo.GetRecord())}");

			// 	RenderInfo.Clear();
			// }
			// #endregion

			// #region ImageBox Render Test (20)
			// {
			// 	Logger.LogInfo("ImageBox Render Test (20):", Global.LogLevel.Todo);

			// 	RegisterLayout(new LayerIdInfo() { LayoutID = 0, });
			// 	SetActiveLayout(new LayerIdInfo() { LayoutID = 0, });

			// 	for (int cnt = 0; cnt < 20; cnt++)
			// 	{
			// 		RegisterLayer(new LayerBox()
			// 		{
			// 			Attribute = new()
			// 			{
			// 				ID = new() { LayoutID = 0, LayerID = cnt, },
			// 				Position = new(5 * cnt, 5 * cnt),
			// 				Size = new(1280, 720),
			// 				Type = "WidgetImageBox",
			// 			}
			// 		});
			// 		SetImageBoxInfo(new ImageBoxInfo()
			// 		{
			// 			ID = new() { LayoutID = 0, LayerID = cnt, },
			// 			Image = new() { ImageName = "test", SubRect = new(), },
			// 		});
			// 	}

			// 	await new PeriodicTimer(TimeSpan.FromMilliseconds(30000)).WaitForNextTickAsync();
			// 	//跟踪 10s 的帧率
			// 	RenderInfo.SatrtRecord();
			// 	await new PeriodicTimer(TimeSpan.FromMilliseconds(10000)).WaitForNextTickAsync();
			// 	RenderInfo.SatrtRecord(false);
			// 	_layoutManager.Clear();

			// 	Logger.LogInfo($"{string.Join(",", RenderInfo.GetRecord())}");

			// 	RenderInfo.Clear();
			// }
			// #endregion


			await _resourceManager.PullImageAsync("test640", "/Image/test640.png");

			#region ImageBox Render Test 640 (1)
			{
				Logger.LogInfo("ImageBox Render Test 640 (1):", Global.LogLevel.Todo);

				RegisterLayout(new LayerIdInfo() { LayoutID = 0, });
				SetActiveLayout(new LayerIdInfo() { LayoutID = 0, });

				for (int cnt = 0; cnt < 1; cnt++)
				{
					RegisterLayer(new LayerBox()
					{
						Attribute = new()
						{
							ID = new() { LayoutID = 0, LayerID = cnt, },
							Position = new(5 * cnt, 5 * cnt),
							Size = new(640, 320),
							Type = "WidgetImageBox",
						}
					});
					SetImageBoxInfo(new ImageBoxInfo()
					{
						ID = new() { LayoutID = 0, LayerID = cnt, },
						Image = new() { ImageName = "test640", SubRect = new(), },
					});
				}

				await new PeriodicTimer(TimeSpan.FromMilliseconds(10000)).WaitForNextTickAsync();
				//跟踪 10s 的帧率
				RenderInfo.SatrtRecord();
				await new PeriodicTimer(TimeSpan.FromMilliseconds(10000)).WaitForNextTickAsync();
				RenderInfo.SatrtRecord(false);
				_layoutManager.Clear();

				Logger.LogInfo($"{string.Join(",", RenderInfo.GetRecord())}");

				RenderInfo.Clear();
			}
			#endregion

			#region ImageBox Render Test 640 (5)
			{
				Logger.LogInfo("ImageBox Render Test 640 (5):", Global.LogLevel.Todo);

				RegisterLayout(new LayerIdInfo() { LayoutID = 0, });
				SetActiveLayout(new LayerIdInfo() { LayoutID = 0, });

				for (int cnt = 0; cnt < 5; cnt++)
				{
					RegisterLayer(new LayerBox()
					{
						Attribute = new()
						{
							ID = new() { LayoutID = 0, LayerID = cnt, },
							Position = new(5 * cnt, 5 * cnt),
							Size = new(640, 320),
							Type = "WidgetImageBox",
						}
					});
					SetImageBoxInfo(new ImageBoxInfo()
					{
						ID = new() { LayoutID = 0, LayerID = cnt, },
						Image = new() { ImageName = "test640", SubRect = new(), },
					});
				}

				await new PeriodicTimer(TimeSpan.FromMilliseconds(10000)).WaitForNextTickAsync();
				//跟踪 10s 的帧率
				RenderInfo.SatrtRecord();
				await new PeriodicTimer(TimeSpan.FromMilliseconds(10000)).WaitForNextTickAsync();
				RenderInfo.SatrtRecord(false);
				_layoutManager.Clear();

				Logger.LogInfo($"{string.Join(",", RenderInfo.GetRecord())}");

				RenderInfo.Clear();
			}
			#endregion

			#region ImageBox Render Test 640 (10)
			{
				Logger.LogInfo("ImageBox Render Test 640 (10):", Global.LogLevel.Todo);

				RegisterLayout(new LayerIdInfo() { LayoutID = 0, });
				SetActiveLayout(new LayerIdInfo() { LayoutID = 0, });

				for (int cnt = 0; cnt < 10; cnt++)
				{
					RegisterLayer(new LayerBox()
					{
						Attribute = new()
						{
							ID = new() { LayoutID = 0, LayerID = cnt, },
							Position = new(5 * cnt, 5 * cnt),
							Size = new(640, 320),
							Type = "WidgetImageBox",
						}
					});
					SetImageBoxInfo(new ImageBoxInfo()
					{
						ID = new() { LayoutID = 0, LayerID = cnt, },
						Image = new() { ImageName = "test640", SubRect = new(), },
					});
				}

				await new PeriodicTimer(TimeSpan.FromMilliseconds(10000)).WaitForNextTickAsync();
				//跟踪 10s 的帧率
				RenderInfo.SatrtRecord();
				await new PeriodicTimer(TimeSpan.FromMilliseconds(10000)).WaitForNextTickAsync();
				RenderInfo.SatrtRecord(false);
				_layoutManager.Clear();

				Logger.LogInfo($"{string.Join(",", RenderInfo.GetRecord())}");

				RenderInfo.Clear();
			}
			#endregion

			#region ImageBox Render Test 640 (20)
			{
				Logger.LogInfo("ImageBox Render Test 640 (20):", Global.LogLevel.Todo);

				RegisterLayout(new LayerIdInfo() { LayoutID = 0, });
				SetActiveLayout(new LayerIdInfo() { LayoutID = 0, });

				for (int cnt = 0; cnt < 20; cnt++)
				{
					RegisterLayer(new LayerBox()
					{
						Attribute = new()
						{
							ID = new() { LayoutID = 0, LayerID = cnt, },
							Position = new(5 * cnt, 5 * cnt),
							Size = new(640, 320),
							Type = "WidgetImageBox",
						}
					});
					SetImageBoxInfo(new ImageBoxInfo()
					{
						ID = new() { LayoutID = 0, LayerID = cnt, },
						Image = new() { ImageName = "test640", SubRect = new(), },
					});
				}

				await new PeriodicTimer(TimeSpan.FromMilliseconds(10000)).WaitForNextTickAsync();
				//跟踪 10s 的帧率
				RenderInfo.SatrtRecord();
				await new PeriodicTimer(TimeSpan.FromMilliseconds(10000)).WaitForNextTickAsync();
				RenderInfo.SatrtRecord(false);
				_layoutManager.Clear();

				Logger.LogInfo($"{string.Join(",", RenderInfo.GetRecord())}");

				RenderInfo.Clear();
			}
			#endregion

		}
	}
}