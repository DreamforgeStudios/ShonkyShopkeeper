using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour {
	public int index = 0;

	// Only public for debugging purposes.
	//public Item item;
	public GameObject itemObj;
	public GameObject prefabInstance;

	// TODO: it would be better if we used SetActive() etc rather than Instantiate/Destroy.
	public void SetItem(GameObject item) {
		//this.item = item;
		//this.itemObj = item;
		this.prefabInstance = Instantiate(item, transform.position, transform.rotation);
		//Debug.Log("ERROR: No GameObject found for item: ItemType: " + item.itemType + " GemType: " + item.gemType);
	}

	public void RemoveItem() {
		//this. = null;
		this.itemObj = null;
		Destroy(this.prefabInstance);
		this.prefabInstance = null;

	}
}
