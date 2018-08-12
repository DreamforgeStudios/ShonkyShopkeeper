using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[ExecuteInEditMode]
public class RadialBar : MonoBehaviour {
	[ShowNonSerializedField]
	public const int MAX_POINTS = 20;
	public Material RadialMaterial;
	public List<Vector4> Points;
	
	// Use this for initialization
	void Start () {
		RadialMaterial.SetVectorArray("_Points", new Vector4[MAX_POINTS]);
	}
	
	// Update is called once per frame
	void Update () {
		UpdateMaterial();
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

	public List<Vector4> GetPoints() {
		return Points;
	}
}
