window.getDimensions = function () {
	var myDiv = document.getElementById('screnn');
	return {
		width: myDiv.clientWidth,
		height: myDiv.clientHeight
	};
};

window.globalVar = {};

window.getMousePosition = function (event) {
	window.globalVar.X = event.offsetX;
	window.globalVar.Y = event.offsetY;
};

window.consoleLogger = {
	logInfo: function (message) {
		console.info('%c' + message, 'color: blue;');
	},
	logTodo: function (message) {
		console.info('%c' + message, 'color: brown;');
	},
	logWarning: function (message) {
		console.warn('%c' + message, 'color: orange;');
	},
	logError: function (message) {
		console.error('%c' + message, 'color: red;');
	}
};