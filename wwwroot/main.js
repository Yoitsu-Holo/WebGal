window.getDimensions = function () {
	var myDiv = document.getElementById('screnn');
	return {
		width: myDiv.clientWidth,
		height: myDiv.clientHeight
	};
};

// window.getMousePosition = function () {
// 	return new Promise((resolve) => {
// 		window.onmousemove = function (e) {
// 			var myDiv = document.getElementById('gamecanvas');
// 			resolve({ x: myDiv.clientX, y: myDiv.clientY });
// 			window.onmousemove = null;
// 		};
// 	});
// }

window.globalVar = {};

window.getMousePosition = function (event) {
	window.globalVar.X = event.offsetX;
	window.globalVar.Y = event.offsetY;
};