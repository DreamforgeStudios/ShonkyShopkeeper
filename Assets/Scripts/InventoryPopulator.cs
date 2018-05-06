using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPopulator : MonoBehaviour {
	// Maybe change this to slot evenutally.
	public List<Slot> inventorySlots;

	// Use this for initialization
	void Start () {
		inventorySlots = new List<Slot>();
		inventorySlots.AddRange(GameObject.FindObjectsOfType<Slot>());
		
		inventorySlots.Sort((a, b) => a.index - b.index);

		PopulateWithJunk();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Populate() {
		for (int i = 0; i < inventorySlots.Count; i++) {
			// Not using drawer tabs for now -- default to drawer 0.
			//inventorySlots[i].SetItem(Inventory.GetItem(0, i));
		}
	}

	public void PopulateWithJunk() {
		// Error.
		//Inventory.GenerateNewInventory();
		//Debug.Log("Inventory: " + Inventory.ReturnInventory());
		// Should return null.
		//Debug.Log("Inventory[0,0]: " + Inventory.GetItem(0, 0));

		/*
		for (int i = 0; i < inventorySlots.Count; i++) {
			Item.ItemType type = (Item.ItemType)Random.Range(0, 7);
			Item.GemType gemType = Item.GemType.NotGem;
			Item item;
			if (type == Item.ItemType.Gem || type == Item.ItemType.Jewel || type == Item.ItemType.ChargedJewel) {
				gemType = (Item.GemType)Random.Range(1, 5);
				item = new Gem(gemType);
			} else {
				item = new Brick(Quality.QualityGrade.Passable);
			}

			Inventory.AddItem(item);
		}

		Populate();
		*/
	}
}
