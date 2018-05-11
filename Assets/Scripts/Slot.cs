using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour {
	public int index = 0;

	// Only public for debugging purposes.
	public ItemInstance itemInstance = null;	// Inventory backend representation.
	public GameObject prefabInstance = null;	// Inventory frontend representation.

	// TODO: it would be better if we used SetActive() etc rather than Instantiate/Destroy.
	// Use this method to set a slot's item.
	// The slot will automatically instantiate the gameobject associated with the item.
	public void SetItem(ItemInstance instance) {
		this.itemInstance = instance;
		this.prefabInstance = Instantiate(instance.item.physicalRepresentation, transform.position, transform.rotation);
	}

	// Use this method if you don't want the slot to spawn a new object.
	public void SetItemInstantiated(ItemInstance instance, GameObject prefabInstance) {
		this.itemInstance = instance;
		this.prefabInstance = prefabInstance;
	}

	// Remove the item from the slot, and destroy the associated gameobject.
	public void RemoveItem() {
		this.itemInstance = null;
		Destroy(this.prefabInstance);
		this.prefabInstance = null;
	}

	public void RemoveDontDestroy() {
		this.itemInstance = null;
		this.prefabInstance = null;
	}

	public bool GetItemInstance(out ItemInstance itemInstance) {
		if (this.itemInstance == null) {
			itemInstance = null;
			return false;
		}

		itemInstance = this.itemInstance;
		return true;
	}

	public bool GetItem(out Item item) {
		if (this.itemInstance.item == null) {
			item = null;
			return false;
		}

		item = this.itemInstance.item;
		return true;
	}

	public bool GetPrefabInstance(out GameObject prefabInstance) {
		if (this.prefabInstance == null) {
			prefabInstance = null;
			return false;
		}

		prefabInstance = this.prefabInstance;
		return true;
	}
}
