function audioToUrl(blob, type) {
	var fileType = "audio/" + type;
	var myBlob = new Blob([blob], { type: [fileType] });
	return (window.URL || window.webkitURL || window || {}).createObjectURL(myBlob);
}

function setAudioVolume(volume) {
	let audio = document.getElementById("test");
	audio.volume = volume;
};