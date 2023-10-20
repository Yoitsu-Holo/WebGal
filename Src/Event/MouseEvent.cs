using WebGal.Libs.Base;
namespace WebGal.Event;

public enum MouseButton
{
	Empty,
	Any,
	LButton,
	MButton,
	RButton,
	Button4,
	Button5,
	Button6,
	Button7,
	Button8,
	MouseChord, // mouse chord key - both the left and right mouse buttons pressed at the same time
	MouseWheel // By default is bound to the "+camdistadjust" command. But can be used for other commands (e.g. target_next_enemy).
}

public enum MouseStatus
{
	Release,
	Hold,
	Up,
	Down,
	WheelUp,
	WheelDown
}

public record struct MouseEvent
(
	IVector Position,
	MouseButton Button,
	MouseStatus Status
);