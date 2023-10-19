namespace WebGal.Libs.Base;

public record struct ResouresStructure
(
	List<UrlStructure>? ImageURL,
	List<UrlStructure>? AudioURL,
	List<UrlStructure>? TextURL
);
