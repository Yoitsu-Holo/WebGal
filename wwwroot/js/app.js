function audioOggToLink(blob) {
	var myBlob = new Blob([blob], { type: "audio/ogg" });
	return (window.URL || window.webkitURL || window || {}).createObjectURL(myBlob);
}