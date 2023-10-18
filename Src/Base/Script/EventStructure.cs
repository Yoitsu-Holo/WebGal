namespace WebGal.Libs.Base;

public record struct EventStructure
(
	TigerStructure? Triger,
	List<ActionStructure>? Action
);

public record struct TigerStructure
(
	string? LayerName,
	string? MouseEvent,
	string? KeyboardEvent
);

public record struct ActionStructure
(
	string? LayerName,
	LayerAtrribute Atrribute
);