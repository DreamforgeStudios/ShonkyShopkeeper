using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility {
    public static Vector3 RotateAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
        return Quaternion.Euler(angles) * (point - pivot) + pivot;
    }
}
