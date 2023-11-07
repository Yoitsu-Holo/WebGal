namespace WebGal.Types;

public record struct ResouresStructure
(
	List<UrlStructure>? ImageURL,
	List<UrlStructure>? AudioURL,
	List<UrlStructure>? TextURL
);

public record struct ResouresStructureRegister
(
	List<UrlStructureRegister> O_ImageURL,
	List<UrlStructureRegister> O_AudioURL,
	List<UrlStructureRegister> O_TextURL
);
