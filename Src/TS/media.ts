namespace media {
	function audioToUrl(audioBlob: Blob, audioType: string) {
		let fileType = "audio/" + audioType;
		let myBlob = new Blob([audioBlob], { type: fileType });
		return (window.URL || window.webkitURL || window || {}).createObjectURL(myBlob);
	}

	function setAudioVolume(audioVolume: number, audioName: string) {
		let audio = document.getElementById(audioName) as HTMLAudioElement;
		audio.volume = audioVolume;
	}
}