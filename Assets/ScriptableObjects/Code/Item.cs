using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A "template" for items.
[System.Serializable]
public abstract class Item : ScriptableObject {
    public enum GemType {
        Ruby, Amethyst, Sapphire, Emerald
    }
    
    public string itemName;
    public GameObject physicalRepresentation;
    public int stackLimit;
    public bool mergeable;
}

// A class that holds a real instance of a ScriptableObject item.
// Allows us to have copies with mutable data.
[System.Serializable]
public class ItemInstance {
    public int Quantity = 1;
    public Quality.QualityGrade Quality;
    public bool IsNew;
    public string ItemIdentifier;
    public bool InMine;
    
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
        this.ItemIdentifier = itemName;
        this.Quantity = quantity;
        this.Quality = quality;
        this.IsNew = isNew;
        
        _item = ((ItemDatabase) Resources.Load("ItemDatabase")).GetActual(ItemIdentifier);
    }

    public void AddQuantity(int amount) {
        Quantity = Mathf.Min(item.stackLimit, Quantity + amount);
    }

    private Item GetItem() {
        if (_item == null) {
            _item = ((ItemDatabase) Resources.Load("ItemDatabase")).GetActual(ItemIdentifier);
        }

        return _item;
    }

    private string GetItemName() {
        return item.itemName;
    }

    private string GetItemInfo() {
        string grade = global::Quality.GradeToString(Quality);
        string gradeCol = "#" + ColorUtility.ToHtmlStringRGB(global::Quality.GradeToColor(Quality));
        string str = string.Format("Quality: <color={0}>{1}</color>\n" +
                                   "Quantity: <color=white>{2}</color>\n" +
                                   (IsNew ? "<color=#ffc605fc>NEW</color>\n" : ""), gradeCol, grade, Quantity);
        return str;
    }
}