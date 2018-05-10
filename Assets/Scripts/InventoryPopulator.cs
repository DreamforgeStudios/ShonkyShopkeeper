using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPopulator : MonoBehaviour {
	// Maybe change this to slot evenutally.
	public List<Slot> inventorySlots;
	public Inventory InventoryTemplate;

	// Use this for initialization
	void Start () {
		inventorySlots = new List<Slot>();
		inventorySlots.AddRange(GameObject.FindObjectsOfType<Slot>());
		
		inventorySlots.Sort((a, b) => a.index - b.index);

		PopulateInitial();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void PopulateInitial() {
		for (int i = 0; i < inventorySlots.Count; i++) {
			ItemInstance instance;
			// If an object exists at the specified location.
			if (Inventory.Instance.GetItem(i, out instance)) {
				inventorySlots[i].SetItem(instance.item.physicalRepresentation);
			}
		}
	}

	/*
	public void PopulateWithJunk(Inventory inventory) {
        // Error.
        //Debug.Log("generating inventory");
		//Inventory.GenerateNewInventory();
		//Debug.Log("Inventory: " + Inventory.ReturnInventory());
		// Should return null.
		//Debug.Log("Inventory[0,0]: " + Inventory.GetItem(0, 0));

		
		for (int i = 0; i < inventorySlots.Count; i++) {
			Item.ItemType type = (Item.ItemType)Random.Range(0, 7);
			Item.GemType gemType = Item.GemType.Ruby;
			if (type == Item.ItemType.Gem || type == Item.ItemType.Jewel || type == Item.ItemType.ChargedJewel) {
				gemType = (Item.GemType)Random.Range(1, 5);
				//item = new Gem(gemType);
			} else {
				//item = new Brick(Quality.QualityGrade.Passable);
			}

			//inventory.AddItem(item);
		}

		Populate();
		
	}
		*/
}
