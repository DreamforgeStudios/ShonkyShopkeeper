using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility {
    public static Vector3 RotateAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
        return Quaternion.Euler(angles) * (point - pivot) + pivot;
    }

	// Calculates the angle between 2 vectors.
	public static float CalculateAngle(Vector2 v1, Vector2 v2) {
		v1.Normalize();
		v2.Normalize();

		float dot = Vector2.Dot(v1, v2);
		float det = v1.x * v2.y - v2.x * v1.y;

		return Mathf.Atan2(det, dot) * Mathf.Rad2Deg;
	}
	
	public static Vector3 ConvertToWorldPoint(Vector3 screenPoint, float z = 10) {
		screenPoint.z = z;
		return Camera.main.ScreenToWorldPoint(screenPoint);
	}

	public static Vector3 ConvertToScreenPoint(Vector3 worldPoint) {
		return Camera.main.WorldToScreenPoint(worldPoint);
	}
}
