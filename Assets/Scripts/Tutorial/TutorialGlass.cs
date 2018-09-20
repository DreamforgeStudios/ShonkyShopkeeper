using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

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
	public int InfoIndex;

	private List<GameObject> activeObjects;

	private void SetIndex(int index) {
		var objectInfos = ObjectPresets[index].ObjectInfos;

		for (int i = 0; i < objectInfos.Length; i++) {
			TutorialPanel clone = Instantiate(PanePrefab, Vector3.zero, Quaternion.identity, gameObject.transform);
			clone.GetComponent<RectTransform>().anchoredPosition =
				Utility.ConvertToScreenPoint(objectInfos[i].Object.transform.position, Canvas) + objectInfos[i].Offset;
			clone.TextHeading.text = objectInfos[i].Heading;
			clone.TextDescription.text = objectInfos[i].Description;
		}
	}
	

	public int DebugIndex;
	[Button("Set Debug Index")]
	private void DebugIdx() {
		SetIndex(DebugIndex);
	}
}
