function audioToUrl(blob, type) {
	var fileType = "audio/" + type;
	var myBlob = new Blob([blob], { type: [fileType] });
	return (window.URL || window.webkitURL || window || {}).createObjectURL(myBlob);
}