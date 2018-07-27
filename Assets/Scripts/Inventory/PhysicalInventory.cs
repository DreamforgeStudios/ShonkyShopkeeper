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
		SaveManager.LoadOrInitializeInventory(defaultInventory);
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
				if (instance.IsNew) {
					GameObject obj;
					if (inventorySlots[i].GetPrefabInstance(out obj)) {
						// TODO, change tween / fixup.
						obj.transform.DOMove(obj.transform.position + Vector3.up, 0.7f);
						obj.GetComponent<Rotate>().Enable = true;
					}

					if (instance.Quality == Quality.QualityGrade.Mystic) {
						AchievementManager.Get("item_quality_01");
					}
					// TODO: should this pop up on the cutting screen or when you go back to the inventory???
					if (instance.item != null) {
						var type = instance.item.GetType();
						if (type == typeof(Jewel)) {
							AchievementManager.Get("cut_jewel_01");
						} else if (type == typeof(ChargedJewel)) {
							AchievementManager.Get("charged_jewel_01");
						} else if (type == typeof(Brick)) {
							AchievementManager.Get("brick_01");
						} else if (type == typeof(Shell)) {
							AchievementManager.Get("golem_shell_01");
						}
					}
				}
			}// else {
				// Set slot to null, incase something was previously in the slot.
				//inventorySlots[i].RemoveItem();
			//}
		}
	}

	public void Clear() {
		for (int i = 0; i < inventorySlots.Count; i++) {
			inventorySlots[i].RemoveItem();
		}
	}

	public Slot GetSlotAtIndex(int index) {
        Debug.Log("getting slot index " + index);
		return inventorySlots[index];
	}

	public void LoadDefaultInventory() {
		SaveManager.LoadFromTemplate(defaultInventory);
		Clear();
		PopulateInitial();
	}

}
