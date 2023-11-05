export function audioToUrl(audioBlob, audioType) {
    var fileType = "audio/" + audioType;
    var myBlob = new Blob([audioBlob], { type: fileType });
    return (window.URL || window.webkitURL).createObjectURL(myBlob);
}
export function setAudioVolume(audioVolume, audioName) {
    var audio = document.getElementById(audioName);
    audio.volume = audioVolume;
}
export function getAudioLength(audioName) {
    var audio = document.getElementById(audioName);
    var durationMs = audio.duration * 1000;
    // console.log(`The duration of the audio is ${durationMs} milliseconds.`);
}
// public setAudioVolume(audioVolume: number) {
// 	let audio = document.getElementById("test") as HTMLAudioElement;
// 	audio.volume = audioVolume;
// }
