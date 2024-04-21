using WebGal.Global;

namespace WebGal.Shared;

public partial class NavMenu
{
	public bool collapseNavMenu = true;

	public string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

	private int _fps, _frameTime;
	private int _mouseX, _mouseY;

	private Timer _timer = null!;

	public void ToggleNavMenu() => collapseNavMenu = !collapseNavMenu;

	protected override void OnInitialized()
	{
		_timer = new Timer(_ =>
		{
			// 注意：因为定时器的回调在另一个线程中运行，所以我们需要调用InvokeAsync来确保在正确的线程上更新UI
			InvokeAsync(() =>
			{
				_fps = GameStatus.FPS;
				_frameTime = GameStatus.FrameTime;
				_mouseX = GameStatus.MouseX;
				_mouseY = GameStatus.MouseY;
				StateHasChanged();
			});
		}, null, 0, 100);  // 首次执行延迟0毫秒，然后每1000毫秒执行一次
	}
}