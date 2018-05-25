using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Inventory", fileName = "Inventory.asset")]
[System.Serializable]
public class Inventory : ScriptableObject {
    // Saving using unity dev example.
    // https://bitbucket.org/richardfine/scriptableobjectdemo/src/9a60686609a42fea4d00f5d20ffeb7ae9bc56eb9/Assets/ScriptableObject/GameSession/GameSettings.cs?at=default#GameSettings.cs-16,79,83,87,90
    private static Inventory _instance;
    public static Inventory Instance {
        get {
            if (!_instance) {
                Inventory[] tmp = Resources.FindObjectsOfTypeAll<Inventory>();
                if (tmp.Length > 0) {
                    _instance = tmp[0];
                    Debug.Log("Found inventory as: " + _instance);
                } else {
                    Debug.Log("did not find invenotry.");
                    _instance = null;
                }
            }

            return _instance;
        }
    }

    public static void InitializeFromDefault(Inventory inventory) {
		if (_instance) DestroyImmediate(_instance);
		_instance = Instantiate(inventory);
		_instance.hideFlags = HideFlags.HideAndDontSave;
	}

    public static void LoadFromJSON(string path) {
        if (!_instance) DestroyImmediate(_instance);
        _instance = ScriptableObject.CreateInstance<Inventory>();
        JsonUtility.FromJsonOverwrite(System.IO.File.ReadAllText(path), _instance);
        _instance.hideFlags = HideFlags.HideAndDontSave;
    }

    public void SaveToJSON(string path) {
        Debug.LogFormat("Saving inventory to {0}", path);
        System.IO.File.WriteAllText(path, JsonUtility.ToJson(this, true));
    }

    /* Inventory START */
    public int goldCount;
    public ItemInstance[] inventory;
    public ItemInstance empty;
    

    // Not used in vertical slice.
    // public int drawers;

    public void OnEnable() {
        SaveManager save = CreateInstance<SaveManager>();
        save.SaveInventory();
    }
    public void AddGold(int amount) {
        goldCount += amount;
    }

    public bool RemoveGold(int amount) {
        if (goldCount - amount >= 0) {
            goldCount -= amount;
            return true;
        }

        return false;
    }

    // Get an item if it exists.
    public bool GetItem(int index, out ItemInstance item) {
        // inventory[index] doesn't return null, so check item instead.
        if (SlotEmpty(index)) {
            item = null;
            return false;
        }

        item = inventory[index];
        return true;
    }

    // Remove an item at an index if one exists at that index.
    public bool RemoveItem(int index) {
        if (!SlotEmpty(index)) {
            inventory[index] = empty;
            return true;
        }

        // Nothing existed at the specified slot.
        return false;
    }

    // Insert an item, return the index where it was inserted.  -1 if error.
    public int InsertItem(ItemInstance item) {
        for (int i = 0; i < inventory.Length; i++) {
            if (SlotEmpty(i) || PossibleEmpties(i)) {
                inventory[i] = item;
                Debug.Log("Inserted at slot " + i);
                return i;
            }
        }

        // Couldn't find a free slot.
        return -1;
    }

    // Swap two items.
    public bool SwapItem(int index1, int index2) {
        if (inventory[index1] == null || inventory[index2] == null) {
            return false;
        }

        ItemInstance temp = inventory[index1];
        inventory[index1] = inventory[index2];
        inventory[index2] = temp;

        return true;
    }

    // Insert item at specific slot
    public bool InsertItemAtSlot(int currentIndex, int indexToBePlaced) {
        if (inventory[indexToBePlaced] == null && inventory[currentIndex] != null) {
            ItemInstance temp = inventory[currentIndex];
            inventory[currentIndex] = null;
            inventory[indexToBePlaced] = temp;
            return true;
        } else {
            return false;
        }
    }

    public bool SlotEmpty(int index) {
        if (inventory[index] == null || inventory[index].item == null) {
            return true;
        }
        return false;
    }

    public bool PossibleEmpties(int index) {
        if (inventory[index].item.name == "Empty")
            return true;
        else
            return false;
    }
}