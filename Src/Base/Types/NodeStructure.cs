namespace WebGal.Libs.Base;

// 默认加载第一个场景
public record struct GameStructure
(
	List<UrlStructure> NodeURLs
);

public record struct NodeStructure
(
	List<UrlStructure>? ResouresPackURL,
	List<UrlStructure>? SceneURLs
);

public record struct GameStructureRegister
(
	List<UrlStructureRegister> O_NodeURLs
);

public record struct NodeStructureRegister
(
	List<UrlStructureRegister> O_ResouresPackURL,
	List<UrlStructureRegister> O_SceneURLs
);