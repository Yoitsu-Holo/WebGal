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
	logError: function (message, info) {
		console.error('%c' + message + "\n" + '%c' + info, 'color: red;', 'color: blue;');
	}
};

window.run = function (name) {
	DotNet.invokeMethodAsync('WebGal', name)
		.then(result => { console.log(result); });
}

Object.defineProperty(window, 'help', {
	get: function () {
		DotNet.invokeMethodAsync('WebGal', 'Help')
			.then(result => { console.log(result); });
	}
});