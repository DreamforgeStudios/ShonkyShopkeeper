using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A "template" for items.
[System.Serializable]
public abstract class Item : ScriptableObject {
    public string itemName;
    public GameObject physicalRepresentation;
    public int stackLimit;
    public bool mergeable;
}

// A class that holds a real instance of a ScriptableObject item.
// Allows us to have copies with mutable data.
[System.Serializable]
public class ItemInstance {
    //public Item item;
    // TODO: shouldn't be able to access this publicly, but it isn't accessed at all at the moment.
    public int quantity = 1;
    public Quality.QualityGrade quality;
    public bool isNew;
    public string itemIdentifier;
    public bool inMine;
    
    public string itemInfo {
        get { return GetItemInfo(); }
    }

    public string itemName {
        get { return GetItemName(); }
    }

    private Item _item = null;
    public Item item {
        get { return GetItem(); }
    }

    public ItemInstance(string itemName, int quantity, Quality.QualityGrade quality, bool isNew) {
        this.itemIdentifier = itemName;
        this.quantity = quantity;
        this.quality = quality;
        this.isNew = isNew;
        
        _item = ((ItemDatabase) Resources.Load("ItemDatabase")).GetActual(itemIdentifier);
    }

    public void AddQuantity(int amount) {
        quantity = Mathf.Min(item.stackLimit, quantity + amount);
    }

    private Item GetItem() {
        if (_item == null) {
            _item = ((ItemDatabase) Resources.Load("ItemDatabase")).GetActual(itemIdentifier);
        }

        return _item;
    }

    private string GetItemName() {
        return item.itemName;
    }

    private string GetItemInfo() {
        string grade = Quality.GradeToString(quality);
        string gradeCol = "#" + ColorUtility.ToHtmlStringRGB(Quality.GradeToColor(quality));
        string str = string.Format("Quality: <color={0}>{1}</color>\n" +
                                   "Quantity: <color=white>{2}</color>\n" +
                                   (isNew ? "<color=#ffc605fc>NEW</color>\n" : ""), gradeCol, grade, quantity);
        return str;
    }
}