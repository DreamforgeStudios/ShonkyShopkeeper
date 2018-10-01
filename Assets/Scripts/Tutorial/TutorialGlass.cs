using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;

[System.Serializable]
public class ObjectInfo {
	public GameObject Object;
	public Vector3 Offset;
	public string Heading;
	public string Description;
}

[System.Serializable]
public class ObjectArray {
	public ObjectInfo[] ObjectInfos;
}

public class TutorialGlass : MonoBehaviour {
	public TutorialPanel PanePrefab;
	public ObjectArray[] ObjectPresets;
	public Canvas Canvas;
	[HideInInspector]
	public int Index;
	//public int InfoIndex;

	private List<GameObject> activeObjects = new List<GameObject>();

	public void EnableOverlay() {
		gameObject.SetActive(true);
		Load();
	}

	public void DisableOverlay() {
		// Bad for performance, use object pooling if its important.
		for (int i = 0; i < activeObjects.Count; i++) {
			Destroy(activeObjects[i]);
		}
		
		activeObjects.Clear();
		gameObject.SetActive(false);
	}

	private void Load() {
		SetIndex(Index);
	}

	private void SetIndex(int index) {
		ObjectInfo[] objectInfos = ObjectPresets[index].ObjectInfos;

		for (int i = 0; i < objectInfos.Length; i++) {
			// Object is UI element.
            TutorialPanel clone = Instantiate(PanePrefab, Vector3.zero, Quaternion.identity, gameObject.transform);
			RectTransform rect = objectInfos[i].Object.GetComponent<RectTransform>();
			if (rect != null) {
                clone.GetComponent<RectTransform>().position = rect.position + objectInfos[i].Offset;
			// Object is regular game object.
			} else {
                clone.GetComponent<RectTransform>().position = Utility.ConvertToScreenPoint(objectInfos[i].Object.transform.position) + objectInfos[i].Offset;
			}
			
            clone.TextHeading.text = objectInfos[i].Heading;
            clone.TextDescription.text = objectInfos[i].Description;
			
			activeObjects.Add(clone.gameObject);
		}
	}
	

	public int DebugIndex;
	[Button("Set Debug Index")]
	private void DebugIdx() {
		SetIndex(DebugIndex);
	}
	
	private void OnDrawGizmos() {
		/*
		var objectInfos = ObjectPresets[DebugIndex].ObjectInfos;
		CanvasScaler scaler = Canvas.GetComponent<CanvasScaler>();
		Vector2 scaleMultiplier = new Vector2(scaler.referenceResolution.x / Screen.width, scaler.referenceResolution.y / Screen.height);

		for (int i = 0; i < objectInfos.Length; i++) {
			Vector2 screenPos = Utility.ConvertToScreenPoint(objectInfos[i].Object.transform.position + objectInfos[i].Offset);
			var pos = (Vector2) screenPos * scaleMultiplier;
			Gizmos.DrawWireSphere(pos, 20f);
		}
		*/
	}
}
