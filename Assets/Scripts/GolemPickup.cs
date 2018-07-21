using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GolemPickup : MonoBehaviour {
    //Currently held golem
    private int pickedUpGolemSlot;
    private GameObject pickedupGolem;
    //List of returning golems
    List<int> golems = new List<int>();

    //Grabbing Variables
    private Vector3 modifiedMousePos;
    private Vector3 mousePos;
    private bool overPortal = false;
    private bool holding = false;

    //Exit Portal Location - used for 'respawning' returning golems
    public GameObject exitPortal;
    private Vector3 exitPortalLocation;

    //Reference to the pouch itemInstance
    public ItemInstance pouch;

    //Need Reference to physical inventory to spawn items
    public GameObject inventory;
    private PhysicalInventory inv;
    public PhysicalShonkyInventory shonkyInv;

    //Reference to the shonkyInventory to set boolean flag when in mine
    //public Mine mineInventory;

    // Use this for initialization
    void Start() {
        exitPortalLocation = exitPortal.transform.position;
        inv = inventory.GetComponent<PhysicalInventory>();
    }

    // Update is called once per frame
    void Update() {
        //Debug.Log(overPortal + " is over portal");
        if (Input.GetMouseButton(0)) {
            GolemGrab();
        } 
        else if (overPortal) {
            Debug.Log("Sending to Mine");
            int index = GetGolemSlot();
            Mine.Instance.AddGolemAndTime(System.DateTime.Now, index);
            SetGolemInMine(index, true);
            SaveManager.SaveShonkyInventory();
            pickedupGolem.SetActive(false);
            pickedupGolem = null;
            overPortal = false;
        }
        else if (pickedupGolem != null) {
            ResetGolem();
        }
    }

    private int GetGolemSlot()
    {
        for (int i = 0; i < shonkyInv.amountOfSlots; i++) {
            PenSlot slot = shonkyInv.shonkySlots[i];
            GameObject obj;
            if (slot.GetPrefabInstance(out obj) == pickedupGolem)
            {
                return slot.index;
            }
            else
            {
                return -1;
            }
            
        }

        return -1;
    }
    
    private void SetGolemInMine(int index, bool inMine) {
        ItemInstance inst;
        Debug.Log("Index " + index + " is in mine : " + inMine);
        if (ShonkyInventory.Instance.GetItem(index, out inst))
        {
            inst.InMine = inMine;
        }
    }
    

    private void GolemGrab() {
        //Debug.Log("Casting Ray");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        //Do an accuracy check before hand to ensure one isn't picked up and floating away
        holding = CheckAccuracy();
        if (!holding)
            ResetGolem();

        if (Physics.Raycast(ray, out hit, 20)) {
            //If a golem
            if (hit.transform.gameObject.tag == "Golem") {
                //Check we don't currently have another golem held, if so reset it
                if (pickedupGolem != null && pickedupGolem != hit.transform.gameObject) {
                    Debug.Log("Reseting golem");
                    ResetGolem();
                }
                else {
                    //If not holding a pouch
                    if (!hit.transform.gameObject.GetComponent<ShonkyWander>().IsHoldingPouch()) {
                        GameManager.pickedUpGolem = true;
                        pickedupGolem = hit.transform.gameObject;
                        HoldGolem(hit);
                    }
                    else {
                        //If room in the inventory, add the pouch
                        int pouchSlot = Inventory.Instance.InsertItem(pouch);
                        if (pouchSlot > -1) {
                            hit.transform.gameObject.GetComponent<ShonkyWander>().RemovePouch();
                            Debug.Log("Slot inserted at is " + pouchSlot);
                            Slot insertedSlot = inv.GetSlotAtIndex(pouchSlot);
                            insertedSlot.SetItem(pouch);
                            GameManager.pickedUpGolem = false;
                        }
                    }
                }
            }
            else if (Mine.Instance.ReadyToCollect() && hit.transform.gameObject.tag == "PortalExit") {
                golems = null;
                golems = Mine.Instance.ReturnReadyGolems();
                foreach (int golem in golems) {
                    ReturnGolem(golem);
                }
            }
            else {
                ResetGolem();
            }
        }
    }

    private void ResetGolem() {
        if (pickedupGolem != null) {
            pickedupGolem.GetComponent<NavMeshAgent>().enabled = true;
            GameManager.pickedUpGolem = false;
            pickedupGolem.GetComponent<ShonkyWander>().pickedUp = false;
            pickedupGolem = null;
        }
    }
    //This method is used to give returning golems resource pouches
    private void ReturnGolem(int golem)
    {
        SetGolemInMine(golem, false);
        GameObject physicalRep = GetGolemObj(golem);
        physicalRep.SetActive(true);
        physicalRep.transform.position = exitPortalLocation;
        physicalRep.GetComponent<NavMeshAgent>().enabled = true;
        physicalRep.GetComponent<ShonkyWander>().pickedUp = false;
        Debug.Log(physicalRep.GetComponent<ShonkyWander>().IsHoldingPouch() + " is holding pouch");
        physicalRep.GetComponent<ShonkyWander>().HoldPouch();
        Debug.Log(physicalRep.GetComponent<ShonkyWander>().IsHoldingPouch() + " is holding pouch");
        SaveManager.SaveShonkyInventory();
    }

    private GameObject GetGolemObj(int slot)
    {
        GameObject obj;
        ItemInstance item;
        Debug.Log("Slot is " + slot);
        PenSlot pSlot = shonkyInv.shonkySlots[slot];
        if (pSlot.GetPrefabInstance(out obj))
        {
            Debug.Log("Found gameobject");
            return obj;
        }
        Debug.Log("No gameobject");
        return null;
    }

    private void HoldGolem(RaycastHit hit) {
        pickedupGolem.GetComponent<ShonkyWander>().pickedUp = true;
        pickedupGolem.GetComponent<NavMeshAgent>().enabled = false;
        modifiedMousePos = Camera.main.ScreenPointToRay(Input.mousePosition).GetPoint(6.5f);
        pickedupGolem.transform.position = modifiedMousePos;
        CheckIfOverPortal();
    }

    private bool CheckAccuracy() {
        if (pickedupGolem != null) {
            //Debug.Log("Distance between golem and mouse is " + Vector3.Distance(pickedupGolem.transform.position, modifiedMousePos));
            if (Vector3.Distance(pickedupGolem.transform.position, modifiedMousePos) > 2f)
                return false;
            else
               return true;
        }
        return true;
    }

    private void CheckIfOverPortal() {
        //Continue to raycast to check if it is over entry portal
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit secondHit;
        int layerMask = LayerMask.GetMask("MinePortal");
        if (Physics.Raycast(ray, out secondHit, 10, layerMask)) {
            overPortal = true;
        }
        else {
            overPortal = false;
        }
        //Debug.Log(overPortal + " is over portal");
    }
}
