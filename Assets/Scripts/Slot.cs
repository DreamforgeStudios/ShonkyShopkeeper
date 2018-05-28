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
		// Instantiate as a child of this transform, though don't pay too much attention to this because it can get muddled by SetItemInstantiated().
		this.prefabInstance = Instantiate(instance.item.physicalRepresentation, transform.position, RandomRotation(transform.rotation), this.transform);
	}

	// Use this method if you don't want the slot to spawn a new object.
	public void SetItemInstantiated(ItemInstance instance, GameObject prefabInstance) {
		this.itemInstance = instance;
		prefabInstance.transform.SetParent(this.transform);
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
		if (this.itemInstance == null || this.itemInstance.item == null) { //|| this.itemInstance.item.GetType() == typeof(Empty)) {
			item = null;
			return false;
		}

		item = this.itemInstance.item;
		Debug.Log("got item.");
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
	/*
	private void Update() {
		if (Input.GetMouseButtonDown(0)) {
			this.prefabInstance.transform.rotation = RandomRotation(this.prefabInstance.transform.rotation);
		}
	}
	*/

	// Experimental...
	private Quaternion RandomRotation(Quaternion currentRotation) {
		float randMax = 100f;
		return currentRotation * Quaternion.Euler(Random.Range(-randMax, randMax), Random.Range(-randMax, randMax), Random.Range(-randMax, randMax));
	}
}
