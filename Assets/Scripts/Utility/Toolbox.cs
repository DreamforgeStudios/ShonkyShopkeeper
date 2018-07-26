using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening; // Tweening / nice lerping.
//using System;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting.APIUpdating;

//To access Minigames

// The toolbox class is how all user inventory interactions should be done.
public class Toolbox : MonoBehaviour {
    public enum Tool {
        Inspector,
        Forceps,
        Wand
    }

    public PhysicalInventory physicalInventory;
    public PhysicalShonkyInventory physicalShonkyInventory;
    public ItemDatabase database;

    public float selectedOutlineThickness = 2;

    //Capture the original positions and gameObjects of each tool
    private Vector3 forcepPos;
    private Vector3 inspectPos;
    private Vector3 wandPos;
    public GameObject forceps;
    public GameObject magnifyer;
    public GameObject wand;

    // A layer mask so that we only hit slots.
    public LayerMask layerMask;

    // Variables for the inspector.
    public GameObject inspectionPanel;
    public TextMeshProUGUI textHeading;
    public TextMeshProUGUI textInfo;

    // Helpers.
    //private Inventory inventoryhelper;
    private Tool currentTool;

    //Audio helpers
    public GameObject audioManager;
    private AudioSource soundEffects;
    private AudioClip cursorSelect;
    private AudioClip golumCreated;
    private AudioClip itemLift;
    private AudioClip itemDown;

    //Helpers to capture Items, transfoms, slot indexes, gameobjects, movement stages and instances when using forceps
    private Slot currentSelection = null;
    //private Slot secondSelection = null;
    private bool canSelect = true;

    // Debug.
    private Ray previousRay;

    // Use this for initialization
    void Start() {
        currentTool = Tool.Inspector;
        SwitchTool(Tool.Inspector);
        SetUpAudio();
        //inventoryhelper = Inventory.Instance;
        //forcepPos = GameObject.FindGameObjectWithTag("forcep").transform.position;
        //wandPos = GameObject.FindGameObjectWithTag("wand").transform.position;
        //inspectPos = GameObject.FindGameObjectWithTag("inspector").transform.position;
    }

    // Update is called once per frame
    void Update() {
        // Check where we are running the program.
        RuntimePlatform p = Application.platform;
        if (p == RuntimePlatform.WindowsEditor || p == RuntimePlatform.WindowsPlayer || p == RuntimePlatform.OSXEditor || p == RuntimePlatform.OSXPlayer)
            // Process mouse inputs.
            ProcessMouse();
        else if (p == RuntimePlatform.IPhonePlayer || p == RuntimePlatform.Android)
            // Process touch inputs.
            ProcessTouch();
    }

    private void SetUpAudio() {
        soundEffects = audioManager.GetComponent<ShopAudioManager>().effects;
        cursorSelect = audioManager.GetComponent<ShopAudioManager>().cursorSelect;
        golumCreated = audioManager.GetComponent<ShopAudioManager>().golumCreated;
        itemLift = audioManager.GetComponent<ShopAudioManager>().itemLift;
        itemDown = audioManager.GetComponent<ShopAudioManager>().itemDown;
    }
    private void ProcessMouse() {
        if (Input.GetMouseButtonDown(0)) {
            Cast();
        }
    }

    private void ProcessTouch() {
        if (Input.touchCount == 0) {
            return;
        }

        foreach (Touch touch in Input.touches) {
            if (touch.phase == TouchPhase.Began) {
                Cast();
            }
        }
    }

    // Get the gameobject for a specific tool.
    private GameObject ToolToObject(Tool tool) {
        switch (tool) {
            case Tool.Inspector: return magnifyer;
            case Tool.Forceps: return forceps;
            case Tool.Wand: return wand;
            default: return magnifyer;
        }
    }

    // Raycast across the scene and decide what to do.
    private void Cast() {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        previousRay = ray;

        //Debug.Log("casting...");
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
            if (hit.transform.CompareTag("Forceps") || hit.transform.CompareTag("Wand") || hit.transform.CompareTag("Magnifyer")) {
                SFX.Play("cursor_select");
                //soundEffects.clip = cursorSelect;
                //soundEffects.Play();
                switch (hit.transform.tag) {
                    case "Forceps": SwitchTool(Tool.Forceps); break;
                    case "Magnifyer": SwitchTool(Tool.Inspector); break;
                    case "Wand": SwitchTool(Tool.Wand); break;
                }
                // Must be a slot if it is not a tool.
            }
            else {
                UseTool(hit.transform.GetComponent<Slot>());
            }
        }
        else {
            HideInspector();
        }
    }

    // Switch tools, animations and all.
    public void SwitchTool(Tool tool) {
        GameObject curToolObj = ToolToObject(currentTool),
                   newToolObj = ToolToObject(tool);

        // Visual scale feedback.
        curToolObj.transform.DOScale(1f, 0.7f).SetEase(Ease.InElastic);
        newToolObj.transform.DOScale(2f, 0.7f).SetEase(Ease.InElastic);

        curToolObj.GetComponent<Outline>().OutlineWidth = 0;
        newToolObj.GetComponent<Outline>().OutlineWidth = selectedOutlineThickness;

        // Finally actually swap tools.
        currentTool = tool;

        // Hide the inspector, kinda annoying.
        HideInspector();
    }

    // Use the right tool on the slot.
    private void UseTool(Slot slot) {
        switch (currentTool) {
            case Tool.Inspector:
                UseInspector(slot);
                break;
            case Tool.Forceps:
                UseForceps(slot);
                break;
            case Tool.Wand:
                UseWand(slot);
                break;
        }
    }
    
    //Simple method to show current selection
    private Tweener MoveUp(Slot slot) {
        GameObject itemObj;
        Tweener tween = null;
        if (slot.GetPrefabInstance(out itemObj)) {
            SFX.Play("item_lift");
            Transform t = itemObj.transform;
            tween = t.DOMove(slot.transform.position + (Vector3.up), 0.7f).SetEase(Ease.OutBack);
            t.GetComponent<Rotate>().Enable = true;
        }

        return tween;
    }

    private Tweener MoveDown(Slot slot) {
        GameObject itemObj;
        Tweener tween = null;
        if (slot.GetPrefabInstance(out itemObj)) {
            SFX.Play("item_down", 1, 1, .25f);
            Transform t = itemObj.transform;
            tween = t.DOMove(slot.transform.position, 1f).SetEase(Ease.OutBounce);
            t.GetComponent<Rotate>().Enable = false;
        }

        return tween;
    }

    // Inspect an item.
    private void UseInspector(Slot slot) {
        // Can't select the same item twice, or select 2 items at once.
        // This doesn't work 100% if you abuse it (spam click), but it does for the most part, and always resets anyway.
        if (currentSelection == slot) {
            HideInspector();
            return;
        } else if (currentSelection) {
            HideInspector();
        }

        // To avoid null errors, always use the x.Get() methods, they check for you.
        Item item;
        ItemInstance instance;
        if (slot.GetItemInstance(out instance) && slot.GetItem(out item)) {
            if (item.GetType() == typeof(ResourceBag)) {
                ResourcePouchOpen(slot);
            } else {
                currentSelection = slot;
                inspectionPanel.SetActive(true);

                textHeading.text = instance.itemName;
                textInfo.text = instance.itemInfo;
                
                MoveUp(slot);
            }
        } else {
            HideInspector();
        }
    }

    // Hide the inspector and the current item (if one is selected).
    private void HideInspector() {
        inspectionPanel.SetActive(false);
        GameObject gameObj;
        if (currentSelection && currentSelection.GetPrefabInstance(out gameObj)) {
            if (currentSelection.itemInstance.IsNew) {
                UnmarkNew();
            }
            
            MoveDown(currentSelection);
            currentSelection = null;
        }
    }

    private void UnmarkNew() {
        // Unmark in backend and frontend.
        Inventory.Instance.UnMarkNew(currentSelection.index);
        currentSelection.itemInstance.IsNew = false;
        GameObject obj;
        /*
        if (currentSelection.GetPrefabInstance(out obj)) {
            Destroy(obj.GetComponent<Rotate>());
        }
        */
    }

    //Move an Item to a new slot
    private void UseForceps(Slot slot) {
        Item item;

        if (slot.GetItem(out item) && canSelect) {
            //If first selection
            if (currentSelection == null) {
                this.currentSelection = slot;
                MoveUp(slot);
                // Second selection.
            } else {
                // If the same item is selected, put it back.
                if (currentSelection == slot) {
                    Debug.Log("Same slot.");
                    MoveDown(slot);
                    currentSelection = null;
                    return;
                }

                // Make things make more sense.
                Slot slot1 = this.currentSelection;
                Slot slot2 = slot;

                // Move objects to other slot.
                GameObject obj1;
                GameObject obj2;
                // Move 1 item.
                if (slot1.GetPrefabInstance(out obj1) && slot2.GetPrefabInstance(out obj2)) {
                    // Don't let user select while we're moving.
                    // TODO: let user select while moving?
                    canSelect = false;

                    Transform t1 = obj1.transform,
                              t2 = obj2.transform;
                    
                    //SFX.Play("item_lift");
                    MoveUp(slot1)
                        .OnComplete(() => t1.DOMove(slot2.transform.position + Vector3.up, 0.6f).SetEase(Ease.OutBack)
                            .OnComplete(() => t1.DOMove(slot2.transform.position, 1f).SetEase(Ease.OutBounce)));
                    MoveUp(slot2)
                        .OnComplete(() => t2.DOMove(slot1.transform.position + Vector3.up, 0.6f).SetEase(Ease.OutBack)
                            .OnComplete(() => t2.DOMove(slot1.transform.position, 1f).SetEase(Ease.OutBounce).OnComplete(() => canSelect = true)));

                    // This is a bit janky, but might be doing physics based inventory soon, so not bothering.
                    t1.GetComponent<Rotate>().Enable = false;
                    t2.GetComponent<Rotate>().Enable = false;
                    SFX.Play("item_down", 1, 1, 1.5f);
                    
                    ItemInstance inst1, inst2;
                    if (slot1.GetItemInstance(out inst1) && slot2.GetItemInstance(out inst2)) {
                        slot1.SetItemInstantiated(inst2, obj2);
                        slot2.SetItemInstantiated(inst1, obj1);
                        Inventory.Instance.SwapItem(slot1.index, slot2.index);
                    }

                    currentSelection = null;
                }
            }
        //Else if selected one item and clicked on null slot
        } else if (currentSelection && !slot.GetItem(out item) && canSelect) {
            Slot slot1 = this.currentSelection;
            Slot slot2 = slot;

            GameObject obj1;
            // If the slot we selected has something in it.
            if (slot1.GetPrefabInstance(out obj1)) {//&& currentSelection.GetItemInstance(out inst1) &&
                canSelect = false;
                Transform t1 = obj1.transform;
                
                SFX.Play("item_lift");
                MoveUp(slot1)
                    .OnComplete(() => t1.DOMove(slot2.transform.position + Vector3.up, 0.6f).SetEase(Ease.OutBack)
                        .OnComplete(() => t1.DOMove(slot2.transform.position, 1f).SetEase(Ease.OutBounce).OnComplete(() => canSelect = true)));
                t1.GetComponent<Rotate>().Enable = false;
                SFX.Play("item_down", 1, 1, 1.5f);

                ItemInstance inst1;
                if (slot1.GetItemInstance(out inst1)) {
                    slot1.RemoveDontDestroy();
                    slot2.SetItemInstantiated(inst1, obj1);
                    Inventory.Instance.SwapItem(slot1.index, slot2.index);
                }

                currentSelection = null;
            }

            currentSelection = null;
        }
    }
    
    private void UseWand(Slot slot) {
        Item item;
        ItemInstance instance;
        // Minigame detection.
        if (currentSelection == null && slot.GetItemInstance(out instance) && slot.GetItem(out item)) {
            Debug.Log("got something:" + instance.item);
            currentSelection = slot;
            switch (item.GetType().ToString()) {
                case "Gem":
                    GameManager.Instance.GemTypeTransfer = (item as Gem).gemType;
                    StartCoroutine(LoadAsyncScene("Cutting"));
                    MinigameTransition();
                    break;
                case "Jewel":
                    GameManager.Instance.GemTypeTransfer = (item as Jewel).gemType;
                    //DataTransfer.GemType = (item as Jewel).gemType.ToString();
                    GameManager.Instance.QualityTransfer = instance.Quality;
                    //DataTransfer.currentQuality = instance.Quality;
                    StartCoroutine(LoadAsyncScene("Polishing"));
                    MinigameTransition();
                    break;
                case "Ore":
                    StartCoroutine(LoadAsyncScene("Smelting"));
                    MinigameTransition();
                    break;
                case "Brick":
                    GameManager.Instance.QualityTransfer = instance.Quality;
                    //DataTransfer.currentQuality = instance.Quality;
                    StartCoroutine(LoadAsyncScene("Tracing"));
                    MinigameTransition();
                    break;
                case "ChargedJewel":
                    MoveUp(slot);
                    break;
                case "Shell":
                    MoveUp(slot);
                    break;
            }
        } else if (currentSelection != null && currentSelection != slot) {
            Debug.Log("Wanting to combine...");
            Item item1, item2;
            if (currentSelection.GetItem(out item1) && slot.GetItem(out item2)) {
                // TODO: could you do some xor magic here?
                if (item1.GetType() == typeof(Shell) && item2.GetType() == typeof(ChargedJewel) ||
                    item1.GetType() == typeof(ChargedJewel) && item2.GetType() == typeof(Shell)) {
                    
                    // Check for a free slot.
                    if (ShonkyInventory.Instance.FreeSlot()) {
                        GameObject obj1, obj2;
                        Vector2 midPoint;
                        if (currentSelection.GetPrefabInstance(out obj1) && slot.GetPrefabInstance(out obj2)) {
                            Transform t1 = obj1.transform,
                                      t2 = obj2.transform;
                            
                            midPoint = ((currentSelection.transform.position + Vector3.up) + (slot.transform.position + Vector3.up) / 2f);
                            
                            SFX.Play("item_lift");
                            t2.DOMove(slot.transform.position + Vector3.up, 0.7f).SetEase(Ease.OutBack)
                                .OnComplete(() => t2.DOMove(midPoint, 0.6f).SetEase(Ease.OutBack));
                            t1.DOMove(currentSelection.transform.position + Vector3.up, 0.7f).SetEase(Ease.OutBack)
                                .OnComplete(() => t1.DOMove(midPoint, 0.6f).SetEase(Ease.OutBack))
                                    .OnComplete(() => CombineItems(slot));
                        }
                    } else {
                        GameObject itemObj;
                        if (currentSelection.GetPrefabInstance(out itemObj)) {
                            Transform t = itemObj.transform;
                            t.DOMove(slot.transform.position, 0.7f).SetEase(Ease.OutBack);
                            currentSelection = null;
                        }
                    }
                } else {
                    HideInspector();
                }
            } else {
                HideInspector();
            }
        }
    }

    //Method used to start minigame transition
    private void MinigameTransition() {
        //Move selection up
        GameObject itemObj;
        if (currentSelection.GetPrefabInstance(out itemObj)) {
            Inventory.Instance.RemoveItem(currentSelection.index);
            currentSelection.RemoveDontDestroy();

            Transform t = itemObj.transform;
            // Long but temporary...
            GameManager.Instance.RetriesRemaining = Inventory.Instance.GetMaxRetries(GameManager.Instance.CurrentTown);

            // Move and vibration for some "feedback".
            t.DOMove(t.position + (Vector3.up), 0.7f).SetEase(Ease.OutBack)
                .OnComplete(() => t.DOShakePosition(.5f, .5f, 100, 30f)
                    .OnComplete(() => asyncLoad.allowSceneActivation = true));
        }
    }
    //Method used to combine shonkys
    private void CombineItems(Slot slot) {

        //Find index of items and remove from inventory backend
        string gemType = FindGemType(slot, currentSelection);
        int index1, index2;
        index1 = currentSelection.index;
        index2 = slot.index;
        //Spawn a golem to show item creation 
        //Play SFX
        SFX.Play("golem_created");
        //soundEffects.clip = golumCreated;
        //soundEffects.Play();
        Debug.Log("Created Golem");
        //Get the average quality of the shell and charged gem, assign to new golem.
        Quality.QualityGrade item1 = currentSelection.itemInstance.Quality;
        Quality.QualityGrade item2 = slot.itemInstance.Quality;
        Quality.QualityGrade avg = Quality.CalculateCombinedQuality(item1, item2);
        ItemInstance newGolem = new ItemInstance(gemType, 1, avg, true);
        int index = ShonkyInventory.Instance.InsertItem(newGolem);
        if (index != -1)
        {
            PenSlot pSlot = physicalShonkyInventory.GetSlotAtIndex(index);
            GameObject clone = Instantiate(newGolem.item.physicalRepresentation, pSlot.transform.position,
                pSlot.transform.rotation);
            //PhysicalShonkyInventory.Instance.InsertItemAtSlot(index, newGolem, inst);
            //Move new golem to pen
            pSlot.SetItemInstantiated(newGolem,clone);
            physicalShonkyInventory.MoveToPen(index);
            //Remove backend items
            Inventory.Instance.RemoveItem(index1);
            Inventory.Instance.RemoveItem(index2);
            //Remove front end items
            currentSelection.RemoveItem();
            slot.RemoveItem();
            //reset selection
            currentSelection = null;

            AchievementManager.Get("golem_create_01");
        }
    }
    //Method used to find the gem type selected
    private string FindGemType(Slot slot1, Slot slot2) {
        if (slot1.itemInstance.itemName == "Charged Emerald" || slot2.itemInstance.itemName == "Charged Emerald") {
            return "EmeraldGolem1";
        } else if (slot1.itemInstance.itemName == "Charged Ruby" || slot2.itemInstance.itemName == "Charged Ruby") {
            return "RubyGolem1";
        } else if (slot1.itemInstance.itemName == "Charged Sapphire" || slot2.itemInstance.itemName == "Charged Sapphire") {
            return "SapphireGolem1";
        } else {
            return "RubyGolem1";
        }
    }
    
    private void ResourcePouchOpen(Slot slot) {
        // Hard coded for now.  To do this dynamically, maybe put <names,chances> in a dictionary<string, float>.
        float gemChance = 0.4f,
              oreChance = 1.00f;
        int numberItems = Random.Range(1, 5);

        var drops = new List<ItemInstance>();
        for (int i = 0; i < numberItems; i++) {
            string dropName;
            string gem = DetermineGemToDrop();
            float spin = Random.Range(0, 1f);
            if (spin < gemChance) {
                dropName = DetermineGemToDrop();
            }
            else if (spin < oreChance) {
                dropName = "Ore";
                //This was for testing and it does drop the correct gems
                //dropName = DetermineGemToDrop();
            }
            else {
                dropName = null;
            }

            // Item is not new for now.
            if (dropName != null)
                drops.Add(new ItemInstance(dropName, 1, Quality.QualityGrade.Unset, false));
        }

        Inventory inv = Inventory.Instance;
        slot.RemoveItem();
        inv.RemoveItem(slot.index);

        foreach (ItemInstance drop in drops) {
            int pos = inv.InsertItem(drop);
            // If found a slot to place item.
            if (pos != -1) {
                Slot toSlot = physicalInventory.GetSlotAtIndex(pos);
                GameObject clone = Instantiate(drop.item.physicalRepresentation, slot.transform.position, slot.transform.rotation);

                // Kind of a placeholder animation.
                // TODO: randomize the Vector3.up a little so that the items separate when they go up.
                clone.transform.DOMove(clone.transform.position + Vector3.up, 0.7f).SetEase(Ease.OutBack)
                    .OnComplete(() => clone.transform.DOMove(toSlot.transform.position + Vector3.up, 0.6f).SetEase(Ease.OutBack)
                    .OnComplete(() => clone.transform.DOMove(toSlot.transform.position, 1f).SetEase(Ease.OutBounce)));

                toSlot.SetItemInstantiated(drop, clone);
            }
        }
    }

    private string DetermineGemToDrop() {
        Debug.Log("Current Town is " + Inventory.Instance.GetCurrentTown());
        switch (Inventory.Instance.GetCurrentTown()) {
            case Travel.Towns.WickedGrove:
                return "ruby";
            case Travel.Towns.FlamingPeak:
                return "sapphire";
            case Travel.Towns.GiantsPass:
                return "emerald";
            case Travel.Towns.SkyCity:
                return "ruby";
            default:
                return "ruby";
        }
    }

    // Load a sync in the background.
    private AsyncOperation asyncLoad;
    IEnumerator LoadAsyncScene(string sceneName) {
        asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        // Wait until the asynchronous scene fully loads.
        // This includes actually starting the scene, so the coroutine wont stop until the scene is changed.
        while (!asyncLoad.isDone) {
            Debug.Log("scene not loaded yet.");
            yield return new WaitForSeconds(.1f);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(previousRay);
    }
}
