namespace WebGal.Types;

public record struct SceneStructure
(
	List<string>? LoopAudio,
	List<string>? OneShotAudio,
	List<LayerStructure>? Layers,
	List<EventArgs>? Events
);

public record struct SceneStructureRegister
(
	List<string> C_LsLoopAudio,
	List<string> C_LsOneShotAudio,
	List<LayerStructureRegister> O_Layers,
	List<EventArgs> O_Events
);
