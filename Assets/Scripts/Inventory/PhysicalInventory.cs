using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;

// Simple inventory populator.
// Might be moved to toolbox in future.
public class PhysicalInventory : MonoBehaviour {
	// Maybe change this to slot evenutally.
	public List<Slot> inventorySlots;
	// In the situation where we haven't saved an inventory before.
	public Inventory defaultInventory;
	// Points that indicate a path for new objects to fly into the inventory.
	// A final point will be added, which will be the item slot position.
	public List<Vector3> FlyInPoints = new List<Vector3>();

	public GameObject particles;

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
			Debug.Log(string.Format("Checking slot {0} out of {1}", i,inventorySlots.Count));
			// If an object exists at the specified location.
			if (Inventory.Instance.GetItem(i, out instance)) {
				inventorySlots[i].SetItem(instance);
				//If a resource pouch, modify its colour to represent its gem type
				//Debug.Log(string.Format("Checking if pouch and this item is {0}",instance.item.GetType()));
				if (instance.item != null && instance.item.GetType() == typeof(ResourceBag))
				{
					GameObject obj;
					if (inventorySlots[i].GetPrefabInstance(out obj))
						obj.GetComponent<SackHueChange>().UpdateCurrentColor(instance.pouchType);
				}
					
				//If a new item check for prefab and achievments
				if (instance.IsNew) {
					GameObject obj;
					if (inventorySlots[i].GetPrefabInstance(out obj)) {
						// TODO, change tween / fixup.
						//obj.transform.DOMove(obj.transform.position + Vector3.up, 0.7f);
						//DoFlyIn(obj, inventorySlots[i]);
						//obj.GetComponent<Rotate>().Enable = true;
						Instantiate(particles, inventorySlots[i].transform.position, Quaternion.identity);
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

	private void DoFlyIn(GameObject obj, Slot slot) {
		obj.transform.position = FlyInPoints[0];
		Sequence seq = DOTween.Sequence();
		for (int i = 1; i < FlyInPoints.Count; i++) {
			seq.Append(obj.transform.DOMove(FlyInPoints[i], 1.2f).SetEase(Ease.OutBack));
		}

		seq.Append(obj.transform.DOMove(slot.transform.position, .7f).SetEase(Ease.OutCirc));
		seq.Play();
	}

	public void Clear() {
		for (int i = 0; i < inventorySlots.Count; i++) {
			inventorySlots[i].RemoveItem();
		}
	}

	public Slot GetSlotAtIndex(int index) {
        //Debug.Log("getting slot index " + index);
		return inventorySlots[index];
	}

	public void LoadDefaultInventory() {
		SaveManager.LoadFromTemplate(defaultInventory);
		Clear();
		PopulateInitial();
	}

	[Button("Repopulate")]
	private void RepopulateInventory() {
		Clear();
		PopulateInitial();
	}

	private void OnDrawGizmos() {
		foreach (var pos in FlyInPoints) {
			Gizmos.DrawWireSphere(pos, .1f);
		}
	}
}
