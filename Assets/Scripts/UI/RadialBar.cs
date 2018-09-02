﻿using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class RadialBar : MonoBehaviour {
	[ShowNonSerializedField]
	public const int MAX_POINTS = 30;
	public Material RadialMaterial;
	public List<Vector4> Points;
	public List<Color> Colors;
	public Color DefaultColor;

	private float cursorPosition;
	public float CursorPosition {
		get { return cursorPosition; }
		set {
			cursorPosition = value;
			RadialMaterial.SetFloat("_CursorPosition", cursorPosition);
		}
	}

	// Use this for initialization
	void Start () {
		RadialMaterial.SetVectorArray("_Points", new Vector4[MAX_POINTS]);
		RadialMaterial.SetVectorArray("_Colors", new Vector4[MAX_POINTS]);
		RadialMaterial.SetColor("_DefaultColor", DefaultColor);
	}
	
	// Update is called once per frame
	void Update () {
		UpdateMaterial();
	}

	private void UpdateMaterial() {
		if (Points.Count <= 0 || Colors.Count <= 0) return;
		
		RadialMaterial.SetVectorArray("_Points", Points);
		RadialMaterial.SetColorArray("_Colors", Colors);
		RadialMaterial.SetInt("_PointsLength", Points.Count);
		
		RadialMaterial.SetColor("_DefaultColor", DefaultColor);
	}

	public void AddPoint(Vector4 point, Color color) {
		Points.Add(point);
		Colors.Add(color);
		UpdateMaterial();
	}

	public void ChangePoint(Vector4 point, Color color) {
		int p = Points.FindIndex(x => x == point);

		if (p >= 0) {
			// TODO: don't hard code this.
			var tmp = Points[p];
			tmp.z = (float) BarterManager.Segment.Bad;
			
			Points[p] = tmp;
			Colors[p] = color;
		} else {
			Debug.LogWarning("Could not change point " + point + " because it was not present in the list of points.");
		}
	}

	// Not sure if vectors are passed by reference, but if they aren't, then this method has the potential to
	//  remove unintended points.  Use with care.
	public void RemovePoint(Vector4 point) {
		int index = Points.FindIndex(x => x == point);
		Colors.RemoveAt(index);
		Points.RemoveAt(index);
		UpdateMaterial();
	}
}
