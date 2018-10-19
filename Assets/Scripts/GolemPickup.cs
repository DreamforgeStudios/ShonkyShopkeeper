using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
//using UnityEditor.Experimental.UIElements;
//using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine;
using UnityEngine.AI;
//using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using UnityEngine.XR.WSA.WebCam;

public class GolemPickup : MonoBehaviour {
    //Currently held golem
    private int pickedUpGolemSlot;
    private GameObject pickedupGolem;
    //List of returning golems
    List<int> golems = new List<int>();

    //Grabbing Variables
    private Vector3 modifiedMousePos;
    private Vector3 mousePos;
    private bool overPortal, overNPC, portalReturnPlaying, holdingSound = false;
    private bool holding, textboxShowing = false;
    private Rigidbody golemRb;
    private Vector3 boundedPos;
    private Vector3 lastPos;

    //Exit Portal Location - used for 'respawning' returning golems
    public Vector3 exitPortalLocation;
    
    //Portal location used for tutorial
    public GameObject portal;

    //Reference to the pouch itemInstance
    public ItemInstance pouch;

    //Need Reference to physical inventory to spawn items
    public GameObject inventory;
    private PhysicalInventory inv;
    public TutorialPhysicalInventory tutInv;
    public TutorialPhysicalShonkyInventory tutShonkyInv;
    public PhysicalShonkyInventory shonkyInv;
    public TutorialManager tutManager;

    //Radial progress bar
    public Image RadialTimer1, RadialTimer2, RadialTimer3;
    private List<Image> _timers;
    public List<DateTime> times = new List<DateTime>();
    
    //Particle system to highlight items to be inspected
    public GameObject particles;
    private GameObject particleChild;
    
    //Rune indicator for tutorial
    public GameObject runeIndicatorPrefab;
    private GameObject runeIndicator;
    public Canvas mainCanvas;
    private bool enabledPortalRune;
    
    //To reset NPC spawner interaction
    public NPCSpawner spawner;
    public NPCInteraction NPCinteraction;

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
        if (Input.GetMouseButton(0) && GameManager.Instance.canUseTools) {
            GolemGrab();
        } 
        else if (overPortal) {
            Debug.Log("Sending to Mine");
            if (GameManager.Instance.BarterTutorial)
            {
                ResetGolem();
                return;
            }
            if (GameManager.Instance.InTutorial)
            {
                GameManager.Instance.WaitingForTimer = true;
                if (!GameManager.Instance.SendToMine)
                    tutManager.NextInstruction();
                
                RemovePortalRune();
            }
            SFX.StopSpecific("Golem Struggle Voices");
            SFX.Play("Portal_Suck",1f,1f,0f,false,0f);
            int index = GetGolemSlot();
            Mine.Instance.AddGolemAndTime(System.DateTime.Now, index);
            SetGolemInMine(index, true);
            SaveManager.SaveShonkyInventory();
            pickedupGolem.SetActive(false);
            pickedupGolem = null;
            overPortal = false;
        } else if (overNPC) {
            Debug.Log("Sending to Barter");
        
            if (GameManager.Instance.BarterTutorial)
            {
                GameManager.Instance.BarterTutorial = false;
                GameManager.Instance.BarterNPC = false;
                GameManager.Instance.introducedNPC = false;
                PlayerPrefs.SetInt("TutorialDone", 1);
            }
            
            //SFX.Play("sound");
            int index = GetGolemSlot();
            Debug.Log(index + " is the index");
            if (index != -1)
            {
                GameManager.Instance.ShonkyIndexTransfer = index;
                pickedupGolem = null;
                overNPC = false;
                spawner.isInteracting = false;
                SaveManager.SaveShonkyInventory();
                ResetGolem();
                SceneManager.LoadScene("Barter");
            }
            else
            {
                ResetGolem();
            }
        }

        if (Input.GetMouseButtonUp(0) && pickedupGolem != null)
        {
            ResetGolem();
        }

        UpdateUITimer();
    }

    private int GetGolemSlot()
    {
        if (!GameManager.Instance.InTutorial)
        {
            for (int i = 0; i < shonkyInv.amountOfSlots; i++)
            {
                Debug.Log("Checking slot " + i + " of " + shonkyInv.amountOfSlots);
                PenSlot slot = shonkyInv.shonkySlots[i];
                GameObject obj;
                if (slot.GetPrefabInstance(out obj))
                {
                    Debug.Log("obj name is " + obj.name);
                    if (obj == pickedupGolem)
                        return slot.index;
                }
            }
            return -1;
        }
        else
        {
            for (int i = 0; i < tutShonkyInv.amountOfSlots; i++)
            {
                PenSlot slot = tutShonkyInv.shonkySlots[i];
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
    }
    
    private void SetGolemInMine(int index, bool inMine) {
        ItemInstance inst;
        Debug.Log("Index " + index + " is in mine : " + inMine);
        if (ShonkyInventory.Instance.GetItem(index, out inst))
        {
            inst.InMine = inMine;
        }
    }


    private void GolemGrab()
    {
        //Debug.Log("Casting Ray");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawLine(ray.origin, ray.direction, Color.green);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 40))
        {
            //If a golem
            if (hit.transform.gameObject.CompareTag("Golem") && pickedupGolem == null)
            {
                //If not holding a pouch
                if (!hit.transform.gameObject.GetComponent<ShonkyWander>().IsHoldingPouch())
                {
                    GameManager.pickedUpGolem = true;
                    pickedupGolem = hit.transform.gameObject;
                    HoldGolem(hit);
                }
                else
                {
                    //If room in the inventory, add the pouch
                    int pouchSlot = Inventory.Instance.InsertItem(pouch);
                    pickedupGolem = hit.transform.gameObject;
                    int golemIndex = GetGolemSlot();
                    if (pouchSlot > -1)
                    {
                        //Reset golem and set pouch to inventory
                        SFX.Play("Golem Exclaim Voices", 1f, 1f, 0f, false, 0f);
                        hit.transform.gameObject.GetComponent<ShonkyWander>().RemovePouch();
                        Slot insertedSlot;
                        if (!GameManager.Instance.InTutorial)
                        {
                            insertedSlot = inv.GetSlotAtIndex(pouchSlot);
                        }
                        else
                        {
                            insertedSlot = tutInv.GetSlotAtIndex(pouchSlot);
                        }

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

                        if (GameManager.Instance.InTutorial && !GameManager.Instance.MineGoleminteractGolem)
                        {
                            if (GameManager.Instance.ReturnPouch)
                            {
                                tutManager.NextInstruction();
                                tutShonkyInv.RemoveSpecificRune(pickedupGolem);
                                GameManager.Instance.ReturnPouch = false;
                                GameManager.Instance.MineGoleminteractGolem = true;
                                RemovePortalRune();
                                Camera.main.GetComponent<CameraTap>().HighlightButton();
                            }
                        }
                        
                        //Move pouch to slot from golem position
                        GameObject newPouch = insertedSlot.prefabInstance;
                        newPouch.transform.position = pickedupGolem.transform.position;

                        Vector3 midwayPos = (Camera.main.transform.position + newPouch.transform.position)/2;
                        newPouch.transform.DOMove(midwayPos, 1f, false).SetEase(Ease.OutBack).OnComplete(() =>
                            newPouch.transform.DOMove(insertedSlot.transform.position, 2f, false)
                                .SetEase(Ease.OutBack));
                        pickedupGolem = null;
                    }
                    else
                    {
                        HoldGolem(hit);
                    }
                }
            }
            else if (pickedupGolem != null)
            {
                //Debug.Log("Calling hold golem");
                HoldGolem();
            }
            else if (Mine.Instance.ReadyToCollect() && hit.transform.gameObject.CompareTag("PortalEntry")
                                                    && pickedupGolem == null)
            {
                //Stop Sound
                SFX.StopSpecific("Mine_portal_fini");
                
                golems = null;
                golems = Mine.Instance.ReturnReadyGolems();
                foreach (int golem in golems)
                {
                    ReturnGolem(golem);
                }
            }
        } else if (pickedupGolem != null)
        {
            HoldGolem();
        }
    }

    private void ResetGolem() {
        if (pickedupGolem != null) {
            //Debug.Log("Resetting Golem");
            holdingSound = false;
            SFX.StopSpecific("Golem Struggle Voices");            
            Vector3 direction = (pickedupGolem.transform.position - lastPos).normalized;
            //Vector3 direction = Input.GetTouch(0).deltaPosition;
            Debug.DrawLine(pickedupGolem.transform.position,lastPos,Color.green);
            golemRb.useGravity = true;
            golemRb.AddForce(direction * 3000f);
            pickedupGolem.GetComponent<ShonkyWander>().FloatToPen();
            GameManager.pickedUpGolem = false;
            pickedupGolem = null;
            overNPC = false;
            overPortal = false;
            
            if (GameManager.Instance.BarterTutorial)
            {
                if (GameManager.Instance.introducedNPC && GameManager.Instance.OfferNPC)
                {
                    BarterTutorial.Instance.StartShonkyParticles();
                    
                    GameManager.Instance.OfferNPC = true;
                    GameManager.Instance.BarterNPC = false;
                    
                } else if (GameManager.Instance.introducedNPC && !GameManager.Instance.OfferNPC)
                {
                    BarterTutorial.Instance.RemoveShonkyParticles();
                }
            }
        }
    }

    //This method is used to give returning golems resource pouches
    private void ReturnGolem(int golem)
    {
        SetGolemInMine(golem, false);
        SetGolemBagType(golem);
        //Stop portal sound and play return sound
        portalReturnPlaying = true;
        //SFX.Play("sound");
        
        GameObject physicalRep = GetGolemObj(golem);
        physicalRep.SetActive(true);
        physicalRep.transform.position = exitPortalLocation;
        //SFX.Play("sound");
        physicalRep.GetComponent<ShonkyWander>().enableNavmesh = true;
        physicalRep.GetComponent<NavMeshAgent>().enabled = true;
        physicalRep.GetComponent<ShonkyWander>().pickedUp = false;
        physicalRep.GetComponent<ShonkyWander>().HoldPouch();
        SaveManager.SaveShonkyInventory();
        if (GameManager.Instance.InTutorial && !GameManager.Instance.MineGoleminteractGolem)
        {
            if (GameManager.Instance.HasMinePouch)
            {
                tutManager.HideExposition();
                GameManager.Instance.canUseTools = false;
                GameObject highlightedGolem = tutShonkyInv.ReturnSingleGolem();
                tutManager.StartDialogue(tutManager.tapPouch, tutManager.openPouch, tutManager.mainCanvas, 
                    highlightedGolem, false);
                InstructionBubble.onInstruction += () => GameManager.Instance.canUseTools = true;
                tutManager.MoveInstructionScroll();
                GameManager.Instance.ReturnPouch = true;
            }
        }
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
        if (!GameManager.Instance.InTutorial)
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
        else
        {
            GameObject obj;
            ItemInstance item;
            Debug.Log("Slot is " + slot);
            PenSlot pSlot = tutShonkyInv.shonkySlots[slot];
            if (pSlot.GetPrefabInstance(out obj))
            {
                Debug.Log("Found gameobject");
                return obj;
            }

            Debug.Log("No gameobject");
            return null;
        }
    }

    private void HoldGolem(RaycastHit hit)
    {
        //SFX.Play("sound");
        lastPos = pickedupGolem.transform.position;
        golemRb = pickedupGolem.GetComponent<Rigidbody>();
        golemRb.useGravity = false;
        pickedupGolem.GetComponent<ShonkyWander>().pickedUp = true;
        pickedupGolem.GetComponent<ShonkyWander>().PickUpAnimation(true);
        pickedupGolem.GetComponent<ShonkyWander>().StopFloat();
        pickedupGolem.GetComponent<NavMeshAgent>().enabled = false;
        
        //Clamp rotation to avoid unusual spins.
        float rotZ = ClampAngle(pickedupGolem.transform.eulerAngles.z, -35f, 35f);
        float rotX = ClampAngle(pickedupGolem.transform.eulerAngles.x, -35f, 35f);
        pickedupGolem.transform.localEulerAngles = new Vector3(rotX,pickedupGolem.transform.localEulerAngles.y,rotZ);
        
        //Clamp position
        modifiedMousePos = Camera.main.ScreenPointToRay(Input.mousePosition).GetPoint(7.4f);
        float XPos = Mathf.Clamp(modifiedMousePos.x, -6f, 5f);
        float ZPos = Mathf.Clamp(modifiedMousePos.z, -5.45f, -1.8f);
        float YPos = Mathf.Clamp(modifiedMousePos.y, -1.7f, 2.6f);
        boundedPos = new Vector3(XPos,YPos,ZPos);
        //golemRb.MovePosition(boundedPos);
        pickedupGolem.transform.position = boundedPos;

        if (!holdingSound)
        {
            SFX.Play("Golem Struggle Voices",1f,1f,0f,true,0f);
            holdingSound = true;
        }

        if (GameManager.Instance.InTutorial)
        {
            if (!GameManager.Instance.SendToMine && !enabledPortalRune && !GameManager.Instance.MineGoleminteractGolem)
            {
                Debug.Log("Starting portal rune indicator");
                //Remove rune indicator from this golem and add to portal
                runeIndicator = Instantiate(runeIndicatorPrefab, mainCanvas.transform);
                runeIndicator.GetComponent<TutorialRuneIndicator>().SetPosition(portal,false);
                enabledPortalRune = true;
            }
        }

        CheckIfOverPortal();
        CheckIfOverNPC();
    }
    
    //Secondary function which takes the existing pickedup golem as the parameter
    private void HoldGolem()
    {
        if (GameManager.Instance.BarterTutorial)
        {
            if (GameManager.Instance.OfferNPC)
            {
                BarterTutorial.Instance.RemoveShonkyParticles();
                GameManager.Instance.OfferNPC = true;
                GameManager.Instance.BarterNPC = false;
                NPCinteraction.NPCHit.GetComponent<NPCWalker>().EnableOfferParticles();
            }
        }
        //SFX.Play("sound");
        lastPos = pickedupGolem.transform.position;
        golemRb = pickedupGolem.GetComponent<Rigidbody>();
        golemRb.useGravity = false;
        pickedupGolem.GetComponent<ShonkyWander>().pickedUp = true;
        pickedupGolem.GetComponent<ShonkyWander>().PickUpAnimation(true);
        pickedupGolem.GetComponent<NavMeshAgent>().enabled = false;
        
        //Clamp rotation to avoid unusual spins.
        float rotZ = ClampAngle(pickedupGolem.transform.eulerAngles.z, -35f, 35f);
        float rotX = ClampAngle(pickedupGolem.transform.eulerAngles.x, -35f, 35f);
        pickedupGolem.transform.localEulerAngles = new Vector3(rotX, pickedupGolem.transform.localEulerAngles.y, rotZ);
        
        //Clamp position
        modifiedMousePos = Camera.main.ScreenPointToRay(Input.mousePosition).GetPoint(7.4f);
        float XPos = Mathf.Clamp(modifiedMousePos.x, -6f, 5f);
        float ZPos = Mathf.Clamp(modifiedMousePos.z, -5.45f, -1.8f);
        float YPos = Mathf.Clamp(modifiedMousePos.y, -1.7f, 2.6f);
        boundedPos = new Vector3(XPos,YPos,ZPos);
        pickedupGolem.transform.position = boundedPos;
        
        if (!holdingSound)
        {
            SFX.Play("Golem Struggle Voices",1f,1f,0f,true,0f);
            holdingSound = true;
        }
        
        if (GameManager.Instance.InTutorial)
        {
            if (!GameManager.Instance.SendToMine && !enabledPortalRune && !GameManager.Instance.MineGoleminteractGolem)
            {
                Debug.Log("Starting portal rune indicator");
                //Remove rune indicator from this golem and add to portal
                runeIndicator = Instantiate(runeIndicatorPrefab, mainCanvas.transform);
                runeIndicator.GetComponent<TutorialRuneIndicator>().SetPosition(portal,false);
                enabledPortalRune = true;
            }
        }
        CheckIfOverPortal();
        CheckIfOverNPC();
    }

    private void RemovePortalRune()
    {
        if (runeIndicator != null)
            Destroy(runeIndicator);

        enabledPortalRune = false;
    }
    
    //Clamping function from: https://forum.unity.com/threads/limiting-rotation-with-mathf-clamp.171294/
    static float ClampAngle(float angle, float min, float max)
    {
        if (min < 0 && max > 0 && (angle > max || angle < min))
        {
            angle -= 360;
            if (angle > max || angle < min)
            {
                if (Mathf.Abs(Mathf.DeltaAngle(angle, min)) < Mathf.Abs(Mathf.DeltaAngle(angle, max))) return min;
                else return max;
            }
        }
        else if(min > 0 && (angle > max || angle < min))
        {
            angle += 360;
            if (angle > max || angle < min)
            {
                if (Mathf.Abs(Mathf.DeltaAngle(angle, min)) < Mathf.Abs(Mathf.DeltaAngle(angle, max))) return min;
                else return max;
            }
        }
 
        if (angle < min) return min;
        else if (angle > max) return max;
        else return angle;
    }

    private void CheckIfOverPortal()
    {
        //Continue to raycast to check if it is over entry portal
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit secondHit;
        int layerMask = LayerMask.GetMask("MinePortal");
        if (Physics.Raycast(ray, out secondHit, 10, layerMask))
        {
            if (!overPortal)
            {
                SFX.Play("Portal_sipping", 1.5f, 1f, 0f, true, 0f);
                overPortal = true;
            }
        }
        //Really dirty but need a way to prevent the sound being constantly initiated.
        else
        {
            if (overPortal)
            {
                overPortal = false;
                SFX.StopSpecific("Portal_sipping");
            }
        }
    }

    private void CheckIfOverNPC() {
        //Debug.Log("Checking if over NPC");
        //Continue to raycast to check if it is over entry portal
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit secondHit;
        int layerMask = LayerMask.GetMask("NPC");
        Debug.DrawRay(ray.origin,ray.direction,Color.yellow);
        if (Physics.Raycast(ray, out secondHit, Mathf.Infinity, layerMask))
        {
            Debug.Log("Shot ray and hit " + secondHit.transform.gameObject.name);
            if (secondHit.transform.gameObject.GetComponent<NPCWalker>().walkNormal == false)
            {
                overNPC = true;
            }
            else
            {
                overNPC = false;
            }
        }
        else
        {
            overNPC = false;
        }
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

        if (GameManager.Instance.InTutorial && !GameManager.Instance.MineGoleminteractGolem)
        {            
            if (GameManager.Instance.WaitingForTimer)
            {
                GameManager.Instance.WaitingForTimer = false;
            }
            if (GameManager.Instance.TimerComplete && !textboxShowing)
            {
                tutManager.HideExposition();
                GameManager.Instance.canUseTools = false;
                tutManager.StartDialogue(tutManager.retrieveGolem, tutManager.retrieveGolemInstruction, tutManager.mainCanvas, portal, false);
                tutManager.MoveInstructionScroll();
                InstructionBubble.onInstruction += () => GameManager.Instance.canUseTools = true;
                GameManager.Instance.HasMinePouch = true;
                GameManager.Instance.TimerComplete = false;
                textboxShowing = true;
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
            if (_timers[j].fillAmount > 0.95f)
            {
                if (GameManager.Instance.InTutorial)
                    GameManager.Instance.TimerComplete = true;
                //Need to boolean lock to prevent constant instantiation
                if (_timers[j].fillAmount > 0.99f && !portalReturnPlaying)
                {
                    SFX.Play("Mine_portal_fini",1f,1f,0f,true,0f);
                    portalReturnPlaying = true;
                }
            }
            
        }

        for (int k = amountToCollect + amountMining; k < _timers.Count; k++)
        {
            _timers[k].fillAmount = 1f;
            _timers[k].color = Color.magenta;
            _timers[k].DOFade(0.6f, 2f);
        }
    }
}
