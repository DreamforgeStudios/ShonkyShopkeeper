using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour {
	public int index = 0;

	// Only public for debugging purposes.
	public Item item;
	private GameObject itemObj;
	private GameObject prefabInstance;

	// TODO: it would be better if we used SetActive() etc rather than Instantiate/Destroy.
    /*
	public void SetItem(Item item) {
		this.item = item;
		GameObject itemObj = ItemLookup.instance.LookupItem(item);
		if (itemObj) {
			this.itemObj = itemObj;
			this.prefabInstance = Instantiate(this.itemObj, transform.position, transform.rotation);
		} else {
			//Debug.Log("ERROR: No GameObject found for item: ItemType: " + item.itemType + " GemType: " + item.gemType);
		}
	}
    */
	public void RemoveItem() {
		this.item = null;
		this.itemObj = null;
		Destroy(this.prefabInstance);
		this.prefabInstance = null;

	}
}
