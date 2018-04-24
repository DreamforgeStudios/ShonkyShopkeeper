using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// INFO: Don't use InputManager (which will be event based) in this class.  It needs more precision and more details.
public class Cutting : MonoBehaviour {
	public Vector3[] cutOrigins;
	public Vector3[] cutVectors;
	public float[] cutTimes;

	// The maximum distance that the cut origin can be from the intended origin to be considered a fail.
	public float maximumDistance;
	// The amount that the distance should affect the final result.
	public float impactDistance;

	// The maximum difference that the closeness can be to be considered a fail.
	public float maximumCloseness;
	public float impactCloseness;

	public float maximumLength;
	public float impactLength;

	public float maximumTimeDiff;

	// (?) Allow these vectors to be nullable, so we can reset them more conveniently later.
	// This also means that we have to use .Value to get the value of these vectors.
	private Vector3? touchOrigin;
	private Vector3? touchVector;

	// Current index of the point we're cutting.
	private int currentIndex;

	// DEBUG.
	private float oCloseness;
	private float vCloseness;
	private float lCloseness;
	public GameObject debugObject;

	// Use this for initialization
	void Start () {
		currentIndex = 0;
		// DEBUG.
		for (int i = 0; i < cutOrigins.Length; i++) {
			// Place objects at start and end positions.
			Instantiate(debugObject, cutOrigins[i], Quaternion.identity);
			//Instantiate(debugObject, cutOrigins[i] + cutVectors[i], Quaternion.identity);
			Vector3 start = cutOrigins[i];
			Vector3 end = start + cutVectors[i];
			if (i == currentIndex)
				Debug.DrawLine(start, end, Color.red, 200);
			else
				Debug.DrawLine(start, end, Color.blue, 200);
		}
	}
	
	// Update is called once per frame
	void Update () {
		// Check if we are running either in the Unity editor or in a standalone build.
		#if UNITY_STANDALONE || UNITY_WEBPLAYER
		// Process mouse inputs.
		ProcessMouse();

		#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
		// Process touch inputs.
		ProcessTouch();

		// End platform dependant input.
		#endif
	}

	// TODO, program for touch events.
	private void ProcessTouch() {
		// Loop through all current touch events.
		foreach (Touch touch in Input.touches) {
			Debug.Log("Touched the screen at position: " + touch.position);
			// Construct a ray from the current touch coordinates.
			Ray ray = Camera.main.ScreenPointToRay(touch.position);
			if (Physics.Raycast(ray)) {
				// Do something if hit.
			}
		}
	}

	private void ProcessMouse() {
		// TODO: record time.
		if (Input.GetMouseButtonDown(0)) {
			//Debug.Log("Clicked the screen at position: " + Input.mousePosition);
			touchOrigin = ConvertToWorldPoint(Input.mousePosition);
		}

		if (Input.GetMouseButtonUp(0)) {
			Vector2 mousePos = Input.mousePosition;
			touchVector = ConvertToWorldPoint(mousePos) - touchOrigin;
			float close = CalculateCloseness(touchOrigin.Value, touchVector.Value);
			DrawDebugLine(touchOrigin.Value);
			//Debug.Log(close);

			// Reset the origin.
			touchOrigin = null;

			/* DEBUG --
			Vector2 mousePos = Input.mousePosition;
			Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10));
			transform.position = pos;

			// Get the vector of the current input.
			touchVector = touchOrigin - Input.mousePosition;
			Vector3 start = touchOrigin.Value;
			start.z = 10;

			Vector3 end = Input.mousePosition;
			end.z = 10;
			Debug.DrawLine(Camera.main.ScreenToWorldPoint(start), Camera.main.ScreenToWorldPoint(end), Color.blue, 1f);
			*/

		}
	}

	private void DrawDebugLine(Vector3 origin) {
		Vector3 end = ConvertToWorldPoint(Input.mousePosition);
		Debug.DrawLine(origin, end, Color.yellow, 2);
	}

	// TODO: this will eventually go in the input manager.
	private Vector3 ConvertToWorldPoint(Vector3 screenPoint) {
		// If this is changed from 10, it fucks up.
		// I think that 10 indicates 10 units FROM the camera, not 10 in the scene.
		screenPoint.z = 10;
		return Camera.main.ScreenToWorldPoint(screenPoint);
	}

	private float CalculateCloseness(Vector3 origin, Vector3 vec) {
		//origin = ConvertToWorldPoint(origin);

		// 0 = close, 1 = far.
		float originCloseness = 0f;
		float vectorCloseness = 0f;
		// We should measure length closeness because we normalize when calculating vector closeness.
		float lengthCloseness = 0f;

		// Calculate how close the player was to the starting point.
		float distance = Vector3.Distance(origin, cutOrigins[currentIndex]);
		originCloseness = Mathf.InverseLerp(0, maximumDistance, distance);
		// So the debugUI can use it.
		this.oCloseness = originCloseness;

		// Calculate how similar the vectors are (how close the player was to the correct swipe).
		// Normalize so that distance doesn't affect the dot product.
		Vector3 vn = Vector3.Normalize(vec);
		Vector3 cn = Vector3.Normalize(cutVectors[currentIndex]);
		float vSimilarity = 1-Vector3.Dot(vn, cn);
		Debug.Log(vSimilarity);
		vectorCloseness = Mathf.InverseLerp(0, maximumCloseness, vSimilarity);
		this.vCloseness = vectorCloseness;

		// Calculate how close the vector length is to the optimum length.  (did the player overshoot? undershoot? etc).
		float vl = Vector3.Magnitude(vec);
		float cl = Vector3.Magnitude(cutVectors[currentIndex]);
		lengthCloseness = Mathf.InverseLerp(0, maximumLength, Mathf.Abs(vl-cl));
		this.lCloseness = lengthCloseness;

		originCloseness *= impactDistance;
		vectorCloseness *= impactCloseness;
		lengthCloseness *= impactLength;

		return (originCloseness + vectorCloseness + lengthCloseness) / 3f;
	}

    void OnGUI()
    {
        Vector3 p = new Vector3();
        Camera  c = Camera.main;
        Event   e = Event.current;
        Vector2 mousePos = new Vector2();

        // Get the mouse position from Event.
        // Note that the y position from Event is inverted.
        mousePos.x = e.mousePosition.x;
        mousePos.y = c.pixelHeight - e.mousePosition.y;

        p = c.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, c.nearClipPlane));

        GUILayout.BeginArea(new Rect(20, 20, 250, 120));
        GUILayout.Label("Origin Closeness: " + oCloseness);
        GUILayout.Label("Vector Closeness: " + vCloseness);
        GUILayout.Label("Length Closeness: " + lCloseness);
        GUILayout.EndArea();
    }
}
