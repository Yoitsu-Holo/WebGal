using WebGal.Event;

namespace WebGal.Libs.Base;

public record struct EventStructure
(
	TrigerStructure? Triger,
	List<ActionStructure>? Action
);

public record struct TrigerStructure
(
	string LayerName,
	MouseEvent MouseEvent,
	KeyboardEvent KeyboardEvent
);

public record struct ActionStructure
(
	string? LayerName,
	LayerAtrribute Attribute,
	string? JumpNodeLabel,
	string? JumpSceneLabel
);

// https://archive.paragonwiki.com/wiki/List_of_Key_Names



public record struct EventStructureRegister(
	TrigerStructureRegister O_Triger,
	ActionStructureRegister C_LoAction
);

public record struct TrigerStructureRegister(
	int C_sLayerName,
	MouseEventRegister O_MouseEvent,
	KeyboardEventRegister O_KeyboardEvent
);
public record struct ActionStructureRegister(
	int C_sLayerName,
	LayerAtrributeRegister O_Attribute,
	int C_sJumpNodeLabel,
	int C_SJumpSceneLabel
);
