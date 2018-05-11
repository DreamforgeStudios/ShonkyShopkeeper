using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A "template" for items.
[System.Serializable]
public abstract class Item : ScriptableObject {
    public enum ItemType {
        Ore, Brick, Shell, Gem, Jewel, ChargedJewel, Shonky, ResourcePouch
    }

    public enum RuneType {
        Rune1, Rune2, Rune3
    }

    public enum GemType {
        Ruby, Diamond, Sapphire, Emerald
    }

    public string itemName;
    public GameObject physicalRepresentation;
    public int stackLimit;
    public bool mergeable;

    // Item name stays the same.
    public string GetItemName() {
        return itemName;
    }

    // Item info will be displayed differently for each item type.
    public abstract string GetItemInfo();
}

// A class that holds a real instance of a ScriptableObject item.
// Allows us to have copies with mutable data.
[System.Serializable]
public class ItemInstance {
    public Item item;
    public int quantity = 1;
    public Quality.QualityGrade quality;

    public void AddQuantity(int amount) {
        quantity = Mathf.Min(item.stackLimit, quantity + amount);
    }
}