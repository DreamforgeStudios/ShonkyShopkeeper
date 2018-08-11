using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RadialBar : MonoBehaviour {
	public Material RadialMaterial;
	public List<Vector4> Points;
	
	// Use this for initialization
	void Start () {
		UpdateMaterial();
	}
	
	// Update is called once per frame
	void Update () {
	}

	private void UpdateMaterial() {
		RadialMaterial.SetVectorArray("_Points", Points);
		RadialMaterial.SetInt("_PointsLength", Points.Count);
	}

	public void AddPoint(Vector4 point) {
		Points.Add(point);
		UpdateMaterial();
	}

	public void RemovePoint(Vector4 point) {
		Points.Remove(point);
		UpdateMaterial();
	}
}
