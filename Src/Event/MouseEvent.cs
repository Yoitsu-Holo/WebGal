using WebGal.Libs.Base;
namespace WebGal.Event;

public enum MouseButton : ushort
{
	Empty = 0x0000,
	LButton = 0x0001,
	MButton = 0x0002,
	RButton = 0x0004,
	Button4 = 0x0008,
	Button5 = 0x0010,
	Button6 = 0x0011,
	Button7 = 0x0012,
	Button8 = 0x0014,
	MouseChord = 0x0018, // mouse chord key - both the left and right mouse buttons pressed at the same time
	MouseWheel = 0x0020, // By default is bound to the "+camdistadjust" command. But can be used for other commands (e.g. target_next_enemy).
	Any = 0xffff
}

public enum MouseStatus : byte
{
	Release = 0x01,
	Hold = 0x02,
	Up = 0x04,
	Down = 0x08,
	WheelUp = 0x10,
	WheelDown = 0x11,
	Any = 0xff
}

public record struct MouseEvent
(
	IVector Position,
	MouseButton Button,
	MouseStatus Status
);