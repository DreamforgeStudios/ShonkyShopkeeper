using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

// Simple inventory populator.
// Might be moved to toolbox in future.
public class PhysicalInventory : MonoBehaviour {
	// Maybe change this to slot evenutally.
	public List<Slot> inventorySlots;
	// In the situation where we haven't saved an inventory before.
	public Inventory defaultInventory;

	// Use this for initialization
	void Start () {
		// Load example.
		SaveManager save = ScriptableObject.CreateInstance<SaveManager>();
		save.LoadOrInitializeInventory(defaultInventory);
        //Inventory.InitializeFromDefault(defaultInventory);
		inventorySlots = new List<Slot>();
		inventorySlots.AddRange(GameObject.FindObjectsOfType<Slot>());
		
		inventorySlots.Sort((a, b) => a.index - b.index);

		PopulateInitial();
	}

	public void PopulateInitial() {
		for (int i = 0; i < inventorySlots.Count; i++) {
			ItemInstance instance;
			// If an object exists at the specified location.
			if (Inventory.Instance.GetItem(i, out instance)) {
				inventorySlots[i].SetItem(instance);
				if (instance.isNew) {
					GameObject obj;
					if (inventorySlots[i].GetPrefabInstance(out obj)) {
						// TODO, change tween / fixup.
						obj.transform.DOMove(obj.transform.position + Vector3.up, 0.7f);
					}
				}
			}
		}
	}

	public Slot GetSlotAtIndex(int index) {
		return inventorySlots[index];
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
