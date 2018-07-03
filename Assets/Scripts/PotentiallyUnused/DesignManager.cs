using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DesignManager : MonoBehaviour {
	public TextMeshProUGUI textItem;
	public GameObject golumContainer;

	public List<GameObject> shonkyList;
	public int currentIndex = 0;

	public void IncrementIndex() {
		// Increment if not at max.
		currentIndex = (currentIndex >= shonkyList.Count-1) ? 0 : currentIndex + 1;
		UpdateDesign();
	}

	public void DecrementIndex() {
		// Decrement if not at min.
		currentIndex = (currentIndex <= 0) ? shonkyList.Count-1 : currentIndex - 1;
		UpdateDesign();
	}

	public GameObject GetCurrentDesign() {
		return shonkyList[currentIndex];
	}

	public void UpdateDesign() {
		if (golumContainer.transform.childCount > 0) {
			Destroy(golumContainer.transform.GetChild(0).gameObject);
		}

		Instantiate(shonkyList[currentIndex], golumContainer.transform);
	}
}