// Version 2.0.1
// ©2012 Starscene Software. All rights reserved. Redistribution of source code without permission not allowed.

using UnityEngine;
using System.Collections.Generic;

public enum LineType {Continuous, Discrete}
public enum Joins {Fill, Weld, None}

public class VectorLine {
	public GameObject vectorObject;
	MeshFilter m_meshFilter;
	Mesh m_mesh;
	Vector3[] m_lineVertices;
	Vector2[] m_lineUVs;
	Color[] m_lineColors;
	public Color color {
		get {return m_lineColors[0];}
	}
	public Vector2[] points2;
	public Vector3[] points3;
	int m_pointsLength;
	bool m_is2D;
	Vector3[] m_screenPoints;
	float[] m_lineWidths;
	public float lineWidth {
		get {return m_lineWidths[0] * 2;}
		set {
			if (m_lineWidths.Length == 1) {
				m_lineWidths[0] = value * .5f;
			}
			else {
				float thisWidth = value * .5f;
				for (int i = 0; i < m_lineWidths.Length; i++) {
					m_lineWidths[i] = thisWidth;
				}
			}
			m_maxWeldDistance = (value*2) * (value*2);
		}
	}
	float m_maxWeldDistance;
	public float maxWeldDistance {
		get {return Mathf.Sqrt(m_maxWeldDistance);}
		set {m_maxWeldDistance = value * value;}
	}
	float[] m_distances;
	string m_name;
	public string name {
		get {return m_name;}
		set {
			m_name = value;
			if (vectorObject != null) {
				vectorObject.name = "Vector " + value;
			}
			if (m_mesh != null) {
				m_mesh.name = value;
			}
		}
	}
	public Material material {
		get {return vectorObject.renderer.material;}
		set {vectorObject.renderer.material = value;}
	}
	bool m_active = true;
	public bool active {
		get {return m_active;}
		set {
			m_active = value;
			if (vectorObject != null) {
				vectorObject.renderer.enabled = m_active;
			}
		}
	}
	public float capLength = 0.0f;
	int m_depth = 0;
	public int depth {
		get {return m_depth;}
		set {m_depth = Mathf.Clamp(value, 0, 100);}
	}
	public bool smoothWidth = false;
	int m_layer = -1;
	public int layer {
		get {return m_layer;}
		set {
			m_layer = value;
			if (m_layer < 0) m_layer = 0;
			else if (m_layer > 31) m_layer = 31;
			if (vectorObject != null) {
				vectorObject.layer = m_layer;
			}
		}
	}
	bool m_continuous;
	public bool continuous {
		get {return m_continuous;}
	}
	Joins m_joins;
	public Joins joins {
		get {return m_joins;}
		set {
			if (m_isPoints) return;
			if (!m_continuous && value == Joins.Fill) return;
			m_joins = value;
		}
	}
	bool m_isPoints;
	int m_maxDrawIndex = 0;
	public int maxDrawIndex {
		get {return m_maxDrawIndex;}
		set {
			m_maxDrawIndex = value;
			if (m_maxDrawIndex < 0) m_maxDrawIndex = 0;
			else {
				if (!m_is2D) {
					if (m_maxDrawIndex > points3.Length-1) m_maxDrawIndex = points3.Length-1;
				}
				else {
					if (m_maxDrawIndex > points2.Length-1) m_maxDrawIndex = points2.Length-1;
				}
			}
		}
	}
	int m_minDrawIndex = 0;
	public int minDrawIndex {
		get {return m_minDrawIndex;}
		set {
			m_minDrawIndex = value;
			if (!m_continuous && (m_minDrawIndex & 1) != 0) {	// No odd numbers for discrete lines
				m_minDrawIndex++;
			}
			if (m_minDrawIndex < 0) m_minDrawIndex = 0;
			else {
				if (!m_is2D) {
					if (m_minDrawIndex > points3.Length-1) m_minDrawIndex = points3.Length-1;
				}
				else {
					if (m_minDrawIndex > points2.Length-1) m_minDrawIndex = points2.Length-1;
				}
			}
		}
	}
	int m_drawStart = 0;
	public int drawStart {
		get {return m_drawStart;}
		set {
			if (!m_continuous && (value & 1) != 0) {
				value++;
			}
			m_drawStart = Mathf.Clamp(value, 0, m_is2D? points2.Length : points3.Length);
		}
	}
	int m_drawEnd = 0;
	public int drawEnd {
		get {return m_drawEnd;}
		set {
			if (!m_continuous && (value & 1) == 0) {
				value++;
			}
			m_drawEnd = Mathf.Clamp(value, 0, m_is2D? points2.Length : points3.Length);
		}
	}
	bool m_useNormals = false;
	int m_triangleCount;
	int m_vertexCount;
	
	// Vector3 constructors
	public VectorLine (string lineName, Vector3[] linePoints, Material lineMaterial, float width) {
		points3 = linePoints;
		Color[] colors = SetColor(Color.white, LineType.Discrete, linePoints.Length, false);
		SetupMesh (ref lineName, lineMaterial, colors, ref width, LineType.Discrete, Joins.None, false, false);
	}
	public VectorLine (string lineName, Vector3[] linePoints, Color color, Material lineMaterial, float width) {
		points3 = linePoints;
		Color[] colors = SetColor(color, LineType.Discrete, linePoints.Length, false);
		SetupMesh (ref lineName, lineMaterial, colors, ref width, LineType.Discrete, Joins.None, false, false);
	}
	public VectorLine (string lineName, Vector3[] linePoints, Color[] colors, Material lineMaterial, float width) {
		points3 = linePoints;
		SetupMesh (ref lineName, lineMaterial, colors, ref width, LineType.Discrete, Joins.None, false, false);
	}

	public VectorLine (string lineName, Vector3[] linePoints, Material lineMaterial, float width, LineType lineType) {
		points3 = linePoints;
		Color[] colors = SetColor(Color.white, lineType, linePoints.Length, false);
		SetupMesh (ref lineName, lineMaterial, colors, ref width, lineType, Joins.None, false, false);
	}
	public VectorLine (string lineName, Vector3[] linePoints, Color color, Material lineMaterial, float width, LineType lineType) {
		points3 = linePoints;
		Color[] colors = SetColor(color, lineType, linePoints.Length, false);
		SetupMesh (ref lineName, lineMaterial, colors, ref width, lineType, Joins.None, false, false);
	}
	public VectorLine (string lineName, Vector3[] linePoints, Color[] colors, Material lineMaterial, float width, LineType lineType) {
		points3 = linePoints;
		SetupMesh (ref lineName, lineMaterial, colors, ref width, lineType, Joins.None, false, false);
	}

	public VectorLine (string lineName, Vector3[] linePoints, Material lineMaterial, float width, LineType lineType, Joins joins) {
		points3 = linePoints;
		Color[] colors = SetColor(Color.white, lineType, linePoints.Length, false);
		SetupMesh (ref lineName, lineMaterial, colors, ref width, lineType, joins, false, false);
	}
	public VectorLine (string lineName, Vector3[] linePoints, Color color, Material lineMaterial, float width, LineType lineType, Joins joins) {
		points3 = linePoints;
		Color[] colors = SetColor(color, lineType, linePoints.Length, false);
		SetupMesh (ref lineName, lineMaterial, colors, ref width, lineType, joins, false, false);
	}
	public VectorLine (string lineName, Vector3[] linePoints, Color[] colors, Material lineMaterial, float width, LineType lineType, Joins joins) {
		points3 = linePoints;
		SetupMesh (ref lineName, lineMaterial, colors, ref width, lineType, joins, false, false);
	}

	// Vector2 constructors
	public VectorLine (string lineName, Vector2[] linePoints, Material lineMaterial, float width) {
		points2 = linePoints;
		Color[] colors = SetColor(Color.white, LineType.Discrete, linePoints.Length, false);
		SetupMesh (ref lineName, lineMaterial, colors, ref width, LineType.Discrete, Joins.None, true, false);
	}
	public VectorLine (string lineName, Vector2[] linePoints, Color color, Material lineMaterial, float width) {
		points2 = linePoints;
		Color[] colors = SetColor(color, LineType.Discrete, linePoints.Length, false);
		SetupMesh (ref lineName, lineMaterial, colors, ref width, LineType.Discrete, Joins.None, true, false);
	}
	public VectorLine (string lineName, Vector2[] linePoints, Color[] colors, Material lineMaterial, float width) {
		points2 = linePoints;
		SetupMesh (ref lineName, lineMaterial, colors, ref width, LineType.Discrete, Joins.None, true, false);
	}

	public VectorLine (string lineName, Vector2[] linePoints, Material lineMaterial, float width, LineType lineType) {
		points2 = linePoints;
		Color[] colors = SetColor(Color.white, lineType, linePoints.Length, false);
		SetupMesh (ref lineName, lineMaterial, colors, ref width, lineType, Joins.None, true, false);
	}
	public VectorLine (string lineName, Vector2[] linePoints, Color color, Material lineMaterial, float width, LineType lineType) {
		points2 = linePoints;
		Color[] colors = SetColor(color, lineType, linePoints.Length, false);
		SetupMesh (ref lineName, lineMaterial, colors, ref width, lineType, Joins.None, true, false);
	}
	public VectorLine (string lineName, Vector2[] linePoints, Color[] colors, Material lineMaterial, float width, LineType lineType) {
		points2 = linePoints;
		SetupMesh (ref lineName, lineMaterial, colors, ref width, lineType, Joins.None, true, false);
	}

	public VectorLine (string lineName, Vector2[] linePoints, Material lineMaterial, float width, LineType lineType, Joins joins) {
		points2 = linePoints;
		Color[] colors = SetColor(Color.white, lineType, linePoints.Length, false);
		SetupMesh (ref lineName, lineMaterial, colors, ref width, lineType, joins, true, false);
	}
	public VectorLine (string lineName, Vector2[] linePoints, Color color, Material lineMaterial, float width, LineType lineType, Joins joins) {
		points2 = linePoints;
		Color[] colors = SetColor(color, lineType, linePoints.Length, false);
		SetupMesh (ref lineName, lineMaterial, colors, ref width, lineType, joins, true, false);
	}
	public VectorLine (string lineName, Vector2[] linePoints, Color[] colors, Material lineMaterial, float width, LineType lineType, Joins joins) {
		points2 = linePoints;
		SetupMesh (ref lineName, lineMaterial, colors, ref width, lineType, joins, true, false);
	}

	// Points constructors
	protected VectorLine (bool usePoints, string lineName, Vector2[] linePoints, Material lineMaterial, float width) {
		points2 = linePoints;
		Color[] colors = SetColor(Color.white, LineType.Continuous, linePoints.Length, true);
		SetupMesh (ref lineName, lineMaterial, colors, ref width, LineType.Continuous, Joins.None, true, true);
	}
	protected VectorLine (bool usePoints, string lineName, Vector2[] linePoints, Color color, Material lineMaterial, float width) {
		points2 = linePoints;
		Color[] colors = SetColor(color, LineType.Continuous, linePoints.Length, true);
		SetupMesh (ref lineName, lineMaterial, colors, ref width, LineType.Continuous, Joins.None, true, true);
	}
	protected VectorLine (bool usePoints, string lineName, Vector2[] linePoints, Color[] colors, Material lineMaterial, float width) {
		points2 = linePoints;
		SetupMesh (ref lineName, lineMaterial, colors, ref width, LineType.Continuous, Joins.None, true, true);
	}

	protected VectorLine (bool usePoints, string lineName, Vector3[] linePoints, Material lineMaterial, float width) {
		points3 = linePoints;
		Color[] colors = SetColor(Color.white, LineType.Continuous, linePoints.Length, true);
		SetupMesh (ref lineName, lineMaterial, colors, ref width, LineType.Continuous, Joins.None, false, true);
	}
	protected VectorLine (bool usePoints, string lineName, Vector3[] linePoints, Color[] colors, Material lineMaterial, float width) {
		points3 = linePoints;
		SetupMesh (ref lineName, lineMaterial, colors, ref width, LineType.Continuous, Joins.None, false, true);
	}
	protected VectorLine (bool usePoints, string lineName, Vector3[] linePoints, Color color, Material lineMaterial, float width) {
		points3 = linePoints;
		Color[] colors = SetColor(color, LineType.Continuous, linePoints.Length, true);
		SetupMesh (ref lineName, lineMaterial, colors, ref width, LineType.Continuous, Joins.None, false, true);
	}
		
	Color[] SetColor (Color color, LineType lineType, int size, bool usePoints) {
		if (!usePoints) {
			size = lineType == LineType.Continuous? size-1 : size/2;
		}
		Color[] colors = new Color[size];
		for (int i = 0; i < size; i++) {
			colors[i] = color;
		}
		return colors;
	}

	protected void SetupMesh (ref string lineName, Material useMaterial, Color[] colors, ref float width, LineType lineType, Joins joins, bool use2Dlines, bool usePoints) {
		m_continuous = (lineType == LineType.Continuous);
		m_is2D = use2Dlines;
		if (joins == Joins.Fill && !m_continuous) {
			Debug.LogError("VectorLine: Must use LineType.Continuous if using Joins.Fill for \"" + lineName + "\"");
			return;
		}
		if ( (m_is2D && points2 == null) || (!m_is2D && points3 == null) ) {
			Debug.LogError("VectorLine: the points array is null for \"" + lineName + "\"");
			return;
		}
		if (colors == null) {
			Debug.LogError("Vectorline: the colors array is null for \"" + lineName + "\"");
			return;
		}
		m_pointsLength = m_is2D? points2.Length : points3.Length;
		if (!usePoints && m_pointsLength < 2) {
			Debug.LogError("The points array must contain at least two points");
			return;
		}
		if (!m_continuous && m_pointsLength%2 != 0) {
			Debug.LogError("VectorLine: Must have an even points array length for \"" + lineName + "\" when using LineType.Discrete");
			return;
		}
		
		m_maxWeldDistance = (width*2) * (width*2);
		m_drawEnd = m_pointsLength;
		m_lineWidths = new float[1];
		m_lineWidths[0] = width * .5f;
		m_isPoints = usePoints;
		m_joins = joins;
		bool useSegmentColors = true;
		int colorsLength = 0;
		
		if (!usePoints) {
			if (m_continuous) {
				if (colors.Length != m_pointsLength-1) {
					Debug.LogWarning("VectorLine: Length of color array for \"" + lineName + "\" must be length of points array minus one");
					useSegmentColors = false;
					colorsLength = m_pointsLength-1;
				}
			}
			else {
				if (colors.Length != m_pointsLength/2) {
					Debug.LogWarning("VectorLine: Length of color array for \"" + lineName + "\" must be exactly half the length of points array");
					useSegmentColors = false;
					colorsLength = m_pointsLength/2;
				}
			}
		}
		else {
			if (colors.Length != m_pointsLength) {
				Debug.LogWarning("VectorLine: Length of color array for \"" + lineName + "\" must be the same length as the points array");
				useSegmentColors = false;
				colorsLength = m_pointsLength;
			}
		}
		if (!useSegmentColors) {
			colors = new Color[colorsLength];
			for (int i = 0; i < colorsLength; i++) {
				colors[i] = Color.white;
			}
		}
		
		if (useMaterial == null) {
			if (defaultMaterial == null) {
				defaultMaterial = new Material("Shader \"Vertex Colors/Alpha\" {Category{Tags {\"Queue\"=\"Transparent\" \"IgnoreProjector\"=\"True\" \"RenderType\"=\"Transparent\"}SubShader {Cull Off ZWrite On Blend SrcAlpha OneMinusSrcAlpha Pass {BindChannels {Bind \"Color\", color Bind \"Vertex\", vertex}}}}}");
			}
			useMaterial = defaultMaterial;
		}
	
		vectorObject = new GameObject("Vector "+lineName, typeof(MeshRenderer));
		vectorObject.layer = vectorLayer;
		vectorObject.renderer.material = useMaterial;
		m_mesh = new Mesh();
		m_mesh.name = lineName;
		m_meshFilter = (MeshFilter)vectorObject.AddComponent(typeof(MeshFilter));
		m_meshFilter.mesh = m_mesh;		
		name = lineName;
		BuildMesh (colors);
	}
	
	public void Resize (Vector3[] linePoints) {
		if (m_is2D) {
			Debug.LogError ("Must supply a Vector2 array instead of a Vector3 array for \"" + name + "\"");
			return;
		}
		points3 = linePoints;
		m_pointsLength = linePoints.Length;
		RebuildMesh();
	}

	public void Resize (Vector2[] linePoints) {
		if (!m_is2D) {
			Debug.LogError ("Must supply a Vector3 array instead of a Vector2 array for \"" + name + "\"");
			return;
		}
		points2 = linePoints;
		m_pointsLength = linePoints.Length;
		RebuildMesh();
	}
	
	public void Resize (int newSize) {
		if (m_is2D) {
			points2 = new Vector2[newSize];
		}
		else {
			points3 = new Vector3[newSize];
		}
		m_pointsLength = newSize;
		RebuildMesh();
	}
	
	void RebuildMesh () {
		if (!m_continuous && m_pointsLength%2 != 0) {
			Debug.LogError("VectorLine.Resize: Must have an even points array length for \"" + name + "\" when using LineType.Discrete");
			return;
		}
		
		m_mesh.Clear();

		Color[] colors = SetColor (m_lineColors[0], m_continuous? LineType.Continuous : LineType.Discrete, m_pointsLength, m_isPoints);
		if (m_lineWidths.Length > 1) {
			float thisWidth = lineWidth;
			m_lineWidths = new float[m_pointsLength];
			lineWidth = thisWidth;
		}

		BuildMesh (colors);
		m_minDrawIndex = 0;
		m_maxDrawIndex = 0;
		m_drawStart = 0;
		m_drawEnd = m_pointsLength;
	}
	
	void BuildMesh (Color[] colors) {
		if (m_continuous) {
			m_vertexCount = m_isPoints? m_vertexCount = (m_pointsLength)*4 : m_vertexCount = (m_pointsLength-1)*4;
		}
		else {
			m_vertexCount = m_pointsLength*2;
		}
		if (m_vertexCount > 65534) {
			Debug.LogError("VectorLine: exceeded maximum vertex count of 65534 for \"" + name + "\"...use fewer points");
			return;
		}
		
		m_lineVertices = new Vector3[m_vertexCount];
		m_lineUVs = new Vector2[m_vertexCount];
		
		int idx = 0, end = 0;
		if (!m_isPoints) {
			end = m_continuous? m_pointsLength-1 : m_pointsLength/2;
		}
		else {
			end = m_pointsLength;
		}
		
		for (int i = 0; i < end; i++) {	
			m_lineUVs[idx]   = new Vector2(0.0f, 1.0f);
			m_lineUVs[idx+1] = new Vector2(0.0f, 0.0f);
			m_lineUVs[idx+2] = new Vector2(1.0f, 1.0f);
			m_lineUVs[idx+3] = new Vector2(1.0f, 0.0f);
			idx += 4;
		}
		
		m_lineColors = new Color[m_vertexCount];
		idx = 0;
		for (int i = 0; i < end; i++) {
			m_lineColors[idx]   = colors[i];
			m_lineColors[idx+1] = colors[i];
			m_lineColors[idx+2] = colors[i];
			m_lineColors[idx+3] = colors[i];
			idx += 4;
		}
		
		m_mesh.vertices = m_lineVertices;
		m_mesh.uv = m_lineUVs;
		m_mesh.colors = m_lineColors;
		SetupTriangles();
						
		if (!m_is2D) {
			m_screenPoints = new Vector3[m_lineVertices.Length];
		}
		if (m_useNormals) {
			m_mesh.RecalculateNormals();
		}
	}

	void SetupTriangles () {
		bool addPoint = false;
		
		if (m_continuous) {
			m_triangleCount = m_isPoints? m_triangleCount = (m_pointsLength)*6 : m_triangleCount = (m_pointsLength-1)*6;
			if (m_joins == Joins.Fill) {
				m_triangleCount += (m_pointsLength-2)*6;
				// Add another join fill if the first point equals the last point (like with a square)
				if ( (m_is2D && points2[0] == points2[points2.Length-1]) || (!m_is2D && points3[0] == points3[points3.Length-1]) ) {
					m_triangleCount += 6;
					addPoint = true;
				}
			}
		}
		else {
			m_triangleCount = m_pointsLength/2 * 6;
		}

		int[] newTriangles = new int[m_triangleCount];
		
		int idx = 0, end = 0;
		if (!m_isPoints) {
			end = m_continuous? (m_pointsLength-1)*4 : m_pointsLength*2;
		}
		else {
			end = m_pointsLength*4;
		}
		for (int i = 0; i < end; i += 4) {
			newTriangles[idx]   = i;
			newTriangles[idx+1] = i+2;
			newTriangles[idx+2] = i+1;
		
			newTriangles[idx+3] = i+2;
			newTriangles[idx+4] = i+3;
			newTriangles[idx+5] = i+1;
			idx += 6;
		}
		
		if (m_joins == Joins.Fill) {
			end -= 2;
			int i = 0;
			for (i = 2; i < end; i += 4) {
				newTriangles[idx]   = i;
				newTriangles[idx+1] = i+2;
				newTriangles[idx+2] = i+1;
		
				newTriangles[idx+3] = i+2;
				newTriangles[idx+4] = i+3;
				newTriangles[idx+5] = i+1;
				idx += 6;
			}
			if (addPoint) {
				newTriangles[idx]   = i;
				newTriangles[idx+1] = 0;
				newTriangles[idx+2] = i+1;
		
				newTriangles[idx+3] = 0;
				newTriangles[idx+4] = 1;
				newTriangles[idx+5] = i+1;
			}
		}

		m_mesh.triangles = newTriangles;
	}
	
	public void AddNormals () {
		m_mesh.RecalculateNormals();
		m_useNormals = true;
	}

	static Material defaultMaterial;
	static Camera cam;
	static Transform camTransform;
	static Camera cam3D;
	static Vector3 oldPosition;
	static Vector3 oldRotation;
	public static Vector3 camTransformPosition {
		get {return camTransform.position;}
	}
	public static bool camTransformExists {
		get {return camTransform != null;}
	}
	static int _vectorLayer = 31;
	public static int vectorLayer {
		get {
			return _vectorLayer;
		}
		set {
			_vectorLayer = value;
			if (_vectorLayer > 31) _vectorLayer = 31;
			else if (_vectorLayer < 0) _vectorLayer = 0;
		}
	}
	static int _vectorLayer3D = 0;
	public static int vectorLayer3D {
		get {
			return _vectorLayer3D;
		}
		set {
			_vectorLayer3D = value;
			if (_vectorLayer > 31) _vectorLayer3D = 31;
			else if (_vectorLayer < 0) _vectorLayer3D = 0;
		}
	}
	static float zDist;
	static bool useOrthoCam;
	const float cutoff = .15f;
	static bool error = false;
	static bool lineManagerCreated = false; 
	static LineManager _lineManager;
	public static LineManager lineManager {
		get {
			// This prevents OnDestroy functions that reference VectorManager from creating LineManager again when editor play mode is stopped
			// Checking _lineManager == null can randomly fail, since the order of objects being Destroyed is undefined
			if (!lineManagerCreated) {
				lineManagerCreated = true;
				var lineManagerGO = new GameObject("LineManager");
				_lineManager = lineManagerGO.AddComponent(typeof(LineManager)) as LineManager;
				_lineManager.enabled = false;
				MonoBehaviour.DontDestroyOnLoad(_lineManager);
			}
			return _lineManager;
		}
	}
	static int widthIdxAdd;
	
	static void LogError (string errorString) {
		Debug.LogError(errorString);
		error = true;
	}

	public static Camera SetCamera () {
		return SetCamera (CameraClearFlags.Depth, false);
	}
	
	public static Camera SetCamera (bool useOrtho) {
		return SetCamera (CameraClearFlags.Depth, useOrtho);
	}
	
	public static Camera SetCamera (CameraClearFlags clearFlags) {
		return SetCamera (clearFlags, false);
	}
	
	public static Camera SetCamera (CameraClearFlags clearFlags, bool useOrtho) {
		if (Camera.main == null) {
			LogError("VectorLine.SetCamera: no camera tagged \"Main Camera\" found");
			return null;
		}
		return SetCamera (Camera.main, clearFlags, useOrtho);
	}
	
	public static Camera SetCamera (Camera thisCamera) {
		return SetCamera (thisCamera, CameraClearFlags.Depth, false);
	}
	
	public static Camera SetCamera (Camera thisCamera, bool useOrtho) {
		return SetCamera (thisCamera, CameraClearFlags.Depth, useOrtho);
	}
	
	public static Camera SetCamera (Camera thisCamera, CameraClearFlags clearFlags) {
		return SetCamera (thisCamera, clearFlags, false);
	}
	
	public static Camera SetCamera (Camera thisCamera, CameraClearFlags clearFlags, bool useOrtho) {
		if (!cam) {
			cam = new GameObject("VectorCam", typeof(Camera)).camera;
			MonoBehaviour.DontDestroyOnLoad(cam);
		}
		cam.depth = thisCamera.depth+1;
		cam.clearFlags = clearFlags;
		cam.orthographic = useOrtho;
		useOrthoCam = useOrtho;
		if (useOrtho) {
			cam.orthographicSize = Screen.height/2;
			cam.farClipPlane = 101.1f;
			cam.nearClipPlane = .9f;
		}
		else {
			cam.fieldOfView = 90.0f;
			cam.farClipPlane = Screen.height/2 + .0101f;
			cam.nearClipPlane = Screen.height/2 - .0001f;
		}
		cam.transform.position = new Vector3(Screen.width/2 - .5f, Screen.height/2 - .5f, 0.0f);
		cam.transform.eulerAngles = Vector3.zero;
		cam.cullingMask = 1 << _vectorLayer;	// Turn on only the vector layer on the Vectrosity camera
		cam.backgroundColor = thisCamera.backgroundColor;
		
		thisCamera.cullingMask &= ~(1 << _vectorLayer);	// Turn off the vector layer on the non-Vectrosity camera
		camTransform = thisCamera.transform;
		cam3D = thisCamera;
		oldPosition = camTransform.position + Vector3.one;
		oldRotation = camTransform.eulerAngles + Vector3.one;
		return cam;
	}
	
	public static void SetCamera3D () {
		if (Camera.main == null) {
			LogError("VectorLine.SetCamera3D: no camera tagged \"Main Camera\" found. Please call SetCamera3D with a specific camera instead.");
			return;
		}
		SetCamera3D (Camera.main);
	}
	
	public static void SetCamera3D (Camera thisCamera) {
		camTransform = thisCamera.transform;
		cam3D = thisCamera;
		oldPosition = camTransform.position + Vector3.one;
		oldRotation = camTransform.eulerAngles + Vector3.one;
	}
	
	public static bool CameraHasMoved () {
		return oldPosition != camTransform.position || oldRotation != camTransform.eulerAngles;
	}
	
	public static void UpdateCameraInfo () {
		oldPosition = camTransform.position;
		oldRotation = camTransform.eulerAngles;	
	}
	
	public static Camera GetCamera () {
		return cam;
	}
	
	public static void SetVectorCamDepth (int depth) {
		cam.depth = depth;
	}
	
	public int GetSegmentNumber () {
		if (m_continuous) {
			return m_is2D? points2.Length-1 : points3.Length-1;
		}
		else {
			return m_is2D? points2.Length/2 : points3.Length/2;
		}
	}

	static string[] functionNames = {"VectorLine.SetColors: Length of color", "VectorLine.SetColorsSmooth: Length of color", "VectorLine.SetWidths: Length of line widths", "MakeCurve", "MakeSpline", "MakeEllipse"};
	enum FunctionName {SetColors, SetColorsSmooth, SetWidths, MakeCurve, MakeSpline, MakeEllipse}
	
	bool WrongArrayLength (int arrayLength, FunctionName functionName) {
		if (m_continuous) {
			if (arrayLength != m_pointsLength-1) {
				LogError(functionNames[(int)functionName] + " array for \"" + name + "\" must be length of points array minus one for a continuous line (one entry per line segment)");
				return true;
			}
		}
		else if (arrayLength != m_pointsLength/2) {
			LogError(functionNames[(int)functionName] + " array in \"" + name + "\" must be exactly half the length of points array for a discrete line (one entry per line segment)");
			return true;
		}
		return false;
	}
	
	bool CheckArrayLength (FunctionName functionName, int segments, int index) {
		if (segments < 1) {
			LogError("VectorLine." + functionNames[(int)functionName] + " needs at least 1 segment");
			return false;
		}

		if (m_isPoints) {
			if (index + segments > m_pointsLength) {
				if (index == 0) {
					LogError("VectorLine." + functionNames[(int)functionName] + ": The number of segments cannot exceed the number of points in the array for \"" + name + "\"");
					return false;
				}
				LogError("VectorLine: Calling " + functionNames[(int)functionName] + " with an index of " + index + " would exceed the length of the Vector array for \"" + name + "\"");
				return false;				
			}
			return true;
		}

		if (m_continuous) {
			if (index + (segments+1) > m_pointsLength) {
				if (index == 0) {
					LogError("VectorLine." + functionNames[(int)functionName] + ": The length of the array for continuous lines needs to be at least the number of segments plus one for \"" + name + "\"");
					return false;
				}
				LogError("VectorLine: Calling " + functionNames[(int)functionName] + " with an index of " + index + " would exceed the length of the Vector array for \"" + name + "\"");
				return false;
			}
		}
		else {
			if (index + segments*2 > m_pointsLength) {
				if (index == 0) {
					LogError("VectorLine." + functionNames[(int)functionName] + ": The length of the array for discrete lines needs to be at least twice the number of segments for \"" + name + "\"");
					return false;
				}
				LogError("VectorLine: Calling " + functionNames[(int)functionName] + " with an index of " + index + " would exceed the length of the Vector array for \"" + name + "\"");
				return false;
			}	
		}
		return true;	
	}
	
	public void SetColor (Color color) {
		int end = m_lineColors.Length;
		for (int i = 0; i < end; i++) {
			m_lineColors[i] = color;
		}
		m_mesh.colors = m_lineColors;
	}

	public void SetColors (Color[] lineColors) {
		if (lineColors == null) {
			LogError("VectorLine.SetColors: line colors array must not be null");
			return;
		}
		if (!m_isPoints) {
			if (WrongArrayLength (lineColors.Length, FunctionName.SetColors)) {
				return;
			}
		}
		else if (lineColors.Length != m_pointsLength) {
			LogError("VectorLine.SetColors: Length of lineColors array in \"" + name + "\" must be same length as points array");
			return;
		}
		
		int start = 0;
		int end = lineColors.Length;
		SetStartAndEnd (ref start, ref end);
		int idx = start*4;
		
		for (int i = start; i < end; i++) {
			m_lineColors[idx]   = lineColors[i];
			m_lineColors[idx+1] = lineColors[i];
			m_lineColors[idx+2] = lineColors[i];
			m_lineColors[idx+3] = lineColors[i];
			idx += 4;
		}
		m_mesh.colors = m_lineColors;
	}
	
	public void SetColorsSmooth (Color[] lineColors) {
		if (lineColors == null) {
			LogError("VectorLine.SetColors: line colors array must not be null");
			return;
		}
		if (m_isPoints) {
			LogError ("VectorLine.SetColorsSmooth must be used with a line rather than points");
			return;
		}
		if (WrongArrayLength (lineColors.Length, FunctionName.SetColorsSmooth)) {
			return;
		}
		
		int start = 0;
		int end = lineColors.Length;
		SetStartAndEnd (ref start, ref end);
		int idx = start*4;
		
		m_lineColors[idx  ] = lineColors[start];
		m_lineColors[idx+1] = lineColors[start];
		m_lineColors[idx+2] = lineColors[start];
		m_lineColors[idx+3] = lineColors[start];
		idx += 4;
		for (int i = start+1; i < end; i++) {
			m_lineColors[idx  ] = lineColors[i-1];
			m_lineColors[idx+1] = lineColors[i-1];
			m_lineColors[idx+2] = lineColors[i];
			m_lineColors[idx+3] = lineColors[i];
			idx += 4;
		}
		m_mesh.colors = m_lineColors;
	}

	void SetStartAndEnd (ref int start, ref int end) {
		start = (m_minDrawIndex == 0)? 0 : (m_continuous)? m_minDrawIndex : m_minDrawIndex/2;
		if (m_maxDrawIndex > 0) {
			if (m_continuous) {
				end = m_maxDrawIndex;
			}
			else {
				end = m_maxDrawIndex/2;
				if (m_maxDrawIndex%2 != 0) {
					end++;
				}
			}
		}
	}

	public void SetWidths (float[] lineWidths) {
		SetWidths (lineWidths, null, lineWidths.Length, true);
	}
	
	public void SetWidths (int[] lineWidths) {
		SetWidths (null, lineWidths, lineWidths.Length, false);
	}
	
	void SetWidths (float[] lineWidthsFloat, int[] lineWidthsInt, int arrayLength, bool doFloat) {
		if ((doFloat && lineWidthsFloat == null) || (!doFloat && lineWidthsInt == null)) {
			LogError("VectorLine.SetWidths: line widths array must not be null");
			return;
		}
		if (m_isPoints) {
			if (arrayLength != m_pointsLength) {
				LogError("VectorLine.SetWidths: line widths array must be the same length as the points array for \"" + name + "\"");
				return;
			}
		}
		else if (WrongArrayLength (arrayLength, FunctionName.SetWidths)) {
			return;
		}
		
		m_lineWidths = new float[arrayLength];
		if (doFloat) {
			for (int i = 0; i < arrayLength; i++) {
				m_lineWidths[i] = lineWidthsFloat[i] * .5f;
			}
		}
		else {
			for (int i = 0; i < arrayLength; i++) {
				m_lineWidths[i] = (float)lineWidthsInt[i] * .5f;
			}
		}
	}
	
	static Material defaultLineMaterial;
	static float defaultLineWidth;
	static int defaultLineDepth;
	static float defaultCapLength;
	static Color defaultLineColor;
	static LineType defaultLineType;
	static Joins defaultJoins;
	static bool defaultsSet = false;
	static Vector3 v1;
	static Vector3 v2;
	static Vector3 v3;
	
	public static void SetLineParameters (Color color, Material material, float width, float capLength, int depth, LineType lineType, Joins joins) {
		defaultLineColor = color;
		defaultLineMaterial = material;
		defaultLineWidth = width;
		defaultLineDepth = depth;
		defaultCapLength = capLength;
		defaultLineType = lineType;
		defaultJoins = joins;
		defaultsSet = true;
	}
	
	static void PrintMakeLineError () {
		LogError("VectorLine.MakeLine: Must call SetLineParameters before using MakeLine with these parameters");
	}
	
	public static VectorLine MakeLine (string name, Vector3[] points, Color[] colors) {
		if (!defaultsSet) {
			PrintMakeLineError();
			return null;
		}
		var line = new VectorLine(name, points, colors, defaultLineMaterial, defaultLineWidth, defaultLineType, defaultJoins);
		line.capLength = defaultCapLength;
		line.depth = defaultLineDepth;
		return line;
	}

	public static VectorLine MakeLine (string name, Vector2[] points, Color[] colors) {
		if (!defaultsSet) {
			PrintMakeLineError();
			return null;
		}
		var line = new VectorLine(name, points, colors, defaultLineMaterial, defaultLineWidth, defaultLineType, defaultJoins);
		line.capLength = defaultCapLength;
		line.depth = defaultLineDepth;
		return line;
	}

	public static VectorLine MakeLine (string name, Vector3[] points, Color color) {
		if (!defaultsSet) {
			PrintMakeLineError();
			return null;
		}
		var line = new VectorLine(name, points, color, defaultLineMaterial, defaultLineWidth, defaultLineType, defaultJoins);
		line.capLength = defaultCapLength;
		line.depth = defaultLineDepth;
		return line;
	}

	public static VectorLine MakeLine (string name, Vector2[] points, Color color) {
		if (!defaultsSet) {
			PrintMakeLineError();
			return null;
		}
		var line = new VectorLine(name, points, color, defaultLineMaterial, defaultLineWidth, defaultLineType, defaultJoins);
		line.capLength = defaultCapLength;
		line.depth = defaultLineDepth;
		return line;
	}

	public static VectorLine MakeLine (string name, Vector3[] points) {
		if (!defaultsSet) {
			PrintMakeLineError();
			return null;
		}
		var line = new VectorLine(name, points, defaultLineColor, defaultLineMaterial, defaultLineWidth, defaultLineType, defaultJoins);
		line.capLength = defaultCapLength;
		line.depth = defaultLineDepth;
		return line;
	}

	public static VectorLine MakeLine (string name, Vector2[] points) {
		if (!defaultsSet) {
			PrintMakeLineError();
			return null;
		}
		var line = new VectorLine(name, points, defaultLineColor, defaultLineMaterial, defaultLineWidth, defaultLineType, defaultJoins);
		line.capLength = defaultCapLength;
		line.depth = defaultLineDepth;
		return line;
	}

	public static VectorLine SetLine (Color color, params Vector2[] points) {
		return SetLine (color, 0.0f, points);
	}

	public static VectorLine SetLine (Color color, float time, params Vector2[] points) {
		if (points.Length < 2) {
			LogError("VectorLine.SetLine needs at least two points");
			return null;
		}
		var line = new VectorLine("Line", points, color, null, 1.0f, LineType.Continuous, Joins.None);
		if (time > 0.0f) {
			lineManager.DisableLine(line, time);
		}
		line.Draw();
		return line;
	}

	public static VectorLine SetLine (Color color, params Vector3[] points) {
		return SetLine (color, 0.0f, points);
	}
	
	public static VectorLine SetLine (Color color, float time, params Vector3[] points) {
		if (points.Length < 2) {
			LogError("VectorLine.SetLine needs at least two points");
			return null;
		}
		var line = new VectorLine("SetLine", points, color, null, 1.0f, LineType.Continuous, Joins.None);
		if (time > 0.0f) {
			lineManager.DisableLine(line, time);
		}
		line.Draw();
		return line;
	}

	public static VectorLine SetLine3D (Color color, params Vector3[] points) {
		return SetLine3D (color, 0.0f, points);
	}
		
	public static VectorLine SetLine3D (Color color, float time, params Vector3[] points) {
		if (points.Length < 2) {
			LogError("VectorLine.SetLine3D needs at least two points");
			return null;
		}
		var line = new VectorLine("SetLine3D", points, color, null, 1.0f, LineType.Continuous, Joins.None);
		line.Draw3DAuto (time);
		return line;
	}

	public static VectorLine SetRay (Color color, Vector3 origin, Vector3 direction) {
		return SetRay (color, 0.0f, origin, direction);
	}

	public static VectorLine SetRay (Color color, float time, Vector3 origin, Vector3 direction) {
		var line = new VectorLine("SetRay", new Vector3[] {origin, new Ray(origin, direction).GetPoint(direction.magnitude)}, color, null, 1.0f, LineType.Continuous, Joins.None);
		if (time > 0.0f) {
			lineManager.DisableLine(line, time);
		}
		line.Draw();
		return line;
	}

	public static VectorLine SetRay3D (Color color, Vector3 origin, Vector3 direction) {
		return SetRay3D (color, 0.0f, origin, direction);
	}

	public static VectorLine SetRay3D (Color color, float time, Vector3 origin, Vector3 direction) {
		var line = new VectorLine("SetRay3D", new Vector3[] {origin, new Ray(origin, direction).GetPoint(direction.magnitude)}, color, null, 1.0f, LineType.Continuous, Joins.None);
		line.Draw3DAuto (time);
		return line;
	}
	
	void CheckJoins () {
		if (m_joins != Joins.Fill) {
			if (m_triangleCount != m_vertexCount + m_vertexCount/2) {
				SetupTriangles();
			}
		}
		else {
			if (m_is2D) {
				if ( (points2[0] != points2[m_pointsLength-1] && m_triangleCount != m_vertexCount*3 - 6) ||
					 (points2[0] == points2[m_pointsLength-1] && m_triangleCount != m_vertexCount*3) ) {
					SetupTriangles();
				}
			}
			else {
				if ( (points3[0] != points3[m_pointsLength-1] && m_triangleCount != m_vertexCount*3 - 6) ||
					 (points3[0] == points3[m_pointsLength-1] && m_triangleCount != m_vertexCount*3) ) {
					SetupTriangles();
				}				
			}
		}
	}
	
	public void Draw () {
		Draw (null);
	}
	
	public void Draw (Transform thisTransform) {
		if (error || !m_active) return;
		if (!cam) {
			SetCamera();
			if (!cam) {	// If that didn't work (no camera tagged "Main Camera")
				LogError("VectorLine.Draw: You must call SetCamera before calling Draw for \"" + name + "\"");
				return;
			}
		}
		if ( (m_is2D && m_pointsLength != points2.Length) || (!m_is2D && m_pointsLength != points3.Length) ) {
			LogError("VectorLine.Draw: The points array for \"" + name + "\" must not be resized. Use Resize if you need to change the length of the points array");
			return;
		}
		if (m_isPoints) {
			DrawPoints (thisTransform);
			return;
		}
		
		if (smoothWidth && m_lineWidths.Length == 1 && m_pointsLength > 2) {
			LogError("VectorLine.Draw called with smooth line widths for \"" + name + "\", but VectorLine.SetWidths has not been used");
			return;
		}
	
		var useTransformMatrix = (thisTransform == null)? false : true;
		var thisMatrix = useTransformMatrix? thisTransform.localToWorldMatrix : Matrix4x4.identity;
		zDist = useOrthoCam? 101-m_depth : Screen.height/2 + ((100.0f - m_depth) * .0001f);
		
		int start, end = 0;
		SetupDrawStartEnd (out start, out end);
		
		if (m_is2D) {
			Line2D (start, end, thisMatrix, useTransformMatrix);
		}
		else {
			if (m_continuous) {
				Line3DContinuous (start, end, thisMatrix, useTransformMatrix);
			}
			else {
				Line3DDiscrete (start, end, thisMatrix, useTransformMatrix);
			}
		}
		
		m_mesh.vertices = m_lineVertices;
		CheckJoins();		
		if (m_mesh.bounds.center.x != Screen.width/2) {
			SetLineMeshBounds();
		}
	}

	void Line2D (int start, int end, Matrix4x4 thisMatrix, bool doTransform) {
		Vector3 p1, p2;
		int add, idx, widthIdx = 0;
		widthIdxAdd = 0;
		if (m_lineWidths.Length > 1) {
			widthIdx = start;
			widthIdxAdd = 1;
		}
		if (m_continuous) {
			idx = start*4;
			add = 1;
		}
		else {
			idx = start*2;
			add = 2;
			widthIdx /= 2;
		}
		
		if (capLength == 0.0f) {
			var perpendicular = new Vector3(0.0f, 0.0f, 0.0f);
			for (int i = start; i < end; i += add) {
				if (doTransform) {
					p1 = thisMatrix.MultiplyPoint3x4(points2[i]);
					p2 = thisMatrix.MultiplyPoint3x4(points2[i+1]);
				}
				else {
					p1 = points2[i];
					p2 = points2[i+1];
				}
				p1.z = zDist;
				if (p1.x == p2.x && p1.y == p2.y) {Skip (ref idx, ref widthIdx, ref p1); continue;}
				p2.z = zDist;
				
				v1.x = p2.y; v1.y = p1.x;
				v2.x = p1.y; v2.y = p2.x;
				perpendicular = v1 - v2;
				float normalizedDistance = ( 1.0f / Mathf.Sqrt((perpendicular.x * perpendicular.x) + (perpendicular.y * perpendicular.y)) );
				perpendicular *= normalizedDistance * m_lineWidths[widthIdx];
				m_lineVertices[idx]   = p1 - perpendicular;
				m_lineVertices[idx+1] = p1 + perpendicular;
				if (smoothWidth && i < end-add) {
					perpendicular = v1 - v2;
					perpendicular *= normalizedDistance * m_lineWidths[widthIdx+1];
				}
				m_lineVertices[idx+2] = p2 - perpendicular;
				m_lineVertices[idx+3] = p2 + perpendicular;
				idx += 4;
				widthIdx += widthIdxAdd;
			}
			if (m_joins == Joins.Weld) {
				if (m_continuous) {
					WeldJoins (start*4 + (start == 0? 4 : 0), end*4, Approximately2 (points2[0], points2[points2.Length-1])
						&& m_minDrawIndex == 0 && (m_maxDrawIndex == points2.Length-1 || m_maxDrawIndex == 0));
				}
				else {
					WeldJoinsDiscrete (start + 1, end, Approximately2 (points2[0], points2[points2.Length-1])
						&& m_minDrawIndex == 0 && (m_maxDrawIndex == points2.Length-1 || m_maxDrawIndex == 0));
				}
			}
		}
		else {
			var thisLine = new Vector3(0.0f, 0.0f, 0.0f);
			for (int i = m_minDrawIndex; i < end; i += add) {
				if (doTransform) {
					p1 = thisMatrix.MultiplyPoint3x4(points2[i]);
					p2 = thisMatrix.MultiplyPoint3x4(points2[i+1]);
				}
				else {
					p1 = points2[i];
					p2 = points2[i+1];
				}
				p1.z = zDist;
				if (p1.x == p2.x && p1.y == p2.y) {Skip (ref idx, ref widthIdx, ref p1); continue;}
				p2.z = zDist;
				
				thisLine = p2 - p1;
				thisLine *= ( 1.0f / Mathf.Sqrt((thisLine.x * thisLine.x) + (thisLine.y * thisLine.y)) );
				p1 -= thisLine * capLength;
				p2 += thisLine * capLength;
				
				v1.x = thisLine.y; v1.y = -thisLine.x;
				thisLine = v1 * m_lineWidths[widthIdx];
				m_lineVertices[idx]   = p1 - thisLine;
				m_lineVertices[idx+1] = p1 + thisLine;
				if (smoothWidth && i < end-add) {
					thisLine = v1 * m_lineWidths[widthIdx+1];
				}
				m_lineVertices[idx+2] = p2 - thisLine;
				m_lineVertices[idx+3] = p2 + thisLine;
				idx += 4;
				widthIdx += widthIdxAdd;
			}
		}
	}

	void Line3DContinuous (int start, int end, Matrix4x4 thisMatrix, bool doTransform) {
		Vector3 pos1, perpendicular;
		Vector3 pos2 = doTransform? cam3D.WorldToScreenPoint(thisMatrix.MultiplyPoint3x4(points3[start])) :
									cam3D.WorldToScreenPoint(points3[start]);
		pos2.z = pos2.z < cutoff? -zDist : zDist;
		float normalizedDistance = 0.0f;
		int widthIdx = 0;
		widthIdxAdd = 0;
		if (m_lineWidths.Length > 1) {
			widthIdx = start;
			widthIdxAdd = 1;
		}
		int idx = start*4;
		
		for (int i = start; i < end; i++) {
			pos1 = pos2;
			pos2 = doTransform? cam3D.WorldToScreenPoint(thisMatrix.MultiplyPoint3x4(points3[i+1])) :
								cam3D.WorldToScreenPoint(points3[i+1]);
			if (pos1.x == pos2.x && pos1.y == pos2.y) {Skip (ref idx, ref widthIdx, ref pos1); continue;}
			pos2.z = pos2.z < cutoff? -zDist : zDist;
			
			v1.x = pos2.y; v1.y = pos1.x;
			v2.x = pos1.y; v2.y = pos2.x;
			perpendicular = v1 - v2;
			normalizedDistance = 1.0f / Mathf.Sqrt((perpendicular.x * perpendicular.x) + (perpendicular.y * perpendicular.y));
			perpendicular *= normalizedDistance * m_lineWidths[widthIdx];
			m_lineVertices[idx]   = pos1 - perpendicular;
			m_lineVertices[idx+1] = pos1 + perpendicular;
			if (smoothWidth && i < end-1) {
				perpendicular = v1 - v2;
				perpendicular *= normalizedDistance * m_lineWidths[widthIdx+1];
			}
			m_lineVertices[idx+2] = pos2 - perpendicular;
			m_lineVertices[idx+3] = pos2 + perpendicular;
			idx += 4;
			widthIdx += widthIdxAdd;
		}
		
		if (m_joins == Joins.Weld) {
			WeldJoins (start*4 + 4, end*4, Approximately3 (points3[0], points3[points3.Length-1])
				&& m_minDrawIndex == 0 && (m_maxDrawIndex == points3.Length-1 || m_maxDrawIndex == 0));
		}
	}

	void Line3DDiscrete (int start, int end, Matrix4x4 thisMatrix, bool doTransform) {
		Vector3 pos1, pos2, perpendicular;
		float normalizedDistance = 0.0f;
		int widthIdx = 0;
		widthIdxAdd = 0;
		if (m_lineWidths.Length > 1) {
			widthIdx = start;
			widthIdxAdd = 1;
		}
		int idx = start*2;
		
		for (int i = start; i < end; i += 2) {
			if (doTransform) {
				pos1 = cam3D.WorldToScreenPoint(thisMatrix.MultiplyPoint3x4(points3[i]));
				pos2 = cam3D.WorldToScreenPoint(thisMatrix.MultiplyPoint3x4(points3[i+1]));
			}
			else {
				pos1 = cam3D.WorldToScreenPoint(points3[i]);
				pos2 = cam3D.WorldToScreenPoint(points3[i+1]);
			}
			pos1.z = pos1.z < cutoff? -zDist : zDist;
			if (pos1.x == pos2.x && pos1.y == pos2.y) {Skip (ref idx, ref widthIdx, ref pos1); continue;}
			pos2.z = pos2.z < cutoff? -zDist : zDist;
			
			v1.x = pos2.y; v1.y = pos1.x;
			v2.x = pos1.y; v2.y = pos2.x;
			perpendicular = v1 - v2;
			normalizedDistance = 1.0f / Mathf.Sqrt((perpendicular.x * perpendicular.x) + (perpendicular.y * perpendicular.y));
			perpendicular *= normalizedDistance * m_lineWidths[widthIdx];
			m_lineVertices[idx]   = pos1 - perpendicular;
			m_lineVertices[idx+1] = pos1 + perpendicular;
			if (smoothWidth && i < end-2) {
				perpendicular = v1 - v2;
				perpendicular *= normalizedDistance * m_lineWidths[widthIdx+1];
			}
			m_lineVertices[idx+2] = pos2 - perpendicular;
			m_lineVertices[idx+3] = pos2 + perpendicular;
			idx += 4;
			widthIdx += widthIdxAdd;
		}
		
		if (m_joins == Joins.Weld) {
			WeldJoinsDiscrete (start + 1, end, Approximately3 (points3[0], points3[points3.Length-1])
				&& m_minDrawIndex == 0 && (m_maxDrawIndex == points3.Length-1 || m_maxDrawIndex == 0));
		}
	}

	void SetLineMeshBounds () {
		var bounds = new Bounds();
		if (!useOrthoCam) {
			bounds.center = new Vector3(Screen.width/2, Screen.height/2, Screen.height/2);
			bounds.extents = new Vector3(Screen.width*100, Screen.height*100, .1f);
		}
		else {
			bounds.center = new Vector3(Screen.width/2, Screen.height/2, 50.5f);
			bounds.extents = new Vector3(Screen.width*100, Screen.height*100, 51.0f);
		}
		m_mesh.bounds = bounds;
	}
	
	void SetupDrawStartEnd (out int start, out int end) {
		start = m_minDrawIndex;
		end = (m_maxDrawIndex == 0)? m_pointsLength-1 : m_maxDrawIndex;
		if (m_drawStart > 0) {
			start = m_drawStart;
			ZeroVertices (0, m_drawStart);
		}
		if (m_drawEnd < m_pointsLength) {
			end = m_drawEnd;
			ZeroVertices (m_drawEnd, m_pointsLength);
		}
	}

	public void Draw3D () {
		Draw3D (null);
	}

	public void Draw3D (Transform thisTransform) {
		if (error || !m_active) return;
		if (!cam3D) {
			SetCamera3D();
			if (!cam3D) {
				LogError("VectorLine.Draw3D: You must call SetCamera or SetCamera3D before calling Draw3D for \"" + name + "\"");
				return;
			}
		}
		if (m_is2D) {
			LogError("VectorLine.Draw3D can only be used with a Vector3 array, which \"" + name + "\" doesn't have");
			return;
		}
		if (m_pointsLength != points3.Length) {
			LogError("The points array for \"" + name + "\" must not be resized. Use Resize if you need to change the length of the points array");
			return;
		}
		if (m_isPoints) {
			DrawPoints3D (thisTransform);
			return;
		}
		
		if (smoothWidth && m_lineWidths.Length == 1 && m_pointsLength > 2) {
			LogError("VectorLine.Draw3D called with smooth line widths for \"" + name + "\", but VectorLine.SetWidths has not been used");
			return;
		}
		
		if (layer == -1) {
			vectorObject.layer = _vectorLayer3D;
			layer = _vectorLayer3D;
		}
		
		int start, end, idx, add, widthIdx = 0;
		SetupDrawStartEnd (out start, out end);
		
		var useTransformMatrix = (thisTransform == null)? false : true;
		var thisMatrix = useTransformMatrix? thisTransform.localToWorldMatrix : Matrix4x4.identity;
		widthIdxAdd = 0;
		if (m_lineWidths.Length > 1) {
			widthIdx = start;
			widthIdxAdd = 1;
		}
		if (m_continuous) {
			idx = start*4;
			add = 1;
		}
		else {
			idx = start*2;
			widthIdx /= 2;
			add = 2;
		}
		Vector3 pos1, pos2, thisLine, perpendicular;
		
		for (int i = start; i < end; i += add) {
			if (useTransformMatrix) {
				pos1 = cam3D.WorldToScreenPoint(thisMatrix.MultiplyPoint3x4(points3[i]));
				pos2 = cam3D.WorldToScreenPoint(thisMatrix.MultiplyPoint3x4(points3[i+1]));
			}
			else {
				pos1 = cam3D.WorldToScreenPoint(points3[i]);
				pos2 = cam3D.WorldToScreenPoint(points3[i+1]);
			}
			
			v1.x = pos2.y; v1.y = pos1.x;
			v2.x = pos1.y; v2.y = pos2.x;
			thisLine = (v1 - v2).normalized;
			perpendicular = thisLine * m_lineWidths[widthIdx];
			
			m_screenPoints[idx]   = pos1 - perpendicular;	// Used for Joins.Weld
			m_screenPoints[idx+1] = pos1 + perpendicular;
			m_lineVertices[idx]   = cam3D.ScreenToWorldPoint(m_screenPoints[idx]);
			m_lineVertices[idx+1] = cam3D.ScreenToWorldPoint(m_screenPoints[idx+1]);
			
			if (smoothWidth && i < end-add) {
				perpendicular = thisLine * m_lineWidths[widthIdx+1];
			}
			m_screenPoints[idx+2] = pos2 - perpendicular;
			m_screenPoints[idx+3] = pos2 + perpendicular;
			m_lineVertices[idx+2] = cam3D.ScreenToWorldPoint(m_screenPoints[idx+2]);
			m_lineVertices[idx+3] = cam3D.ScreenToWorldPoint(m_screenPoints[idx+3]);
			
			idx += 4;
			widthIdx += widthIdxAdd;
		}
		
		if (m_joins == Joins.Weld) {
			if (m_continuous) {
				WeldJoins3D (start*4 + 4, end*4, Approximately3 (points3[0], points3[m_pointsLength-1])
					&& m_minDrawIndex == 0 && (m_maxDrawIndex == points3.Length-1 || m_maxDrawIndex == 0));
			}
			else {
				WeldJoinsDiscrete3D (start + 1, end, Approximately3 (points3[0], points3[m_pointsLength-1])
					&& m_minDrawIndex == 0 && (m_maxDrawIndex == points3.Length-1 || m_maxDrawIndex == 0));
			}
		}
		
		m_mesh.vertices = m_lineVertices;
		CheckJoins();
		m_mesh.RecalculateBounds();
	}

	public void DrawViewport () {
		DrawViewport (null);
	}

	public void DrawViewport (Transform thisTransform) {
		if (error || !m_active) return;
		if (!cam) {
			SetCamera();
			if (!cam) {	// If that didn't work (no camera tagged "Main Camera")
				LogError("VectorLine.DrawViewport: You must call SetCamera before calling DrawViewport for \"" + name + "\"");
				return;
			}
		}
		if (m_isPoints) {
			LogError("VectorLine.DrawViewport can't be used with VectorPoints");
			return;
		}
		if (!m_is2D) {
			LogError("VectorLine.DrawViewport can only be used with a Vector2 array, which \"" + name + "\" doesn't have");
			return;
		}
		if (smoothWidth && m_lineWidths.Length == 1 && m_pointsLength > 2) {
			LogError("VectorLine.DrawViewport called with smooth line widths for \"" + name + "\", but SetWidths has not been used");
			return;
		}
		if (m_pointsLength != points2.Length) {
			LogError("The points array for \"" + name + "\" must not be resized. Use Resize if you need to change the length of the points array");
			return;
		}
		
		var useTransformMatrix = (thisTransform == null)? false : true;
		var thisMatrix = useTransformMatrix? thisTransform.localToWorldMatrix : Matrix4x4.identity;
		zDist = useOrthoCam? 101-m_depth : Screen.height/2 + ((100.0f - m_depth) * .0001f);
		
		Vector3 p1, p2;
		int idx, add, start, end, widthIdx = 0;
		widthIdxAdd = 0;
		SetupDrawStartEnd (out start, out end);
		
		if (m_lineWidths.Length > 1) {
			widthIdx = start;
			widthIdxAdd = 1;
		}
		if (m_continuous) {
			idx = start*4;
			add = 1;
		}
		else {
			idx = start*2;
			widthIdx /= 2;
			add = 2;
		}
		int screenWidth = Screen.width;
		int screenHeight = Screen.height;
		
		if (capLength == 0.0f) {
			Vector3 perpendicular;
			for (int i = start; i < end; i += add) {
				if (useTransformMatrix) {
					p1 = thisMatrix.MultiplyPoint3x4(points2[i]);
					p2 = thisMatrix.MultiplyPoint3x4(points2[i+1]);
				}
				else {
					p1 = points2[i];
					p2 = points2[i+1];
				}
				p1.z = zDist;
				if (p1.x == p2.x && p1.y == p2.y) {Skip (ref idx, ref widthIdx, ref p1); continue;}
				p2.z = zDist;
				p1.x *= screenWidth; p1.y *= screenHeight;
				p2.x *= screenWidth; p2.y *= screenHeight;
				
				v1.x = p2.y * screenWidth; v1.y = p1.x * screenHeight;
				v2.x = p1.y * screenWidth; v2.y = p2.x * screenHeight;
				perpendicular = v1 - v2;
				float normalizedDistance = ( 1.0f / Mathf.Sqrt((perpendicular.x * perpendicular.x) + (perpendicular.y * perpendicular.y)) );
				perpendicular *= normalizedDistance * m_lineWidths[widthIdx];
				m_lineVertices[idx]   = p1 - perpendicular;
				m_lineVertices[idx+1] = p1 + perpendicular;
				if (smoothWidth && i < end-add) {
					perpendicular = v1 - v2;
					perpendicular *= normalizedDistance * m_lineWidths[widthIdx+1];
				}
				m_lineVertices[idx+2] = p2 - perpendicular;
				m_lineVertices[idx+3] = p2 + perpendicular;
				idx += 4;
				widthIdx += widthIdxAdd;
			}
			if (m_joins == Joins.Weld) {
				if (m_continuous) {
					WeldJoins (start*4 + 4, end*4, Approximately2 (points2[0], points2[m_pointsLength-1])
						&& m_minDrawIndex == 0 && (m_maxDrawIndex == points2.Length-1 || m_maxDrawIndex == 0));
				}
				else {
					WeldJoinsDiscrete (start + 1, end, Approximately2 (points2[0], points2[m_pointsLength-1])
						&& m_minDrawIndex == 0 && (m_maxDrawIndex == points2.Length-1 || m_maxDrawIndex == 0));
				}
			}
		}
		else {
			Vector3 thisLine;
			for (int i = m_minDrawIndex; i < end; i += add) {
				if (useTransformMatrix) {
					p1 = thisMatrix.MultiplyPoint3x4(points2[i]);
					p2 = thisMatrix.MultiplyPoint3x4(points2[i+1]);
				}
				else {
					p1 = points2[i];
					p2 = points2[i+1];
				}
				p1.z = zDist;
				if (p1.x == p2.x && p1.y == p2.y) {Skip (ref idx, ref widthIdx, ref p1); continue;}
				p2.z = zDist;
				p1.x *= screenWidth; p1.y *= screenHeight;
				p2.x *= screenWidth; p2.y *= screenHeight;
				
				thisLine = p2 - p1;
				thisLine *= ( 1.0f / Mathf.Sqrt((thisLine.x * thisLine.x) + (thisLine.y * thisLine.y)) );
				p1 -= thisLine * capLength;
				p2 += thisLine * capLength;
				
				v1.x = thisLine.y; v1.y = -thisLine.x;
				thisLine = v1 * m_lineWidths[widthIdx];
				m_lineVertices[idx]   = p1 - thisLine;
				m_lineVertices[idx+1] = p1 + thisLine;
				if (smoothWidth && i < end-add) {
					thisLine = v1 * m_lineWidths[widthIdx+1];
				}
				m_lineVertices[idx+2] = p2 - thisLine;
				m_lineVertices[idx+3] = p2 + thisLine;
				idx += 4;
				widthIdx += widthIdxAdd;
			}
		}
		
		m_mesh.vertices = m_lineVertices;
		CheckJoins();
		if (m_mesh.bounds.center.x != Screen.width/2) {
			SetLineMeshBounds();
		}		
	}
	
	public static void LineManagerCheckDistance () {
		lineManager.StartCheckDistance();
	}
	
	public static void LineManagerDisable () {
		lineManager.DisableIfUnused();
	}
	
	public static void LineManagerEnable () {
		lineManager.EnableIfUsed();
	}

	public void Draw3DAuto () {
		Draw3DAuto (0.0f, null);
	}

	public void Draw3DAuto (float time) {
		Draw3DAuto (time, null);
	}

	public void Draw3DAuto (Transform thisTransform) {
		Draw3DAuto (0.0f, thisTransform);
	}
	
	public void Draw3DAuto (float time, Transform thisTransform) {
		if (time < 0.0f) time = 0.0f;
		lineManager.AddLine (this, thisTransform, time);
		Draw3D (thisTransform);
	}
	
	public void StopDrawing3DAuto () {
		lineManager.RemoveLine (this);
	}
	
	void WeldJoins (int start, int end, bool connectFirstAndLast) {
		if (connectFirstAndLast) {
			int lineLength = m_lineVertices.Length;
			SetIntersectionPoint (lineLength-4, lineLength-2, 0, 2);
			SetIntersectionPoint (lineLength-3, lineLength-1, 1, 3);
		}
		for (int i = start; i < end; i+= 4) {
			SetIntersectionPoint (i-4, i-2, i, i+2);
			SetIntersectionPoint (i-3, i-1, i+1, i+3);
		}
	}

	void WeldJoinsDiscrete (int start, int end, bool connectFirstAndLast) {
		if (connectFirstAndLast) {
			var lineLength = m_lineVertices.Length;
			SetIntersectionPoint (lineLength-4, lineLength-2, 0, 2);
			SetIntersectionPoint (lineLength-3, lineLength-1, 1, 3);
		}
		int idx = (start+1) / 2 * 4;
		if (m_is2D) {
			for (int i = start; i < end; i+= 2) {
				if (points2[i] == points2[i+1]) {
					SetIntersectionPoint (idx-4, idx-2, idx,   idx+2);
					SetIntersectionPoint (idx-3, idx-1, idx+1, idx+3);
				}
				idx += 4;
			}
		}
		else {
			for (int i = start; i < end; i+= 2) {
				if (points3[i] == points3[i+1]) {
					SetIntersectionPoint (idx-4, idx-2, idx,   idx+2);
					SetIntersectionPoint (idx-3, idx-1, idx+1, idx+3);
				}
				idx += 4;
			}
		}
	}
	
	void SetIntersectionPoint (int p1, int p2, int p3, int p4) {
		var l1a = m_lineVertices[p1]; var l1b = m_lineVertices[p2];
		var l2a = m_lineVertices[p3]; var l2b = m_lineVertices[p4];
		float d = (l2b.y - l2a.y)*(l1b.x - l1a.x) - (l2b.x - l2a.x)*(l1b.y - l1a.y);
		if (d == 0.0f) return;	// Parallel lines
		float n = ( (l2b.x - l2a.x)*(l1a.y - l2a.y) - (l2b.y - l2a.y)*(l1a.x - l2a.x) ) / d;
		
		v3.x = l1a.x + (n * (l1b.x - l1a.x));
		v3.y = l1a.y + (n * (l1b.y - l1a.y));
		v3.z = l1a.z;
		if ((v3 - l1b).sqrMagnitude > m_maxWeldDistance) return;
		m_lineVertices[p2] = v3;
		m_lineVertices[p3] = v3;
	}

	void WeldJoins3D (int start, int end, bool connectFirstAndLast) {
		if (connectFirstAndLast) {
			var lineLength = m_lineVertices.Length;
			SetIntersectionPoint3D (lineLength-4, lineLength-2, 0, 2);
			SetIntersectionPoint3D (lineLength-3, lineLength-1, 1, 3);
		}
		for (int i = start; i < end; i+= 4) {
			SetIntersectionPoint3D (i-4, i-2, i, i+2);
			SetIntersectionPoint3D (i-3, i-1, i+1, i+3);
		}
	}

	void WeldJoinsDiscrete3D (int start, int end, bool connectFirstAndLast) {
		if (connectFirstAndLast) {
			var lineLength = m_lineVertices.Length;
			SetIntersectionPoint3D (lineLength-4, lineLength-2, 0, 2);
			SetIntersectionPoint3D (lineLength-3, lineLength-1, 1, 3);
		}
		int idx = (start+1) / 2 * 4;
		for (int i = start; i < end; i+= 2) {
			if (points3[i] == points3[i+1]) {
				SetIntersectionPoint3D (idx-4, idx-2, idx,   idx+2);
				SetIntersectionPoint3D (idx-3, idx-1, idx+1, idx+3);
			}
			idx += 4;
		}
	}

	void SetIntersectionPoint3D (int p1, int p2, int p3, int p4) {
		var l1a = m_screenPoints[p1]; var l1b = m_screenPoints[p2];
		var l2a = m_screenPoints[p3]; var l2b = m_screenPoints[p4];
		float d = (l2b.y - l2a.y)*(l1b.x - l1a.x) - (l2b.x - l2a.x)*(l1b.y - l1a.y);
		if (d == 0.0f) return;	// Parallel lines
		float n = ( (l2b.x - l2a.x)*(l1a.y - l2a.y) - (l2b.y - l2a.y)*(l1a.x - l2a.x) ) / d;
		
		v3.x = l1a.x + (n * (l1b.x - l1a.x));
		v3.y = l1a.y + (n * (l1b.y - l1a.y));
		v3.z = l1a.z;
		if ((v3 - l1b).sqrMagnitude > m_maxWeldDistance) return;
		m_lineVertices[p2] = cam3D.ScreenToWorldPoint(v3);
		m_lineVertices[p3] = m_lineVertices[p2];
	}
	
	public void SetTextureScale (float textureScale) {
		SetTextureScale (null, textureScale, 0.0f);
	}

	public void SetTextureScale (Transform thisTransform, float textureScale) {
		SetTextureScale (thisTransform, textureScale, 0.0f);
	}

	public void SetTextureScale (float textureScale, float offset) {
		SetTextureScale (null, textureScale, offset);
	}
	
	public void SetTextureScale (Transform thisTransform, float textureScale, float offset) {
		int end = m_continuous? m_pointsLength-1 : m_pointsLength;
		int add = m_continuous? 1 : 2;
		int idx = 0;
		int widthIdx = 0;
		widthIdxAdd = m_lineWidths.Length == 1? 0 : 1;
		float thisScale = 1.0f / textureScale;
		
		if (m_is2D) {
			for (int i = 0; i < end; i += add) {
				float xPos = thisScale / (m_lineWidths[widthIdx]*2 / (points2[i] - points2[i+1]).magnitude);
				m_lineUVs[idx++].x = offset;
				m_lineUVs[idx++].x = offset;
				m_lineUVs[idx++].x = xPos + offset;
				m_lineUVs[idx++].x = xPos + offset;
				offset = (offset + xPos) % 1;
				widthIdx += widthIdxAdd;
			}
		}
		else {
			if (!cam3D) {
				SetCamera3D();
				if (!cam3D) {
					LogError("VectorLine.SetTextureScale: You must call SetCamera3D before calling SetTextureScale");
					return;
				}
			}
			
			var useTransformMatrix = (thisTransform == null)? false : true;
			var thisMatrix = useTransformMatrix? thisTransform.localToWorldMatrix : Matrix4x4.identity;
			var p1 = Vector2.zero;
			var p2 = Vector2.zero;
			for (int i = 0; i < end; i += add) {
				if (useTransformMatrix) {
					p1 = cam3D.WorldToScreenPoint(thisMatrix.MultiplyPoint3x4(points3[i]));
					p2 = cam3D.WorldToScreenPoint(thisMatrix.MultiplyPoint3x4(points3[i+1]));					
				}
				else {
					p1 = cam3D.WorldToScreenPoint(points3[i]);
					p2 = cam3D.WorldToScreenPoint(points3[i+1]);
				}
				float xPos = thisScale / (m_lineWidths[widthIdx]*2 / (p1 - p2).magnitude);
				m_lineUVs[idx++].x = offset;
				m_lineUVs[idx++].x = offset;
				m_lineUVs[idx++].x = xPos + offset;
				m_lineUVs[idx++].x = xPos + offset;
				offset = (offset + xPos) % 1;
				widthIdx += widthIdxAdd;
			}
		}
		
		m_mesh.uv = m_lineUVs;
	}

	public void ResetTextureScale () {
		int end = m_lineUVs.Length;
		
		for (int i = 0; i < end; i += 4) {
			m_lineUVs[i  ].x = 0.0f;
			m_lineUVs[i+1].x = 0.0f;
			m_lineUVs[i+2].x = 1.0f;
			m_lineUVs[i+3].x = 1.0f;
		}
		
		m_mesh.uv = m_lineUVs;
	}
	
	void DrawPoints () {
		DrawPoints (null);
	}
	
	void DrawPoints (Transform thisTransform) {	
		var useTransformMatrix = (thisTransform == null)? false : true;
		var thisMatrix = useTransformMatrix? thisTransform.localToWorldMatrix : Matrix4x4.identity;
		zDist = useOrthoCam? 101-m_depth : Screen.height/2 + ((100.0f - m_depth) * .0001f);

		int start, end, widthIdx = 0;
		SetupDrawStartEnd (out start, out end);
		int idx = start*4;
		widthIdxAdd = 0;
		if (m_lineWidths.Length > 1) {
			widthIdx = start;
			widthIdxAdd = 1;
		}
		Vector3 pos1;

		if (!m_is2D) {
			for (int i = start; i <= end; i++) {
				pos1 = useTransformMatrix? cam3D.WorldToScreenPoint(thisMatrix.MultiplyPoint3x4(points3[i])) :
										   cam3D.WorldToScreenPoint(points3[i]);
				if (pos1.z < cutoff) {
					Skip (ref idx, ref widthIdx, ref pos1);
					continue;
				}
				pos1.z = zDist;
				v1.x = v1.y = v2.y = m_lineWidths[widthIdx];
				v2.x = -m_lineWidths[widthIdx];				

				m_lineVertices[idx]   = pos1 + v2;
				m_lineVertices[idx+1] = pos1 - v1;
				m_lineVertices[idx+2] = pos1 + v1;
				m_lineVertices[idx+3] = pos1 - v2;
				idx += 4;
				widthIdx += widthIdxAdd;
			}
		}
		else {
			for (int i = start; i <= end; i++) {
				pos1 = useTransformMatrix? thisMatrix.MultiplyPoint3x4(points2[i]) : (Vector3)points2[i];
				pos1.z = zDist;
				v1.x = v1.y = v2.y = m_lineWidths[widthIdx];
				v2.x = -m_lineWidths[widthIdx];
	
				m_lineVertices[idx]   = pos1 + v2;
				m_lineVertices[idx+1] = pos1 - v1;
				m_lineVertices[idx+2] = pos1 + v1;
				m_lineVertices[idx+3] = pos1 - v2;
				idx += 4;
				widthIdx += widthIdxAdd;
			}
		}
		
		m_mesh.vertices = m_lineVertices;
		if (m_mesh.bounds.center.x != Screen.width/2) {
			SetLineMeshBounds();
		}
	}

	void DrawPoints3D () {
		DrawPoints3D (null);
	}

	void DrawPoints3D (Transform thisTransform) {
		if (layer == -1) {
			vectorObject.layer = _vectorLayer3D;
			layer = _vectorLayer3D;
		}
		var useTransformMatrix = (thisTransform == null)? false : true;
		var thisMatrix = useTransformMatrix? thisTransform.localToWorldMatrix : Matrix4x4.identity;

		int idx = m_minDrawIndex*4;
		int start, end, widthIdx = 0;
		SetupDrawStartEnd (out start, out end);
		widthIdxAdd = 0;
		if (m_lineWidths.Length > 1) {
			widthIdx = start;
			widthIdxAdd = 1;
		}
		Vector3 pos1;
		
		for (int i = start; i <= end; i++) {
			pos1 = useTransformMatrix? cam3D.WorldToScreenPoint(thisMatrix.MultiplyPoint3x4(points3[i])) :
									   cam3D.WorldToScreenPoint(points3[i]);
			if (pos1.z < cutoff) {
				pos1 = Vector3.zero;
				Skip (ref idx, ref widthIdx, ref pos1);
				continue;
			}
			v1.x = v1.y = v2.y = m_lineWidths[widthIdx];
			v2.x = -m_lineWidths[widthIdx];

			m_lineVertices[idx]   = cam3D.ScreenToWorldPoint(pos1 + v2);
			m_lineVertices[idx+1] = cam3D.ScreenToWorldPoint(pos1 - v1);
			m_lineVertices[idx+2] = cam3D.ScreenToWorldPoint(pos1 + v1);
			m_lineVertices[idx+3] = cam3D.ScreenToWorldPoint(pos1 - v2);
			idx += 4;
			widthIdx += widthIdxAdd;
		}
		m_mesh.vertices = m_lineVertices;
		m_mesh.RecalculateBounds();
	}

	void Skip (ref int idx, ref int widthIdx, ref Vector3 pos) {
		m_lineVertices[idx  ] = pos;
		m_lineVertices[idx+1] = pos;
		m_lineVertices[idx+2] = pos;
		m_lineVertices[idx+3] = pos;
		idx += 4;
		widthIdx += widthIdxAdd;
	}
	
	public static void SetDepth (Transform thisTransform, int depth) {
		depth = Mathf.Clamp(depth, 0, 100);
		thisTransform.position = new Vector3(thisTransform.position.x,
											 thisTransform.position.y,
											 useOrthoCam? 101-depth : Screen.height/2 + ((100.0f - depth) * .0001f));		
	}
	
	static int endianDiff1;
	static int endianDiff2;
	static byte[] byteBlock;
	
	public static Vector3[] BytesToVector3Array (byte[] lineBytes) {
		if (lineBytes.Length % 12 != 0) {
			LogError("VectorLine.BytesToVector3Array: Incorrect input byte length...must be a multiple of 12");
			return null;
		}
		
		SetupByteBlock();
		Vector3[] points = new Vector3[lineBytes.Length/12];
		int idx = 0;
		for (int i = 0; i < lineBytes.Length; i += 12) {
			points[idx++] = new Vector3( ConvertToFloat (lineBytes, i),
										 ConvertToFloat (lineBytes, i+4),
										 ConvertToFloat (lineBytes, i+8) );
		}
		return points;
	}
	
	public static Vector2[] BytesToVector2Array (byte[] lineBytes) {
		if (lineBytes.Length % 8 != 0) {
			LogError("VectorLine.BytesToVector2Array: Incorrect input byte length...must be a multiple of 8");
			return null;
		}
		
		SetupByteBlock();
		Vector2[] points = new Vector2[lineBytes.Length/8];
		int idx = 0;
		for (int i = 0; i < lineBytes.Length; i += 8) {
			points[idx++] = new Vector2( ConvertToFloat (lineBytes, i),
										 ConvertToFloat (lineBytes, i+4));
		}
		return points;
	}
	
	static void SetupByteBlock () {
		if (byteBlock == null) {byteBlock = new byte[4];}
		if (System.BitConverter.IsLittleEndian) {endianDiff1 = 0; endianDiff2 = 0;}
		else {endianDiff1 = 3; endianDiff2 = 1;}	
	}
	
	// Unfortunately we can't just use System.BitConverter.ToSingle as-is...we need a function to handle both big-endian and little-endian systems
	static float ConvertToFloat (byte[] bytes, int i) {
		byteBlock[    endianDiff1] = bytes[i];
		byteBlock[1 + endianDiff2] = bytes[i+1];
		byteBlock[2 - endianDiff2] = bytes[i+2];
		byteBlock[3 - endianDiff1] = bytes[i+3];
		return System.BitConverter.ToSingle (byteBlock, 0);
	}
	
	public static void Destroy (ref VectorLine line) {
		if (line != null) {
			Object.Destroy (line.m_mesh);
			Object.Destroy (line.m_meshFilter);
			Object.Destroy (line.vectorObject);
			line = null;
		}
	}

	public static void Destroy (ref VectorPoints line) {
		if (line != null) {
			Object.Destroy (line.m_mesh);
			Object.Destroy (line.m_meshFilter);
			Object.Destroy (line.vectorObject);
			line = null;
		}
	}
	
	public static void Destroy (ref VectorLine line, GameObject go) {
		Destroy (ref line);
		if (go != null) {
			Object.Destroy (go);
		}
	}

	public static void Destroy (ref VectorPoints line, GameObject go) {
		Destroy (ref line);
		if (go != null) {
			Object.Destroy (go);
		}
	}

	public void MakeRect (Rect rect) {
		MakeRect (new Vector2(rect.x, rect.y), new Vector2(rect.x+rect.width, rect.y-rect.height), 0);
	}

	public void MakeRect (Rect rect, int index) {
		MakeRect (new Vector2(rect.x, rect.y), new Vector2(rect.x+rect.width, rect.y-rect.height), index);
	}

	public void MakeRect (Vector3 topLeft, Vector3 bottomRight) {
		MakeRect (topLeft, bottomRight, 0);
	}

	public void MakeRect (Vector3 topLeft, Vector3 bottomRight, int index) {
		if (m_continuous) {
			if (index + 5 > m_pointsLength) {
				if (index == 0) {
					LogError("VectorLine.MakeRect: The length of the array for continuous lines needs to be at least 5 for \"" + name + "\"");
					return;
				}
				LogError("Calling VectorLine.MakeRect with an index of " + index + " would exceed the length of the Vector2 array for \"" + name + "\"");
				return;
			}
			if (m_is2D) {
				points2[index  ] = new Vector2(topLeft.x,     topLeft.y);
				points2[index+1] = new Vector2(bottomRight.x, topLeft.y);
				points2[index+2] = new Vector2(bottomRight.x, bottomRight.y);
				points2[index+3] = new Vector2(topLeft.x,	   bottomRight.y);
				points2[index+4] = new Vector2(topLeft.x,     topLeft.y);
			}
			else {
				points3[index  ] = new Vector3(topLeft.x,     topLeft.y, 	  topLeft.z);
				points3[index+1] = new Vector3(bottomRight.x, topLeft.y, 	  topLeft.z);
				points3[index+2] = new Vector3(bottomRight.x, bottomRight.y, bottomRight.z);
				points3[index+3] = new Vector3(topLeft.x,	   bottomRight.y, bottomRight.z);
				points3[index+4] = new Vector3(topLeft.x,     topLeft.y, 	  topLeft.z);
			}
		}
		
		else {
			if (index + 8 > m_pointsLength) {
				if (index == 0) {
					LogError("VectorLine.MakeRect: The length of the array for discrete lines needs to be at least 8 for \"" + name + "\"");
					return;
				}
				LogError("Calling VectorLine.MakeRect with an index of " + index + " would exceed the length of the Vector2 array for \"" + name + "\"");
				return;
			}
			if (m_is2D) {
				points2[index  ] = new Vector2(topLeft.x,     topLeft.y);
				points2[index+1] = new Vector2(bottomRight.x, topLeft.y);
				points2[index+2] = new Vector2(topLeft.x,     bottomRight.y);
				points2[index+3] = new Vector2(bottomRight.x, bottomRight.y);
				points2[index+4] = new Vector2(topLeft.x,     topLeft.y);
				points2[index+5] = new Vector2(topLeft.x,     bottomRight.y);
				points2[index+6] = new Vector2(bottomRight.x, topLeft.y);
				points2[index+7] = new Vector2(bottomRight.x, bottomRight.y);
			}
			else {
				points3[index  ] = new Vector3(topLeft.x,     topLeft.y,	  topLeft.z);
				points3[index+1] = new Vector3(bottomRight.x, topLeft.y, 	  topLeft.z);
				points3[index+2] = new Vector3(topLeft.x,     bottomRight.y, bottomRight.z);
				points3[index+3] = new Vector3(bottomRight.x, bottomRight.y, bottomRight.z);
				points3[index+4] = new Vector3(topLeft.x,     topLeft.y, 	  topLeft.z);
				points3[index+5] = new Vector3(topLeft.x,     bottomRight.y, bottomRight.z);
				points3[index+6] = new Vector3(bottomRight.x, topLeft.y, 	  topLeft.z);
				points3[index+7] = new Vector3(bottomRight.x, bottomRight.y, bottomRight.z);
			}
		}
	}

	public void MakeCircle (Vector3 origin, float radius) {
		MakeEllipse (origin, Vector3.forward, radius, radius, GetSegmentNumber(), 0.0f, 0);
	}
	
	public void MakeCircle (Vector3 origin, float radius, int segments) {
		MakeEllipse (origin, Vector3.forward, radius, radius, segments, 0.0f, 0);
	}

	public void MakeCircle (Vector3 origin, float radius, int segments, float pointRotation) {
		MakeEllipse (origin, Vector3.forward, radius, radius, segments, pointRotation, 0);
	}

	public void MakeCircle (Vector3 origin, float radius, int segments, int index) {
		MakeEllipse (origin, Vector3.forward, radius, radius, segments, 0.0f, index);
	}

	public void MakeCircle (Vector3 origin, float radius, int segments, float pointRotation, int index) {
		MakeEllipse (origin, Vector3.forward, radius, radius, segments, pointRotation, index);
	}

	public void MakeCircle (Vector3 origin, Vector3 upVector, float radius) {
		MakeEllipse (origin, upVector, radius, radius, GetSegmentNumber(), 0.0f, 0);
	}
	
	public void MakeCircle (Vector3 origin, Vector3 upVector, float radius, int segments) {
		MakeEllipse (origin, upVector, radius, radius, segments, 0.0f, 0);
	}

	public void MakeCircle (Vector3 origin, Vector3 upVector, float radius, int segments, float pointRotation) {
		MakeEllipse (origin, upVector, radius, radius, segments, pointRotation, 0);
	}

	public void MakeCircle (Vector3 origin, Vector3 upVector, float radius, int segments, int index) {
		MakeEllipse (origin, upVector, radius, radius, segments, 0.0f, index);
	}

	public void MakeCircle (Vector3 origin, Vector3 upVector, float radius, int segments, float pointRotation, int index) {
		MakeEllipse (origin, upVector, radius, radius, segments, pointRotation, index);
	}

	public void MakeEllipse (Vector3 origin, float xRadius, float yRadius) {
		MakeEllipse (origin, Vector3.forward, xRadius, yRadius, GetSegmentNumber(), 0.0f, 0);
	}
	
	public void MakeEllipse (Vector3 origin, float xRadius, float yRadius, int segments) {
		MakeEllipse (origin, Vector3.forward, xRadius, yRadius, segments, 0.0f, 0);
	}
	
	public void MakeEllipse (Vector3 origin, float xRadius, float yRadius, int segments, int index) {
		MakeEllipse (origin, Vector3.forward, xRadius, yRadius, segments, 0.0f, index);
	}

	public void MakeEllipse (Vector3 origin, float xRadius, float yRadius, int segments, float pointRotation) {
		MakeEllipse (origin, Vector3.forward, xRadius, yRadius, segments, pointRotation, 0);
	}

	public void MakeEllipse (Vector3 origin, Vector3 upVector, float xRadius, float yRadius) {
		MakeEllipse (origin, upVector, xRadius, yRadius, GetSegmentNumber(), 0.0f, 0);
	}

	public void MakeEllipse (Vector3 origin, Vector3 upVector, float xRadius, float yRadius, int segments) {
		MakeEllipse (origin, upVector, xRadius, yRadius, segments, 0.0f, 0);
	}
	
	public void MakeEllipse (Vector3 origin, Vector3 upVector, float xRadius, float yRadius, int segments, int index) {
		MakeEllipse (origin, upVector, xRadius, yRadius, segments, 0.0f, index);
	}

	public void MakeEllipse (Vector3 origin, Vector3 upVector, float xRadius, float yRadius, int segments, float pointRotation) {
		MakeEllipse (origin, upVector, xRadius, yRadius, segments, pointRotation, 0);
	}
	
	public void MakeEllipse (Vector3 origin, Vector3 upVector, float xRadius, float yRadius, int segments, float pointRotation, int index) {
		if (segments < 3) {
			LogError("VectorLine.MakeEllipse needs at least 3 segments");
			return;
		}
		if (!CheckArrayLength (FunctionName.MakeEllipse, segments, index)) {
			return;
		}
		
		float radians = 360.0f / segments*Mathf.Deg2Rad;
		float p = -pointRotation*Mathf.Deg2Rad;
		
		if (m_continuous) {
			int i = 0;
			if (m_is2D) {
				Vector2 v2Origin = origin;
				for (i = 0; i < segments; i++) {
					points2[index+i] = v2Origin + new Vector2(.5f + Mathf.Cos(p)*xRadius, .5f + Mathf.Sin(p)*yRadius);
					p += radians;
				}
				if (!m_isPoints) {
					points2[index+i] = points2[index+(i-segments)];
				}
			}
			else {
				var thisMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.LookRotation(-upVector, upVector), Vector3.one);
				for (i = 0; i < segments; i++) {
					points3[index+i] = origin + thisMatrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(p)*xRadius, Mathf.Sin(p)*yRadius, 0.0f));
					p += radians;
				}
				if (!m_isPoints) {
					points3[index+i] = points3[index+(i-segments)];
				}
			}
		}
		
		else {
			if (m_is2D) {
				Vector2 v2Origin = origin;
				for (int i = 0; i < segments*2; i++) {
					points2[index+i] = v2Origin + new Vector2(.5f + Mathf.Cos(p)*xRadius, .5f + Mathf.Sin(p)*yRadius);
					p += radians;
					i++;
					points2[index+i] = v2Origin + new Vector2(.5f + Mathf.Cos(p)*xRadius, .5f + Mathf.Sin(p)*yRadius);
				}
			}
			else {
				var thisMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.LookRotation(-upVector, upVector), Vector3.one);
				for (int i = 0; i < segments*2; i++) {
					points3[index+i] = origin + thisMatrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(p)*xRadius, Mathf.Sin(p)*yRadius, 0.0f));
					p += radians;
					i++;
					points3[index+i] = origin + thisMatrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(p)*xRadius, Mathf.Sin(p)*yRadius, 0.0f));
				}
			}
		}
	}

	public void MakeCurve (Vector2[] curvePoints) {
		MakeCurve (curvePoints, GetSegmentNumber(), 0);
	}
	
	public void MakeCurve (Vector2[] curvePoints, int segments) {
		MakeCurve (curvePoints, segments, 0);
	}

	public void MakeCurve (Vector2[] curvePoints, int segments, int index) {
		if (curvePoints.Length != 4) {
			LogError("VectorLine.MakeCurve needs exactly 4 points in the curve points array");
			return;
		}
		MakeCurve (curvePoints[0], curvePoints[1], curvePoints[2], curvePoints[3], segments, index);
	}

	public void MakeCurve (Vector3[] curvePoints) {
		MakeCurve (curvePoints, GetSegmentNumber(), 0);
	}
	
	public void MakeCurve (Vector3[] curvePoints, int segments) {
		MakeCurve (curvePoints, segments, 0);
	}
	
	public void MakeCurve (Vector3[] curvePoints, int segments, int index) {
		if (curvePoints.Length != 4) {
			LogError("VectorLine.MakeCurve needs exactly 4 points in the curve points array");
			return;
		}
		MakeCurve (curvePoints[0], curvePoints[1], curvePoints[2], curvePoints[3], segments, index);
	}

	public void MakeCurve (Vector3 anchor1, Vector3 control1, Vector3 anchor2, Vector3 control2) {
		MakeCurve (anchor1, control1, anchor2, control2, GetSegmentNumber(), 0);
	}
	
	public void MakeCurve (Vector3 anchor1, Vector3 control1, Vector3 anchor2, Vector3 control2, int segments) {
		MakeCurve (anchor1, control1, anchor2, control2, segments, 0);
	}
	
	public void MakeCurve (Vector3 anchor1, Vector3 control1, Vector3 anchor2, Vector3 control2, int segments, int index) {
		if (!CheckArrayLength (FunctionName.MakeCurve, segments, index)) {
			return;
		}
		
		if (m_continuous) {
			int end = m_isPoints? segments : segments+1;
			if (m_is2D) {
				for (int i = 0; i < end; i++) {
					points2[index+i] = GetBezierPoint (anchor1, control1, anchor2, control2, (float)i/segments);
				}
			}
			else {
				for (int i = 0; i < end; i++) {
					points3[index+i] = GetBezierPoint3D (anchor1, control1, anchor2, control2, (float)i/segments);
				}
			}
		}
		
		else {
			int idx = 0;
			if (m_is2D) {
				for (int i = 0; i < segments; i++) {
					points2[index + idx++] = GetBezierPoint (anchor1, control1, anchor2, control2, (float)i/segments);
					points2[index + idx++] = GetBezierPoint (anchor1, control1, anchor2, control2, (float)(i+1)/segments);
				}
			}
			else {
				for (int i = 0; i < segments; i++) {
					points3[index + idx++] = GetBezierPoint3D (anchor1, control1, anchor2, control2, (float)i/segments);
					points3[index + idx++] = GetBezierPoint3D (anchor1, control1, anchor2, control2, (float)(i+1)/segments);
				}
			}
		}
	}
	
	static Vector2 GetBezierPoint (Vector2 anchor1, Vector2 control1, Vector2 anchor2, Vector2 control2, float t) {
		float cx = 3 * (control1.x - anchor1.x);
		float bx = 3 * (control2.x - control1.x) - cx;
		float ax = anchor2.x - anchor1.x - cx - bx;
		float cy = 3 * (control1.y - anchor1.y);
		float by = 3 * (control2.y - control1.y) - cy;
		float ay = anchor2.y - anchor1.y - cy - by;
		
		return new Vector2( (ax * (t*t*t)) + (bx * (t*t)) + (cx * t) + anchor1.x,
						    (ay * (t*t*t)) + (by * (t*t)) + (cy * t) + anchor1.y );
	}

	static Vector3 GetBezierPoint3D (Vector3 anchor1, Vector3 control1, Vector3 anchor2, Vector3 control2, float t) {
		float cx = 3 * (control1.x - anchor1.x);
		float bx = 3 * (control2.x - control1.x) - cx;
		float ax = anchor2.x - anchor1.x - cx - bx;
		float cy = 3 * (control1.y - anchor1.y);
		float by = 3 * (control2.y - control1.y) - cy;
		float ay = anchor2.y - anchor1.y - cy - by;
		float cz = 3 * (control1.z - anchor1.z);
		float bz = 3 * (control2.z - control1.z) - cz;
		float az = anchor2.z - anchor1.z - cz - bz;
		
		return new Vector3( (ax * (t*t*t)) + (bx * (t*t)) + (cx * t) + anchor1.x,
							(ay * (t*t*t)) + (by * (t*t)) + (cy * t) + anchor1.y,
							(az * (t*t*t)) + (bz * (t*t)) + (cz * t) + anchor1.z );
	}

	public void MakeSpline (Vector2[] splinePoints) {
		MakeSpline (splinePoints, null, GetSegmentNumber(), 0, false);
	}

	public void MakeSpline (Vector2[] splinePoints, bool loop) {
		MakeSpline (splinePoints, null, GetSegmentNumber(), 0, loop);
	}
	
	public void MakeSpline (Vector2[] splinePoints, int segments) {
		MakeSpline (splinePoints, null, segments, 0, false);
	}

	public void MakeSpline (Vector2[] splinePoints, int segments, bool loop) {
		MakeSpline (splinePoints, null, segments, 0, loop);
	}

	public void MakeSpline (Vector2[] splinePoints, int segments, int index) {
		MakeSpline (splinePoints, null, segments, index, false);
	}

	public void MakeSpline (Vector2[] splinePoints, int segments, int index, bool loop) {
		MakeSpline (splinePoints, null, segments, index, loop);
	}

	public void MakeSpline (Vector3[] splinePoints) {
		MakeSpline (null, splinePoints, GetSegmentNumber(), 0, false);
	}

	public void MakeSpline (Vector3[] splinePoints, bool loop) {
		MakeSpline (null, splinePoints, GetSegmentNumber(), 0, loop);
	}
	
	public void MakeSpline (Vector3[] splinePoints, int segments) {
		MakeSpline (null, splinePoints, segments, 0, false);
	}

	public void MakeSpline (Vector3[] splinePoints, int segments, bool loop) {
		MakeSpline (null, splinePoints, segments, 0, loop);
	}

	public void MakeSpline (Vector3[] splinePoints, int segments, int index) {
		MakeSpline (null, splinePoints, segments, index, false);
	}

	public void MakeSpline (Vector3[] splinePoints, int segments, int index, bool loop) {
		MakeSpline (null, splinePoints, segments, index, loop);
	}
		
	private void MakeSpline (Vector2[] splinePoints2, Vector3[] splinePoints3, int segments, int index, bool loop) {
		int pointsLength = (splinePoints2 != null)? splinePoints2.Length : splinePoints3.Length;		
		if (pointsLength < 2) {
			LogError("VectorLine.MakeSpline needs at least 2 spline points");
			return;
		}
		if (splinePoints2 != null && !m_is2D) {
			LogError("VectorLine.MakeSpline was called with a Vector2 spline points array, but the line uses Vector3 points");
			return;
		}
		if (splinePoints3 != null && m_is2D) {
			LogError("VectorLine.MakeSpline was called with a Vector3 spline points array, but the line uses Vector2 points");
			return;
		}
		if (!CheckArrayLength (FunctionName.MakeSpline, segments, index)) {
			return;
		}

		var pointCount = index;
		var numberOfPoints = loop? pointsLength : pointsLength-1;
		var add = 1.0f / segments * numberOfPoints;
		float i, start = 0.0f;
		int j, p0 = 0, p2 = 0, p3 = 0;
		
		for (j = 0; j < numberOfPoints; j++) {
			p0 = j-1;
			p2 = j+1;
			p3 = j+2;
			if (p0 < 0) {
				p0 = loop? numberOfPoints-1 : 0;
			}
			if (loop && p2 > numberOfPoints-1) {
				p2 -= numberOfPoints;
			}
			if (p3 > numberOfPoints-1) {
				p3 = loop? p3-numberOfPoints : numberOfPoints;
			}
			if (m_continuous) {
				if (m_is2D) {
					for (i = start; i <= 1.0f; i += add) {
						points2[pointCount++] = GetSplinePoint (splinePoints2[p0], splinePoints2[j], splinePoints2[p2], splinePoints2[p3], i);
					}
				}
				else {
					for (i = start; i <= 1.0f; i += add) {
						points3[pointCount++] = GetSplinePoint3D (splinePoints3[p0], splinePoints3[j], splinePoints3[p2], splinePoints3[p3], i);
					}
				}
			}
			else {
				if (m_is2D) {
					for (i = start; i <= 1.0f; i += add) {
						points2[pointCount++] = GetSplinePoint (splinePoints2[p0], splinePoints2[j], splinePoints2[p2], splinePoints2[p3], i);
						if (pointCount > index+1 && pointCount < index + (segments*2)) {
							points2[pointCount++] = points2[pointCount-2];
						}
					}
				}
				else {
					for (i = start; i <= 1.0f; i += add) {
						points3[pointCount++] = GetSplinePoint3D (splinePoints3[p0], splinePoints3[j], splinePoints3[p2], splinePoints3[p3], i);
						if (pointCount > index+1 && pointCount < index + (segments*2)) {
							points3[pointCount++] = points3[pointCount-2];
						}
					}
				}
			}
			start = i - 1.0f;
		}
		// The last point might not get done depending on number of splinePoints and segments, so ensure that it's done here
		if ( (m_continuous && pointCount < index + (segments+1)) || (!m_continuous && pointCount < index + (segments*2)) ) {
			if (m_is2D) {
				points2[pointCount] = GetSplinePoint (splinePoints2[p0], splinePoints2[j-1], splinePoints2[p2], splinePoints2[p3], 1.0f);			
			}
			else {
				points3[pointCount] = GetSplinePoint3D (splinePoints3[p0], splinePoints3[j-1], splinePoints3[p2], splinePoints3[p3], 1.0f);		
			}
		}
	}

	static Vector2 GetSplinePoint (Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t) {
		float t2 = t*t;
		float t3 = t2*t;
		return new Vector2 (0.5f * ((2.0f*p1.x) + (-p0.x + p2.x)*t + (2.0f*p0.x - 5.0f*p1.x + 4.0f*p2.x - p3.x)*t2 + (-p0.x + 3.0f*p1.x- 3.0f*p2.x + p3.x)*t3),
							0.5f * ((2.0f*p1.y) + (-p0.y + p2.y)*t + (2.0f*p0.y - 5.0f*p1.y + 4.0f*p2.y - p3.y)*t2 + (-p0.y + 3.0f*p1.y- 3.0f*p2.y + p3.y)*t3));
	}
	
	static Vector3 GetSplinePoint3D (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
		float t2 = t*t;
		float t3 = t2*t;
		return new Vector3 (0.5f * ((2.0f*p1.x) + (-p0.x + p2.x)*t + (2.0f*p0.x - 5.0f*p1.x + 4.0f*p2.x - p3.x)*t2 + (-p0.x + 3.0f*p1.x- 3.0f*p2.x + p3.x)*t3),
							0.5f * ((2.0f*p1.y) + (-p0.y + p2.y)*t + (2.0f*p0.y - 5.0f*p1.y + 4.0f*p2.y - p3.y)*t2 + (-p0.y + 3.0f*p1.y- 3.0f*p2.y + p3.y)*t3),
							0.5f * ((2.0f*p1.z) + (-p0.z + p2.z)*t + (2.0f*p0.z - 5.0f*p1.z + 4.0f*p2.z - p3.z)*t2 + (-p0.z + 3.0f*p1.z- 3.0f*p2.z + p3.z)*t3));
	}
	
	public void MakeText (string text, Vector3 startPos, float size) {
		MakeText (text, startPos, size, 1.0f, 1.5f, true);
	}
	
	public void MakeText (string text, Vector3 startPos, float size, bool uppercaseOnly) {
		MakeText (text, startPos, size, 1.0f, 1.5f, uppercaseOnly);
	}
	
	public void MakeText (string text, Vector3 startPos, float size, float charSpacing, float lineSpacing) {
		MakeText (text, startPos, size, charSpacing, lineSpacing, true);
	}
	
	public void MakeText (string text, Vector3 startPos, float size, float charSpacing, float lineSpacing, bool uppercaseOnly) {
		if (m_continuous) {
			LogError ("VectorLine.MakeText can only be used with a discrete line");
			return;
		}
		int charPointsLength = 0;
		
		// Get total number of points needed for all characters in the string
		for (int i = 0; i < text.Length; i++) {
			int charNum = System.Convert.ToInt32(text[i]);
			if (charNum < 0 || charNum > VectorChar.numberOfCharacters) {
				LogError ("VectorLine.MakeText: Character '" + text[i] + "' is not valid");
				return;
			}
			if (uppercaseOnly && charNum >= 97 && charNum <= 122) {
				charNum -= 32;
			}
			if (VectorChar.data[charNum] != null) {
				charPointsLength += VectorChar.data[charNum].Length;
			}
		}
		if (charPointsLength > m_pointsLength) {
			Resize (charPointsLength);
		}
		else if (charPointsLength < m_pointsLength) {
			ZeroPoints (charPointsLength);
		}
		
		float charPos = 0.0f, linePos = 0.0f;
		int idx = 0;
		var scaleVector = new Vector2(size, size);

		for (int i = 0; i < text.Length; i++) {
			int charNum = System.Convert.ToInt32(text[i]);
			// Newline
			if (charNum == 10) {
				linePos -= lineSpacing;
				charPos = 0.0f;
			}
			// Space
			else if (charNum == 32) {
				charPos += charSpacing;
			}
			// Character
			else {
				if (uppercaseOnly && charNum >= 97 && charNum <= 122) {
					charNum -= 32;
				}
				int end = VectorChar.data[charNum].Length;
				if (m_is2D) {
					for (int j = 0; j < end; j++) {
						points2[idx++] = Vector2.Scale(VectorChar.data[charNum][j] + new Vector2(charPos, linePos), scaleVector) + (Vector2)startPos;
					}
				}
				else {
					for (int j = 0; j < end; j++) {
						points3[idx++] = Vector3.Scale((Vector3)VectorChar.data[charNum][j] + new Vector3(charPos, linePos, 0.0f), scaleVector) + startPos;
					}
				}
				charPos += charSpacing;
			}
		}
	}
	
	public void MakeWireframe (Mesh mesh) {
		if (m_continuous) {
			LogError ("VectorLine.MakeWireframe only works with a discrete line");
			return;
		}
		if (m_is2D) {
			LogError ("VectorLine.MakeWireframe only works with Vector3 points");
			return;
		}
		if (mesh == null) {
			LogError ("VectorLine.MakeWireframe can't use a null mesh");
			return;
		}
		var meshTris = mesh.triangles;
		var meshVertices = mesh.vertices;
		var pairs = new Dictionary<Vector3Pair, bool>();
		var linePoints = new List<Vector3>();
		
		for (int i = 0; i < meshTris.Length; i += 3) {
			CheckPairPoints (pairs, meshVertices[meshTris[i]],   meshVertices[meshTris[i+1]], linePoints);
			CheckPairPoints (pairs, meshVertices[meshTris[i+1]], meshVertices[meshTris[i+2]], linePoints);
			CheckPairPoints (pairs, meshVertices[meshTris[i+2]], meshVertices[meshTris[i]],   linePoints);
		}
		
		if (linePoints.Count > points3.Length) {
			System.Array.Resize (ref points3, linePoints.Count);
			Resize (linePoints.Count);
		}
		else if (linePoints.Count < points3.Length) {
			ZeroPoints (linePoints.Count);
		}
		System.Array.Copy (linePoints.ToArray(), points3, linePoints.Count);
	}

	static void CheckPairPoints (Dictionary<Vector3Pair, bool> pairs, Vector3 p1, Vector3 p2, List<Vector3> linePoints) {
		var pair1 = new Vector3Pair(p1, p2);
		var pair2 = new Vector3Pair(p2, p1);
		if (!pairs.ContainsKey(pair1) && !pairs.ContainsKey(pair2)) {
			pairs[pair1] = true;
			pairs[pair2] = true;
			linePoints.Add(p1);
			linePoints.Add(p2);
		}
	}

	public void SetDistances () {
		if (m_distances == null || m_distances.Length != (m_continuous? m_pointsLength : m_pointsLength/2+1)) {
			m_distances = new float[m_continuous? m_pointsLength : m_pointsLength/2+1];
		}

		var totalDistance = 0.0d;
		int pointsLength = m_pointsLength-1;
		
		if (points3 != null) {
			if (m_continuous) {
				for (int i = 0; i < pointsLength; i++) {
					Vector3 diff = points3[i] - points3[i+1];
					totalDistance += System.Math.Sqrt(diff.x*diff.x + diff.y*diff.y + diff.z*diff.z); // Same as Vector3.Distance, but with double instead of float
					m_distances[i+1] = (float)totalDistance;
				}
			}
			else {
				var count = 1;
				for (int i = 0; i < pointsLength; i += 2) {
					Vector3 diff = points3[i] - points3[i+1];
					totalDistance += System.Math.Sqrt(diff.x*diff.x + diff.y*diff.y + diff.z*diff.z);
					m_distances[count++] = (float)totalDistance;
				}
			}
		}
		else {
			if (m_continuous) {
				for (int i = 0; i < pointsLength; i++) {
					Vector2 diff = points2[i] - points2[i+1];
					totalDistance += System.Math.Sqrt(diff.x*diff.x + diff.y*diff.y); // Same as Vector2.Distance, but with double instead of float
					m_distances[i+1] = (float)totalDistance;
				}
			}
			else {
				var count = 1;
				for (int i = 0; i < pointsLength; i += 2) {
					Vector2 diff = points2[i] - points2[i+1];
					totalDistance += System.Math.Sqrt(diff.x*diff.x + diff.y*diff.y);
					m_distances[count++] = (float)totalDistance;
				}
			}
		}
	}
	
	public float GetLength () {
		if (m_distances == null || m_distances.Length != (m_continuous? m_pointsLength : m_pointsLength/2+1)) {
			SetDistances();
		}
		return m_distances[m_distances.Length-1];
	}

	public Vector2 GetPoint01 (float distance) {
		return GetPoint (Mathf.Lerp(0.0f, GetLength(), distance) );
	}

	public Vector2 GetPoint (float distance) {
		if (!m_is2D) {
			LogError("VectorLine.GetLinePoint only works with Vector2 points");
			return Vector2.zero;
		}
		if (points2.Length < 2) {
			LogError("VectorLine.GetLinePoint needs at least 2 points in the points2 array");
			return Vector2.zero;
		}
		if (m_distances == null) {
			SetDistances();
		}
		int i = 1;
		int end = m_distances.Length-1;
		while (distance > m_distances[i] && i < end) {
			i++;
		}
		if (m_continuous) {
			return Vector2.Lerp(points2[i-1], points2[i], Mathf.InverseLerp(m_distances[i-1], m_distances[i], distance));
		}
		return Vector2.Lerp(points2[(i-1)*2], points2[(i-1)*2+1], Mathf.InverseLerp(m_distances[i-1], m_distances[i], distance));
	}

	public Vector3 GetPoint3D01 (float distance) {
		return GetPoint3D (Mathf.Lerp(0.0f, GetLength(), distance) );
	}
	
	public Vector3 GetPoint3D (float distance) {
		if (m_is2D) {
			LogError("VectorLine.GetLinePoint3D only works with Vector3 points");
			return Vector3.zero;
		}
		if (points3.Length < 2) {
			LogError("VectorLine.GetLinePoint3D needs at least 2 points in the points3 array");
			return Vector3.zero;
		}
		if (m_distances == null) {
			SetDistances();
		}
		int i = 1;
		int end = m_distances.Length-1;
		while (distance > m_distances[i] && i < end) {
			i++;
		}
		if (m_continuous) {
			return Vector3.Lerp(points3[i-1], points3[i], Mathf.InverseLerp(m_distances[i-1], m_distances[i], distance));			
		}
		return Vector3.Lerp(points3[(i-1)*2], points3[(i-1)*2+1], Mathf.InverseLerp(m_distances[i-1], m_distances[i], distance));
	}

	public void ZeroPoints () {
		ZeroPoints (0, m_pointsLength);
	}
	
	public void ZeroPoints (int startIndex) {
		ZeroPoints (startIndex, m_pointsLength);
	}

	public void ZeroPoints (int startIndex, int endIndex) {
		if (endIndex < 0 || endIndex > m_pointsLength || startIndex < 0 || startIndex > m_pointsLength || startIndex > endIndex) {
			LogError("VectorLine: index out of range for \"" + name + "\" when calling ZeroPoints. StartIndex: " + startIndex + ", EndIndex: " + endIndex + ", array length: " + m_pointsLength);
			return;
		}
		
		if (m_is2D) {
			var v2zero = Vector2.zero;	// Making a local variable is at least twice as fast for some reason
			for (int i = startIndex; i < endIndex; i++) {
				points2[i] = v2zero;
			}
		}
		else {
			var v3zero = Vector3.zero;
			for (int i = startIndex; i < endIndex; i++) {
				points3[i] = v3zero;
			}
		}
	}
	
	void ZeroVertices (int startIndex, int endIndex) {
		var v3zero = Vector3.zero;
		if (m_continuous) {
			startIndex *= 4;
			endIndex *= 4;
			if (endIndex > m_lineVertices.Length) {
				endIndex -= 4;
			}
			for (int i = startIndex; i < endIndex; i += 4) {
				m_lineVertices[i  ] = v3zero;
				m_lineVertices[i+1] = v3zero;
				m_lineVertices[i+2] = v3zero;
				m_lineVertices[i+3] = v3zero;
			}
		}
		else {
			startIndex *= 2;
			endIndex *= 2;
			for (int i = startIndex; i < endIndex; i += 2) {
				m_lineVertices[i  ] = v3zero;
				m_lineVertices[i+1] = v3zero;
			}
		}
	}
	
	bool Approximately2 (Vector2 p1, Vector2 p2) {
		return Approximately(p1.x, p2.x) && Approximately(p1.y, p2.y);
	}

	bool Approximately3 (Vector3 p1, Vector3 p2) {
		return Approximately(p1.x, p2.x) && Approximately(p1.y, p2.y) && Approximately(p1.z, p2.z);
	}
	
	bool Approximately (float a, float b) {
		return Mathf.Round(a*100)/100 == Mathf.Round(b*100)/100;
	}
}

public class VectorPoints : VectorLine {
	public VectorPoints (string name, Vector2[] points, Material material, float width) : base (true, name, points, material, width) {}
	public VectorPoints (string name, Vector2[] points, Color[] colors, Material material, float width) : base (true, name, points, colors, material, width) {}
	public VectorPoints (string name, Vector2[] points, Color color, Material material, float width) : base (true, name, points, color, material, width) {}

	public VectorPoints (string name, Vector3[] points, Material material, float width) : base (true, name, points, material, width) {}
	public VectorPoints (string name, Vector3[] points, Color[] colors, Material material, float width) : base (true, name, points, colors, material, width) {}
	public VectorPoints (string name, Vector3[] points, Color color, Material material, float width) : base (true, name, points, color, material, width) {}
}

public struct Vector3Pair {
	public Vector3 p1;
	public Vector3 p2;
	public Vector3Pair (Vector3 point1, Vector3 point2) {
		p1 = point1;
		p2 = point2;
	}
}