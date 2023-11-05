export function audioToUrl(audioBlob: Blob, audioType: string) {
	let fileType = "audio/" + audioType;
	let myBlob = new Blob([audioBlob], { type: fileType });
	return (window.URL || window.webkitURL).createObjectURL(myBlob);
}

export function setAudioVolume(audioVolume: number, audioName: string) {
	let audio = document.getElementById(audioName) as HTMLAudioElement;
	audio.volume = audioVolume;
}

export function getAudioLength(audioName: string) {
	let audio = document.getElementById(audioName) as HTMLAudioElement
	const durationMs = audio.duration * 1000;
	console.log(`The duration of the audio is ${durationMs} milliseconds.`);
}

// public setAudioVolume(audioVolume: number) {
// 	let audio = document.getElementById("test") as HTMLAudioElement;
// 	audio.volume = audioVolume;
// }

