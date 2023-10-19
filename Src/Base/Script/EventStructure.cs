using SkiaSharp;

namespace WebGal.Libs.Base;

public record struct EventStructure
(
	TrigerStructure? Triger,
	List<ActionStructure>? Action
);

public record struct TrigerStructure
(
	string LayerName = "",
	string MouseEvent = "",
	string KeyboardEvent = ""
);

public record struct ActionStructure
(
	string LayerName,
	LayerAtrribute Attribute
);

// https://archive.paragonwiki.com/wiki/List_of_Key_Names
public enum MouseButton
{
	Space,
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
	ButtonUp,
	ButtonDown,
	WheelUp,
	WheelDown
}

public class MouseEvent
{
	public SKPointI Position { get; set; } = new(0, 0);
	public MouseButton Button { get; set; }
	public MouseStatus Status { get; set; }
};