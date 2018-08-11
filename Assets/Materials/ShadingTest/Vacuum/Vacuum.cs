using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Vacuum : MonoBehaviour {
	public GameObject holeObj;
	public Material mat;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = holeObj.transform.position;
		mat.SetVector("_PullPos", new Vector4(pos.x, pos.y, pos.z, 1));
		Debug.Log(String.Format("pull vector is {0}", new Vector4(pos.x, pos.y, pos.z, 1)));
	}

	private void OnDrawGizmos() {
		Gizmos.DrawWireSphere(holeObj.transform.position, mat.GetFloat("_Range"));
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(holeObj.transform.position, mat.GetFloat("_SoftRange"));
	}
}
