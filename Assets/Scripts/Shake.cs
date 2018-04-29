using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour {
	public void ShakeTransform(float violence, float duration) {
		StartCoroutine(IEShakeTransform(violence, duration, transform.position));
	}

    IEnumerator IEShakeTransform(float violence, float duration, Vector3 originalPos) {
        for (float i= 0f; i < duration; i += Time.deltaTime) {
			Vector3 t = originalPos;
			t.x += Random.Range(-violence, violence);
			t.y += Random.Range(-violence, violence);
			transform.position = t;
            yield return null;
        }
    }
}
