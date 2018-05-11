using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemDict {
	public Item.ItemType type;
	public Item.GemType gemType;
	public GameObject prefab;
}

public class ItemLookup : MonoBehaviour {
	public static ItemLookup instance = null;

	public List<ItemDict> itemDict;

	void Awake () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy(gameObject);
		}
	}

    /* Commented out while fixing item & inventory
	public GameObject LookupItem(Item item) {
		foreach (ItemDict dict in itemDict) {
			if (item.itemType == dict.type && item.gemType == dict.gemType) {
				return dict.prefab;
			}
		}

		return null;
	}
    */
}
