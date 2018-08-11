using System.Collections;
using System.Collections.Generic;
using System.Timers;
using NaughtyAttributes;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;

public class RadialSlider : MonoBehaviour {
	public bool Clockwise = true;
	public float InitialSpeed = 40f;
	[Range(-180, 180)]
	public float MaxRotation;
	[Range(-180, 180)]
	public float MinRotation;
	
	private SpriteRenderer sr;
	private bool okToAlternate = true;
	private float activeSpeed;

	// Use this for initialization
	void Start () {
		sr = GetComponent<SpriteRenderer>();
		activeSpeed = InitialSpeed;
	}
	
	// Update is called once per frame
	void Update () {
		var speed = Clockwise ? -activeSpeed : activeSpeed;
		
		transform.RotateAround(transform.parent.position, transform.parent.forward, speed * Time.deltaTime);

		// Same calculation as in shader.
		// Returns an angle between 180 and -180.
		float angle = Mathf.Atan2(transform.localPosition.y, transform.localPosition.x) * Mathf.Rad2Deg;
		if (angle > MaxRotation && okToAlternate) {
			Clockwise = !Clockwise;
			okToAlternate = false;
		} else if (angle < MinRotation && okToAlternate) {
			Clockwise = !Clockwise;
			okToAlternate = false;
		// Only allow altering if we've been between the correct angles.
		} else if (angle < MaxRotation && angle > MinRotation) {
			okToAlternate = true;
		}
	}

	[Button("Pause")]
	public void Pause() {
		activeSpeed = 0f;
	}

	[Button("Play")]
	public void Play() {
		activeSpeed = InitialSpeed;
	}
}
