using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
//using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;
//using UnityEngine.XR.WSA.WebCam;

public class TutorialGolemPickup : MonoBehaviour {
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
    private Rigidbody golemRb;
    private Vector3 boundedPos;
    private Vector3 lastPos;

    //Exit Portal Location - used for 'respawning' returning golems
    public Vector3 exitPortalLocation;

    //Reference to the pouch itemInstance
    public ItemInstance pouch;

    //Need Reference to physical inventory to spawn items
    public GameObject inventory;
    private PhysicalInventory inv;
    public PhysicalShonkyInventory shonkyInv;

    //Radial progress bar
    public Image RadialTimer1, RadialTimer2, RadialTimer3;
    private List<Image> _timers;
    public List<DateTime> times = new List<DateTime>();

    // Use this for initialization
    void Start() {
        inv = inventory.GetComponent<PhysicalInventory>();
        _timers = new List<Image>()
        {
            RadialTimer1,
            RadialTimer2,
            RadialTimer3
        };
    }

    // Update is called once per frame
    void Update() {
        //Debug.Log(overPortal + " is over portal");
        if (Input.GetMouseButton(0)) {
            GolemGrab();
        } 
        else if (overPortal) {
            Debug.Log("Sending to Mine");
            //SFX.Play("sound");
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

        UpdateUITimer();
    }

    private int GetGolemSlot()
    {
        for (int i = 0; i < shonkyInv.amountOfSlots; i++) {
            PenSlot slot = shonkyInv.shonkySlots[i];
            GameObject obj;
            if (slot.GetPrefabInstance(out obj))
            {
                if (obj == pickedupGolem)
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
                    if (!hit.transform.gameObject.GetComponent<ShonkyWander>().IsHoldingPouch())
                    {
                        GameManager.pickedUpGolem = true;
                        pickedupGolem = hit.transform.gameObject;
                        HoldGolem(hit);
                    }
                    else {
                        //If room in the inventory, add the pouch
                        int pouchSlot = Inventory.Instance.InsertItem(pouch);
                        pickedupGolem = hit.transform.gameObject;
                        int golemIndex = GetGolemSlot();
                        if (pouchSlot > -1) {
                            //Reset golem and set pouch to inventory
                            //SFX.Play("sound");
                            hit.transform.gameObject.GetComponent<ShonkyWander>().RemovePouch();
                            Slot insertedSlot = inv.GetSlotAtIndex(pouchSlot);
                            insertedSlot.SetItem(pouch);
                            
                            //Get gemtype from golem and apply to pouch
                            ItemInstance instance;
                            if (ShonkyInventory.Instance.GetItem(golemIndex, out instance))
                            {
                                Item.GemType bagType = instance.pouchType;
                                ItemInstance inst;
                                if (Inventory.Instance.GetItem(pouchSlot, out inst))
                                {
                                    inst.pouchType = bagType;
                                }
                            }
                            
                            //Change pouch colour according to Gem
                            GameObject obj;
                            if (insertedSlot.GetPrefabInstance(out obj))
                                obj.GetComponent<SackHueChange>().UpdateCurrentColor(instance.pouchType);
                            
                            ResetGolem();
                        }
                        else
                        {
                            //SFX.Play("sound");
                        }
                    }
                }
            }
            else if (Mine.Instance.ReadyToCollect() && hit.transform.gameObject.CompareTag("PortalEntry")
                     && pickedupGolem == null) {
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
            //pickedupGolem.GetComponent<NavMeshAgent>().enabled = true;
            //Input.
            Debug.Log(String.Format("last pos is {0} while transform is {1}. Direction is {2}", 
                lastPos,pickedupGolem.transform.position, (pickedupGolem.transform.position - lastPos).normalized));
            Vector3 direction = (pickedupGolem.transform.position - lastPos).normalized;
            //Vector3 direction = Input.GetTouch(0).deltaPosition;
            Debug.DrawLine(pickedupGolem.transform.position,lastPos,Color.green);
            golemRb.AddForce(direction * 10000f);
            pickedupGolem.GetComponent<ShonkyWander>().FloatToPen();
            GameManager.pickedUpGolem = false;
            pickedupGolem = null;
        }
    }
    //This method is used to give returning golems resource pouches
    private void ReturnGolem(int golem)
    {
        SetGolemInMine(golem, false);
        SetGolemBagType(golem);
        GameObject physicalRep = GetGolemObj(golem);
        //SFX.Play("sound");
        physicalRep.SetActive(true);
        physicalRep.transform.position = exitPortalLocation;
        //SFX.Play("sound");
        physicalRep.GetComponent<ShonkyWander>().enableNavmesh = true;
        physicalRep.GetComponent<NavMeshAgent>().enabled = true;
        physicalRep.GetComponent<ShonkyWander>().pickedUp = false;
        physicalRep.GetComponent<ShonkyWander>().HoldPouch();
        SaveManager.SaveShonkyInventory();
    }

    private void SetGolemBagType(int index)
    {
        ItemInstance inst;
        if (ShonkyInventory.Instance.GetItem(index, out inst))
        {
            inst.pouchType = Travel.CurrentTownGemType();
        }
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

    private void HoldGolem(RaycastHit hit)
    {
        //SFX.Play("sound");
        lastPos = pickedupGolem.transform.position;
        golemRb = pickedupGolem.GetComponent<Rigidbody>();
        pickedupGolem.GetComponent<ShonkyWander>().pickedUp = true;
        pickedupGolem.GetComponent<ShonkyWander>().PickUpAnimation(true);
        pickedupGolem.GetComponent<NavMeshAgent>().enabled = false;
        modifiedMousePos = Camera.main.ScreenPointToRay(Input.mousePosition).GetPoint(6.5f);
        float XPos = Mathf.Clamp(modifiedMousePos.x, -5f, 4.5f);
        float ZPos = Mathf.Clamp(modifiedMousePos.z, -5.45f, -1.95f);
        boundedPos = new Vector3(XPos,modifiedMousePos.y,ZPos);
        golemRb.MovePosition(boundedPos);
        //pickedupGolem.transform.position = boundedPos;
        CheckIfOverPortal();
    }

    private bool CheckAccuracy() {
        if (pickedupGolem != null) {
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
            //SFX.Play("sound");
        }
        else {
            overPortal = false;
        }
        //Debug.Log(overPortal + " is over portal");
    }

    private void UpdateUITimer()
    {       
        if (Mine.Instance.GolemsInMine())
        {
            times = Mine.Instance.TimeRemaining();
            //Debug.Log(Mine.Instance.AmountOfGolemsInMine());
            ActivatePortalRings(Mine.Instance.AmountReadyToReturn(), Mine.Instance.AmountOfGolemsInMine());
        }
        else
        {
            foreach (Image timer in _timers)
            {
                timer.fillAmount = 1f;
                timer.color = Color.magenta;
                timer.DOFade(0.6f, 0.5f);
            }
        }
    }

    private void ActivatePortalRings(int amountToCollect, int amountMining)
    {
        for (int i = 0; i < amountToCollect; i++)
        {
            _timers[i].DOFade(0.8f, 2f);
            _timers[i].color = Color.yellow;
            _timers[i].fillAmount = 1f;         
            //SFX.Play("sound");
        }

        for (int j = amountToCollect; j < amountToCollect + amountMining; j++)
        {
            //SFX.Play("sound");
            TimeSpan elapsedTime = DateTime.Now - times[j];
            float milliseconds = (float)elapsedTime.TotalMilliseconds;
            _timers[j].DOFade(0.8f, 2f);
            _timers[j].color = Color.yellow;
            _timers[j].fillAmount = Mathf.Lerp(0f, 1f, milliseconds/ (Mine.Instance.MiningTime() * 1000f));
        }

        for (int k = amountToCollect + amountMining; k < _timers.Count; k++)
        {
            _timers[k].fillAmount = 1f;
            _timers[k].color = Color.magenta;
            _timers[k].DOFade(0.6f, 2f);
        }
    }
}
