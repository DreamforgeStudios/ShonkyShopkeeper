using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening; // Tweening / nice lerping.
//using System;
using UnityEngine.SceneManagement; //To access Minigames

// The toolbox class is how all user inventory interactions should be done.
public class Toolbox : MonoBehaviour {
    public enum Tool {
        Inspector,
        Foreceps,
        Wand,
        None
    }

    public PhysicalInventory physicalInventory;
    public ItemDatabase database;

    //Capture the original positions and gameObjects of each tool
    private Vector3 forcepPos;
    private Vector3 inspectPos;
    private Vector3 wandPos;
    public GameObject forcep;
    public GameObject inspectorObj;
    public GameObject wandObj;

    // A layer mask so that we only hit slots.
    public LayerMask layerMask;

    // Variables for the inspector.
    public GameObject inspectionPanel;
    public TextMeshProUGUI textHeading;
    public TextMeshProUGUI textInfo;

    // Variables for minigames - Not necessary right now
    //public TextMeshProUGUI beginMiniGame

    // Helpers.
    //private Inventory inventoryhelper;
    private Tool currentTool;

    //Helpers to capture Items, transfoms, slot indexes, gameobjects, movement stages and instances when using forceps
    private Slot currentSelection = null;
    //private Slot secondSelection = null;
    private bool canSelect = true;
    //private Transform firstTransform;
    //private Transform secondTransform;
    //private Vector3 firstSelectionOriginal;
    //private Vector3 secondSelectionOriginal;
    //private Vector3 firstSelectHigh;
    //private Vector3 secondSelectHigh;
    //private int slot1SelectionIndex;
    //private int slot2SelectionIndex;
    //private ItemInstance instanceItem1;
    //private ItemInstance instanceItem2;
    //private ItemInstance tempHolder;
    //private bool inPosition1 = false;
    //private bool inPosition2 = false;
    //private bool inFinalPosition = false;

    //Two testing instances to showcase resource Pouch. Delete in final implementation once
    //we can create items through code correctly.
    //public ItemInstance ore;
    //public ItemInstance ruby;

    // Debug.
    private Ray previousRay;

    // Use this for initialization
    void Start() {
        // TODO: Change this to the default tool.
        currentTool = Tool.None;
        //inventoryhelper = Inventory.Instance;
        forcepPos = GameObject.FindGameObjectWithTag("forcep").transform.position;
        wandPos = GameObject.FindGameObjectWithTag("wand").transform.position;
        inspectPos = GameObject.FindGameObjectWithTag("inspector").transform.position;
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

    public void SwitchTool(Tool tool) {
        if (currentTool != tool) {
            switch (currentTool) {
                case Tool.Inspector:
                    Transform a = inspectorObj.transform;
                    a.DOMove(inspectPos, 0.7f).SetEase(Ease.OutBack);
                    break;
                case Tool.Foreceps:
                    Transform b = forcep.transform;
                    b.DOMove(forcepPos, 0.7f).SetEase(Ease.OutBack);
                    break;
                case Tool.Wand:
                    Transform c = wandObj.transform;
                    c.DOMove(wandPos, 0.7f).SetEase(Ease.OutBack);
                    break;
            }
            currentTool = tool;
            HideInspector();
        }
    }
    //Updated to raycast if the player selects a tool and switch to that tool if they did
    private void ProcessMouse() {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            previousRay = ray;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
                Debug.Log("Hit: " + hit.transform.name);
                if (hit.transform.tag == "forcep" || hit.transform.tag == "wand" || hit.transform.tag == "inspector" && canSelect) {
                    switch (hit.transform.tag) {
                        case "forcep":
                            SwitchTool(Tool.Foreceps);
                            Transform a = hit.transform;
                            a.DOMove(a.position + (Vector3.up), 0.7f).SetEase(Ease.OutBack);
                            break;
                        case "wand":
                            SwitchTool(Tool.Wand);
                            Transform b = hit.transform;
                            b.DOMove(b.position + (Vector3.up), 0.7f).SetEase(Ease.OutBack);
                            break;
                        case "inspector":
                            SwitchTool(Tool.Inspector);
                            Transform c = hit.transform;
                            c.DOMove(c.position + (Vector3.up), 0.7f).SetEase(Ease.OutBack);
                            break;
                    }
                }
                else {
                    PickAndUseTool(hit.transform.GetComponent<Slot>());
                }
            }
            else {
                if (currentTool == Tool.Inspector) {
                    HideInspector();
                }
            }
        }
    }

    private void ProcessTouch() {
        if (Input.touchCount == 0) {
            return;
        }

        foreach (Touch touch in Input.touches) {
            if (touch.phase == TouchPhase.Began) {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                previousRay = ray;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
                    Debug.Log("Hit: " + hit.transform.name);
                    if (hit.transform.tag == "forcep" || hit.transform.tag == "wand" || hit.transform.tag == "inspector" && canSelect) {
                        switch (hit.transform.tag) {
                            case "forcep":
                                SwitchTool(Tool.Foreceps);
                                Transform a = hit.transform;
                                a.DOMove(a.position + (Vector3.up), 0.7f).SetEase(Ease.OutBack);
                                break;
                            case "wand":
                                SwitchTool(Tool.Wand);
                                Transform b = hit.transform;
                                b.DOMove(b.position + (Vector3.up), 0.7f).SetEase(Ease.OutBack);
                                break;
                            case "inspector":
                                SwitchTool(Tool.Inspector);
                                Transform c = hit.transform;
                                c.DOMove(c.position + (Vector3.up), 0.7f).SetEase(Ease.OutBack);
                                break;
                        }
                    }
                    else {
                        PickAndUseTool(hit.transform.GetComponent<Slot>());
                    }
                }
                else {
                    // If there was no hit and we're using the inspector, then we should hide the inspector.
                    if (currentTool == Tool.Inspector) {
                        HideInspector();
                    }
                }
            }
        }
    }

    // Use the right tool on the slot.
    private void PickAndUseTool(Slot slot) {
        switch (currentTool) {
            case Tool.Inspector:
                UseInspector(slot);
                break;
            case Tool.Foreceps:
                UseForceps(slot);
                break;
            case Tool.Wand:
                UseWand(slot);
                break;
        }
    }

    // Inspect an item.
    private void UseInspector(Slot slot) {
        // Can't select 2 items at once.
        if (currentSelection) {
            // Maybe this will cause flickering, might be better to just hide the object.
            HideInspector();
            currentSelection = null;
        }

        // To avoid null errors, always use the x.Get() methods, they check for you.
        Item item;
        ItemInstance instance;
        if (slot.GetItemInstance(out instance)) {
            //Debug.Log(instance.item.GetItemName());
            if (instance.item.GetItemName() == "ResourcePouch") {
                ResourcePouchOpen(slot);
            }
            else {
                if (slot.GetItem(out item)) {
                    this.currentSelection = slot;
                    inspectionPanel.SetActive(true);

                    textHeading.text = item.GetItemName();
                    textInfo.text = item.GetItemInfo();

                    // Animate using tween library -> see https://easings.net/ for some animaions to use.
                    GameObject itemObj;
                    if (slot.GetPrefabInstance(out itemObj)) {
                        Transform t = itemObj.transform;
                        t.DOMove(t.position + (Vector3.up), 0.7f).SetEase(Ease.OutBack);
                    }
                }
            }
        }
    }

    // Hide the inspector and the current item (if one is selected).
    private void HideInspector() {
        inspectionPanel.SetActive(false);
        GameObject gameObj;
        if (currentSelection && currentSelection.GetPrefabInstance(out gameObj)) {
            gameObj.transform.DOMove(currentSelection.transform.position, 1f).SetEase(Ease.OutBounce);
            currentSelection = null;
        }
    }

    //Move an Item to a new slot
    private void UseForceps(Slot slot) {
        Item item;
        if (slot.GetItem(out item) && canSelect) {
            //If first selection
            if (currentSelection == null) {
                this.currentSelection = slot;

                GameObject itemObj;
                if (slot.GetPrefabInstance(out itemObj)) {
                    Transform t = itemObj.transform;
                    t.DOMove(slot.transform.position + (Vector3.up), 0.7f).SetEase(Ease.OutBack);
                }
            // Second selection.
            } else {
                // If the same item is selected, put it back.
                if (currentSelection == slot) {
                    Debug.Log("Same slot.");
                    GameObject obj;
                    if (currentSelection.GetPrefabInstance(out obj)) {
                        obj.transform.DOMove(currentSelection.transform.position, 1f).SetEase(Ease.OutBounce);
                    }

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
                    Debug.Log("here");

                    Transform t1 = obj1.transform,
                              t2 = obj2.transform;

                    t1.DOMove(slot1.transform.position + Vector3.up, 0.7f).SetEase(Ease.OutBack)
                        .OnComplete(() => t1.DOMove(slot2.transform.position + Vector3.up, 0.6f).SetEase(Ease.OutBack)
                        .OnComplete(() => t1.DOMove(slot2.transform.position, 1f).SetEase(Ease.OutBounce).OnComplete(() => canSelect = true)));

                    t2.DOMove(slot2.transform.position + Vector3.up, 0.7f).SetEase(Ease.OutBack)
                        .OnComplete(() => t2.DOMove(slot1.transform.position + Vector3.up, 0.6f).SetEase(Ease.OutBack)
                        .OnComplete(() => t2.DOMove(slot1.transform.position, 1f).SetEase(Ease.OutBounce).OnComplete(() => canSelect = true)));

                    ItemInstance inst1, inst2;
                    if (slot1.GetItemInstance(out inst1) && slot2.GetItemInstance(out inst2)) {
                        slot1.SetItemInstantiated(inst2, obj2);
                        slot2.SetItemInstantiated(inst1, obj1);
                        Inventory.Instance.SwapItem(slot1.index, slot2.index);
                    }

                    currentSelection = null;
                }
            }
        } //Else if selected one item and click empty slot
        else if (canSelect && currentSelection != null && !slot.GetItem(out item)) {
            ItemInstance inst1;
            GameObject obj;
            if (currentSelection.GetPrefabInstance(out obj)) {
                Transform t1 = obj.transform;
                t1.DOMove(currentSelection.transform.position + Vector3.up, 0.7f).SetEase(Ease.OutBack)
                       .OnComplete(() => t1.DOMove(slot.transform.position + Vector3.up, 0.6f).SetEase(Ease.OutBack)
                       .OnComplete(() => t1.DOMove(slot.transform.position, 1f).SetEase(Ease.OutBounce).OnComplete(() => canSelect = true)));
            }
            
            if (currentSelection.GetItemInstance(out inst1)) {
                Inventory.Instance.InsertItemAtSlot(currentSelection.index, slot.index);
                currentSelection.RemoveDontDestroy();
                slot.SetItemInstantiated(inst1, obj);
                Debug.Log(obj.name);
            }
            currentSelection = null;
            
        }
    }

    private void UseWand(Slot slot) {
        // Can't select 2 items at once.
        if (currentSelection) {
            // Maybe this will cause flickering, might be better to just hide the object.
            HideInspector();
            currentSelection = null;
        }

        Item item;
        ItemInstance instance;
        if (slot.GetItem(out item)) {
            this.currentSelection = slot;

            if(slot.GetItemInstance(out instance)) {
                Debug.Log(instance.item.GetType());

                switch (instance.item.GetType().ToString()) {
                    case "Gem":
                        SceneManager.LoadScene("Cutting");
                        break;
                    case "Ore":
                        SceneManager.LoadScene("Smelting");
                        break;
                    case "Jewel":
                        SceneManager.LoadScene("Polishing");
                        break;
                    case "Brick":
                        SceneManager.LoadScene("Tracing");
                        break;
                }
            }
        }
    }

    private void ResourcePouchOpen(Slot slot) {
        // Hard coded for now.  To do this dynamically, maybe put <names,chances> in a dictionary<string, float>.
        float rubyChance = 0.4f,
              oreChance = 1.00f;
        int numberItems = Random.Range(1, 5);

        var drops = new List<ItemInstance>();
        for (int i = 0; i < numberItems; i++) {
            Item drop;
            float spin = Random.Range(0, 1f);
            if (spin < rubyChance) {
                drop = database.GetActual("Ruby");
                Debug.Log("Added a ruby.");
            } else if (spin < oreChance) {
                drop = database.GetActual("Ore");
                Debug.Log("Added some ore.");
            // To satisfy compiler.
            } else {
                drop = null;
            }

            drops.Add(new ItemInstance(drop, 1, Quality.QualityGrade.Junk));
        }

        Inventory inv = Inventory.Instance;
        slot.RemoveItem();
        inv.RemoveItem(slot.index);

        foreach (ItemInstance drop in drops) {
            int pos = inv.InsertItem(drop);
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

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(previousRay);
    }
}
