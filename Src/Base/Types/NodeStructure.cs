namespace WebGal.Libs.Base;

public record struct NodeStructure
(
	bool IsLeaf,
	List<string>? LoopAudio,
	List<string>? OneShotAudio,
	List<UrlStructure>? ResouresPackURL,
	List<UrlStructure>? NodeURL
);
