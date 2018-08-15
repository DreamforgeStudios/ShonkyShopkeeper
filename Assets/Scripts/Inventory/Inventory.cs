using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
    public List<Travel.Towns> unlockedTowns;
    public Travel.Towns currentTown;

    public void UnlockTown(Travel.Towns town) {
        CheckInitialisation();
        if (unlockedTowns.Contains(town)) {
            Debug.Log("UnlockTown(): " + town.ToString() + " is already unlocked.");
            return;
        }
        
        unlockedTowns.Add(town);
        Save();
    }

    public List<Travel.Towns> GetUnlockedTowns() {
        CheckInitialisation();
        return unlockedTowns;
    }

    private void CheckInitialisation() {
        if (unlockedTowns == null) {
            unlockedTowns = new List<Travel.Towns>();
            unlockedTowns.Add(Travel.Towns.Tutorial);
        }
    }

    public void SetCurrentTown(Travel.Towns town) {
        currentTown = town;
        Save();
    }

    public Travel.Towns GetCurrentTown() {
        return currentTown;
    }

    public int GetMaxRetries(Travel.Towns town) {
        int index = unlockedTowns.IndexOf(town);
        if (index == 0 || index == 1) {
            return 3;
        } else if (index == 2) {
            return 2;
        } else if (index == 3) {
            return 1;
        } else {
            return 2;
        }
    }

    public int GetMaxRetries(Item.GemType gem)
    {
        Travel.Towns townOfGem = TownOfGem(gem);
        int index = unlockedTowns.IndexOf(townOfGem);
        switch (index)
        {
                case 0:
                    return 3;
                case 1:
                    return 3;
                case 2:
                    return 2;
                case 3:
                    return 1;
                default:
                    return 3;
        }
    }

    private Travel.Towns TownOfGem(Item.GemType gem)
    {
        switch (gem)
        {
                case Item.GemType.Ruby:
                    return Travel.Towns.FlamingPeak;
                case Item.GemType.Emerald:
                    return Travel.Towns.WickedGrove;
                case Item.GemType.Sapphire:
                    return Travel.Towns.SkyCity;
                case Item.GemType.Amethyst:
                    return Travel.Towns.GiantsPass;
                default:
                    return Travel.Towns.FlamingPeak;
        }
    }
    
    public void AddGold(int amount) {
        goldCount += amount;
        Save();
    }

    public bool RemoveGold(int amount) {
        if (goldCount - amount >= 0) {
            goldCount -= amount;
            Save();
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
        if (SlotEmpty(index)) {
            // Nothing existed at the specified slot.
            return false;
        }

        inventory[index] = null;//new ItemInstance(CreateInstance(typeof(Empty)) as Item,0, Quality.QualityGrade.Unset, false);
        Save();

        return true;
    }

    // Insert an item, return the index where it was inserted.  -1 if error.
    public int InsertItem(ItemInstance item) {
        for (int i = 0; i < inventory.Length; i++) {
            if (SlotEmpty(i)) {
                //Debug.Log("Inserted at slot " + i);
                inventory[i] = item;
                Save();
                return i;
            }
        }

        // Couldn't find a free slot.
        return -1;
    }

    // Swap two items.
    // BEWARE, can swap null items.
    public bool SwapItem(int index1, int index2) {
        //if (SlotEmpty(index1) || SlotEmpty(index2)) {
            //return false;
        //}

        ItemInstance temp = inventory[index1];
        inventory[index1] = inventory[index2];
        inventory[index2] = temp;
        Save();

        return true;
    }

    public bool SlotEmpty(int index) {
        if (inventory[index] == null || inventory[index].item == null) { //|| inventory[index].item.GetType() == typeof(Empty)) {
            return true;
        }
        return false;
    }

    public bool MarkNew(int index) {
        if (SlotEmpty(index)) {
            return false;
        }

        inventory[index].IsNew = true;
        Save();
        return true;
    }

    public bool UnMarkNew(int index) {
        if (SlotEmpty(index)) {
            return false;
        }

        inventory[index].IsNew = false;
        Save();
        return true;
    }

    // Simply save.
    private void Save() {
        SaveManager.SaveInventory();
    }
}