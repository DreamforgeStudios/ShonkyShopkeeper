using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GolemPickup : MonoBehaviour {
    //Currently held golem
    private int pickedUpGolemSlot;
    private GameObject pickedupGolem;
    //List of returning golems
    List<GameObject> golems = new List<GameObject>();

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

    //Reference to the shonkyInventory to set boolean flag when in mine
    //public Mine mineInventory;

    // Use this for initialization
    void Start() {
        exitPortalLocation = exitPortal.transform.position;
        inv = inventory.GetComponent<PhysicalInventory>();
        PhysicalShonkyInventory.Instance.QueryShonkyInvForMiners();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButton(0)) {
            GolemGrab();
        }
        else if (overPortal) {
            Debug.Log("Sending to Mine");
            Mine.Instance.AddGolemAndTime(System.DateTime.Now, pickedupGolem);
            SetGolemInMine(pickedupGolem, true);
            pickedupGolem.SetActive(false);
            pickedupGolem = null;
            overPortal = false;
        }
        else if (pickedupGolem != null) {
            ResetGolem();
        }
    }
    
    private void SetGolemInMine(GameObject golem, bool inMine) {
        for (int i = 0; i < PhysicalShonkyInventory.Instance.amountOfSlots; i++) {
            PenSlot slot = PhysicalShonkyInventory.Instance.shonkySlots[i];
            GameObject obj;
            if (slot.GetPrefabInstance(out obj) == golem) {
                ItemInstance instance;
                if (slot.GetItemInstance(out instance)) {
                    instance.inMine = inMine;
                    return;
                }
            }
        }
    }
    

    private void GolemGrab() {
        //Debug.Log("Casting Ray");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        //Do an accuraccy check before hand to ensure one isn't picked up and floating away
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
                foreach (GameObject golem in golems) {
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
    //This method will eventually be used to give returning golems resource pouches
    private void ReturnGolem(GameObject golem) {
        SetGolemInMine(golem, false);
        golem.transform.position = exitPortalLocation;
        golem.SetActive(true);
        golem.GetComponent<ShonkyWander>().HoldPouch();
        golem.GetComponent<NavMeshAgent>().enabled = true;
        golem.GetComponent<ShonkyWander>().pickedUp = false;


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
            Debug.Log("Distance between golem and mouse is " + Vector3.Distance(pickedupGolem.transform.position, modifiedMousePos));
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
        Debug.Log(overPortal + " is over portal");
    }
}
