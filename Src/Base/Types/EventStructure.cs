using WebGal.Event;

namespace WebGal.Libs.Base;

public record struct EventStructure
(
	TrigerStructure? Triger,
	List<ActionStructure>? Action
);

public record struct TrigerStructure
(
	string? LayerName,
	MouseEvent MouseEvent,
	KeyboardEvent KeyboardEvent
);

public record struct ActionStructure
(
	string LayerName,
	LayerAtrribute Attribute
);

// https://archive.paragonwiki.com/wiki/List_of_Key_Names
