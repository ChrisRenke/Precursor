// For touchscreen devices -- draw a line with your finger

var lineMaterial : Material;
var maxPoints = 500;
var lineWidth = 4.0;
var minPixelMove = 5;	// Must move at least this many pixels per sample for a new segment to be recorded
private var linePoints : Vector2[];
private var line : VectorLine;
private var touch : Touch;
private var lineIndex = 0;
private var previousPosition : Vector2;
private var sqrMinPixelMove : int;
private var canDraw = false;

function Start () {
	linePoints = new Vector2[maxPoints];
	line = new VectorLine("DrawnLine", linePoints, lineMaterial, lineWidth, LineType.Continuous);
	sqrMinPixelMove = minPixelMove*minPixelMove;
}

function Update () {
	if (Input.touchCount > 0) {
		touch = Input.GetTouch(0);
		if (touch.phase == TouchPhase.Began) {
			line.ZeroPoints();
			line.minDrawIndex = 0;
			line.Draw();
			previousPosition = linePoints[0] = touch.position;
			lineIndex = 0;
			canDraw = true;
		}
		else if (touch.phase == TouchPhase.Moved && (touch.position - previousPosition).sqrMagnitude > sqrMinPixelMove && canDraw) {
			previousPosition = linePoints[++lineIndex] = touch.position;
			line.minDrawIndex = lineIndex-1;
			line.maxDrawIndex = lineIndex;
			if (lineIndex >= maxPoints-1) canDraw = false;
			line.Draw();
		}
	}
}