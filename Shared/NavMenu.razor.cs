using WebGal.Global;

namespace WebGal.Shared;

public partial class NavMenu
{
	public bool collapseNavMenu = true;

	public string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

	private int _fps, _frameTime;
	private int _mouseX, _mouseY;

	private PeriodicTimer _timer = null!;

	public void ToggleNavMenu() => collapseNavMenu = !collapseNavMenu;

	protected override void OnInitialized()
	{
		_timer = new PeriodicTimer(TimeSpan.FromMilliseconds(100));
		_ = Task.Run(async () =>
		{
			while (await _timer.WaitForNextTickAsync())
			{
				_fps = GameStatus.FPS;
				_frameTime = GameStatus.FrameTime;
				_mouseX = GameStatus.MouseX;
				_mouseY = GameStatus.MouseY;
				StateHasChanged();
			}
		});
	}
}