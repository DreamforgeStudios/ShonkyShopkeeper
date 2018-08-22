using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TutorialPhysicalShonkyInventory : MonoBehaviour {
    //To hold all shonkys
    public List<PenSlot> shonkySlots;
    public int amountOfSlots = 5;

    //Spot to move new golems to
    public GameObject middleOfPenEmpty;
    private Vector3 spawnPos;

    //Default Shonky Pen
    public ShonkyInventory inventory;
    //Mine inv
    public Mine mineInventory;
    
    //Particle system to highlight items to be inspected
    public GameObject particles;
    private GameObject particleChild;

    /*
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
    */
    // Use this for initialization
    void Start() {
        // Load example.
        //ShonkyInventory.InitializeFromDefault(inventory);
        Debug.Log("Initialising shonky inv");
        SaveManager.LoadOrInitializeShonkyInventory(inventory);
        SaveManager.LoadOrInitializeMineInventory(mineInventory);
       

        shonkySlots = new List<PenSlot>();
        shonkySlots.AddRange(GameObject.FindObjectsOfType<PenSlot>());
        shonkySlots.Sort((a, b) => a.index - b.index);

        PopulateInitial();
        SaveManager.SaveShonkyInventory();
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
            Debug.Log("Checking Shonky Slot: " + i);
            if (ShonkyInventory.Instance.GetItem(i, out instance)) {
                shonkySlots[i].SetItem(instance);
                GameObject obj;
                Debug.Log("Found Shonky at slot " + i);
                if (shonkySlots[i].GetPrefabInstance(out obj)) {
                    if (CheckIfInMine(i))
                    {
                        Mine.Instance.AddGolemReadyToCollect(i);
                        obj.SetActive(false);
                    }
                    else
                    {
                        obj.GetComponent<ShonkyWander>().enableNavmesh = true;
                    }
                    if (instance.IsNew) {
                        // TODO, change tween / fixup.
                        obj.transform.DOMove(obj.transform.position + Vector3.up, 0.7f);
                    }
                }
            }
        }
        //HighlightGolem();
    }
    
    private void HighlightGolem()
    {
        for (int i = 0; i < shonkySlots.Count; i++)
        {
            ItemInstance instance;
            //Debug.Log(string.Format("Checking slot {0} out of {1}", i,inventorySlots.Count));
            // If an object exists at the specified location.
            if (Inventory.Instance.GetItem(i, out instance))
            {
                if (!GameManager.Instance.MineGoleminteractGolem){
                    GameObject obj;
                    if (shonkySlots[i].GetPrefabInstance(out obj))
                    {
                        //particleChild = Instantiate(particles, obj.transform.position, obj.transform.rotation);
                        //particleChild.transform.parent = obj.transform;
                    }
                }
            }
        }
    }

    private bool CheckIfInMine(int index)
    {
        ItemInstance item;
        if (ShonkyInventory.Instance.GetItem(index, out item))
        {
            if (item.InMine)
            {
                Debug.Log(index + " is in the mine");
                return true;
            }
        }
        Debug.Log(index + " Not in Mine");
        return false;
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
