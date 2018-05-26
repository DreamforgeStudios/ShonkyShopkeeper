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

	public void Clear() {
		for (int i = 0; i < inventorySlots.Count; i++) {
			inventorySlots[i].RemoveItem();
		}
	}

	public Slot GetSlotAtIndex(int index) {
		return inventorySlots[index];
	}

	public void LoadDefaultInventory() {
		SaveManager save = ScriptableObject.CreateInstance<SaveManager>();
		save.LoadFromTemplate(defaultInventory);
		Clear();
		PopulateInitial();
	}

}
