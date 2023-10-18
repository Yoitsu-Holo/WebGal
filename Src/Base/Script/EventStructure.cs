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