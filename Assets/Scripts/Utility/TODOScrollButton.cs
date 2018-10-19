using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TODOScrollButton : MonoBehaviour {
	public GameObject TODOList;

	public void Press() {
		TODOList.SetActive(!TODOList.activeSelf);
	}
}
