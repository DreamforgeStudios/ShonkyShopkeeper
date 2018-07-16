using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PhysicalShonkyInventory : MonoBehaviour {
    //To hold all shonkys
    public List<PenSlot> shonkySlots;
    public int amountOfSlots = 5;

    //Spot to move new golems to
    public GameObject middleOfPenEmpty;
    private Vector3 spawnPos;

    //Default Shonky Pen
    public ShonkyInventory inventory;

    private static PhysicalShonkyInventory _instance;
    public static PhysicalShonkyInventory Instance {
        get {
            if (!_instance) {
                PhysicalShonkyInventory[] tmp = Resources.FindObjectsOfTypeAll<PhysicalShonkyInventory>();
                if (tmp.Length > 0) {
                    _instance = tmp[0];
                    Debug.Log("Found shonky physical inventory as: " + _instance);
                }
                else {
                    Debug.Log("did not find shonky physical inventory.");
                    _instance = null;
                }
            }

            return _instance;
        }
    }
    // Use this for initialization
    void Start() {
        // Load example.
        //ShonkyInventory.InitializeFromDefault(inventory);
        SaveManager.LoadOrInitializeShonkyInventory(inventory);
       

        shonkySlots = new List<PenSlot>();
        shonkySlots.AddRange(GameObject.FindObjectsOfType<PenSlot>());

        PopulateInitial();
        spawnPos = middleOfPenEmpty.transform.position;
    }

    // Update is called once per frame
    void Update() {

    }

    public void PopulateInitial() {
        for (int i = 0; i < shonkySlots.Count; i++) {
            ItemInstance instance;
            // If an object exists at the specified location.
            //Debug.Log("index is " + i);
            if (ShonkyInventory.Instance.GetItem(i, out instance)) {
                shonkySlots[i].SetItem(instance);
                GameObject obj;
                if (shonkySlots[i].GetPrefabInstance(out obj)) {
                    CheckIfInMine(instance, obj);
                    if (instance.IsNew) {
                        // TODO, change tween / fixup.
                        obj.transform.DOMove(obj.transform.position + Vector3.up, 0.7f);
                    }
                    
                }
            }
        }
    }

    public void QueryShonkyInvForMiners() {
        for (int i = 0; i < shonkySlots.Count; i++) {
            ItemInstance instance;
            if (ShonkyInventory.Instance.GetItem(i, out instance)) {
                //shonkySlots[i].SetItem(instance);
                GameObject obj;
                if (shonkySlots[i].GetPrefabInstance(out obj)) {
                    CheckIfInMine(instance, obj);
                }
            }
        }
    }

    private void CheckIfInMine(ItemInstance instance, GameObject obj) {
        Debug.Log("This golem is being checked if in mine");
        if (instance.InMine) {
            Debug.Log("This is in the mine");
            obj.SetActive(false);
            Mine.Instance.AddGolemReadyToCollect(obj);
        }
        else {
            Debug.Log("This is not in the mine");
            obj.GetComponent<ShonkyWander>().enableNavmesh = true;
        }
    }

    public PenSlot GetSlotAtIndex(int index) {
        return shonkySlots[index];
    }

    public void InsertItemAtSlot(int index, ItemInstance golem, GameObject physicalRepresentation) {
        shonkySlots[index].SetItemInstantiated(golem, physicalRepresentation);
    }

    public void MoveToPen(int index) {
        PenSlot slot = GetSlotAtIndex(index);
        GameObject itemObj;
        if (slot.GetPrefabInstance(out itemObj)) {
            Transform t = itemObj.transform;
            t.DOMove(slot.transform.position + (Vector3.up), 0.7f).SetEase(Ease.OutBack).OnComplete(() =>
            t.DOMove(spawnPos, 5f).SetEase(Ease.OutBack).OnComplete(() => itemObj.GetComponent<ShonkyWander>().enableNavmesh = true));
        }
    }

	public void Clear() {
		for (int i = 0; i < shonkySlots.Count; i++) {
			shonkySlots[i].RemoveItem();
		}
	}

	public void LoadDefaultInventory() {
		SaveManager.LoadFromShonkyTemplate(inventory);
		Clear();
		PopulateInitial();
	}
}
