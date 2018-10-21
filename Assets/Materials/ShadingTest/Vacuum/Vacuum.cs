using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[ExecuteInEditMode]
public class Vacuum : MonoBehaviour {
	public bool Enable = true;
	
	public GameObject HoleObject;
	public Vector3 ObjectOffset;
	public Material[] Materials;
	public float Strength;
	[MinMaxSlider(0, 12)]
	public Vector2 MinMaxRange;

	// Use this for initialization
	void Start () {
        UpdateValues();
	}
	
	// Update is called once per frame
	void Update () {
	}

	[Button("Update Values")]
	private void UpdateValues() {
		float enable = Enable ? 1 : 0;
		
		for (int i = 0; i < Materials.Length; i++) {
			Materials[i].SetFloat("_Enabled", enable);
			Materials[i].SetFloat("_Strength", Strength);
			Materials[i].SetFloat("_Range", MinMaxRange.x);
			Materials[i].SetFloat("_SoftRange", MinMaxRange.y);
			Vector3 pos = HoleObject.transform.position + ObjectOffset;
			Materials[i].SetVector("_PullPos", new Vector4(pos.x, pos.y, pos.z, 1));
		}
	}
	
	private void OnDrawGizmos() {
		if (Materials.Length > 0) {
			Gizmos.DrawWireSphere(HoleObject.transform.position + ObjectOffset, MinMaxRange.x);
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(HoleObject.transform.position + ObjectOffset, MinMaxRange.y);
		}
	}
}
