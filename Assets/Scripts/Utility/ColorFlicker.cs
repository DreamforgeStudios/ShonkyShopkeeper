using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using DG.Tweening;

public class ColorFlicker : MonoBehaviour {
	public Color Color1, Color2;
	[Range(0, 1)]
	public float FlickerDuration;
	[Range(0, 1)]
	public float Randomness;
	public bool UseUnscaledTime;

	private Light light;

	private float realFlickerDuration;
	private Color activeColor, inactiveColor;
	void Start () {
		light = GetComponent<Light>();
		
		light.color = Color1;
		activeColor = Color1;
		inactiveColor = Color2;

		realFlickerDuration = FlickerDuration + Random.Range(-Randomness, Randomness);
	}

	private float lerpPoint = 0;
	void Update() {
		lerpPoint += UseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
		float lerpVal = Mathf.InverseLerp(0, FlickerDuration, lerpPoint);
		light.color = Color.Lerp(activeColor, inactiveColor, lerpVal);

		if (lerpPoint >= realFlickerDuration) {
			realFlickerDuration = FlickerDuration + Random.Range(-Randomness, Randomness);
			lerpPoint = 0;
			var tmp = activeColor;
			activeColor = inactiveColor;
			inactiveColor = tmp;
		}
	}
}
