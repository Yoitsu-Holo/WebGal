namespace WebGal.Libs.Base;

public record struct SceneStructure
(
	bool IsStatic,
	List<string>? LoopAudio,
	List<string>? OneShotAudio,
	List<LayerStructure>? Layers,
	List<EventStructure>? Events
);
