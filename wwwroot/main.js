window.getDimensions = function () {
	var myDiv = document.getElementById('gamecanvas');
	return {
		width: myDiv.clientWidth,
		height: myDiv.clientHeight
	};
};