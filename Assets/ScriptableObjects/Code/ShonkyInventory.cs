﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/ShonkyInventory", fileName = "ShonkyInventory.asset")]
[System.Serializable]
public class ShonkyInventory : ScriptableObject {
    // Saving using unity dev example.
    // https://bitbucket.org/richardfine/scriptableobjectdemo/src/9a60686609a42fea4d00f5d20ffeb7ae9bc56eb9/Assets/ScriptableObject/GameSession/GameSettings.cs?at=default#GameSettings.cs-16,79,83,87,90
    private static ShonkyInventory _instance;
    public static ShonkyInventory Instance {
        get {
            if (!_instance) {
                ShonkyInventory[] tmp = Resources.FindObjectsOfTypeAll<ShonkyInventory>();
                if (tmp.Length > 0) {
                    _instance = tmp[0];
                    Debug.Log("Found shonky inventory as: " + _instance);
                }
                else {
                    Debug.Log("did not find shonky inventory.");
                    _instance = null;
                }
            }

            return _instance;
        }
    }

    public static void InitializeFromDefault(ShonkyInventory shonkyInventory) {
        if (_instance) DestroyImmediate(_instance);
        _instance = Instantiate(shonkyInventory);
        _instance.hideFlags = HideFlags.HideAndDontSave;
    }

    public static void LoadFromJSON(string path) {
        if (!_instance) DestroyImmediate(_instance);
        _instance = ScriptableObject.CreateInstance<ShonkyInventory>();
        JsonUtility.FromJsonOverwrite(System.IO.File.ReadAllText(path), _instance);
        _instance.hideFlags = HideFlags.HideAndDontSave;
    }

    public void SaveToJSON(string path) {
        Debug.LogFormat("Saving shonky inventory to {0}", path);
        System.IO.File.WriteAllText(path, JsonUtility.ToJson(this, true));
    }

    /* Shonky Inventory START */
    public ItemInstance[] shonkyInventory;

    //public ItemInstance empty;


    // Not used in vertical slice.
    // public int drawers;

/*
    public void OnEnable() {
        SaveManager save = CreateInstance<SaveManager>();
        save.SaveShonkyInventory();
    }
    */

    //Check if there is a free slot
    public bool FreeSlot() {
        for (int i = 0; i < shonkyInventory.Length; i++) {
            if (SlotEmpty(i)) {
                return true;
            }
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
        
        item = shonkyInventory[index];
        /*
        UnlockTrueGolem(item);
        */
        return true;
    }

    // Remove an item at an index if one exists at that index.
    public bool RemoveItem(int index) {
        if (!SlotEmpty(index)) {
            shonkyInventory[index] = null;
            Save();
            return true;
        }

        // Nothing existed at the specified slot.
        return false;
    }

    // Insert an item, return the index where it was inserted.  -1 if error.
    public int InsertItem(ItemInstance item) {
        for (int i = 0; i < shonkyInventory.Length; i++) {
            if (SlotEmpty(i)) { //|| PossibleEmpties(i)) {
                //Debug.Log("Inserted at slot " + i);
                shonkyInventory[i] = item;
                if (CheckIfTrueGolem(item))
                {
                    UnlockTrueGolem(item);
                }
                Save();
                return i;
            }
        }
        // Couldn't find a free slot.
        return -1;
    }

    public bool CheckIfTrueGolem(ItemInstance item)
    {
        if (!GameManager.Instance.InTutorial)
        {
            if (item.item.GetType() == typeof(Shonky))
            {
                Quality.QualityGrade golemQuality = item.Quality;
                if (golemQuality == Quality.QualityGrade.Mystic)
                {
                    string gemType = (item.item as Shonky).type.ToString();
                    if (TrueGolems.PotentialUnlockTrueGolem(TrueGolems.GemStringToGolem(gemType)))
                    {
                        return true;
                    }
                }
                        
            }
        }
        return false;
    }

    private bool UnlockTrueGolem(ItemInstance item)
    {
        string gemType = (item.item as Shonky).type.ToString();
        return Inventory.Instance.UnlockTrueGolem(TrueGolems.GemStringToGolem(gemType));
    }

    public bool SlotEmpty(int index) {
        if (shonkyInventory[index] == null || shonkyInventory[index].item == null) {
            return true;
        }

        return false;
    }

    //Need a way to determine if a shonky is in the mine and if so, it cannot be sold currently
    public List<int> PopulatedShonkySlots() {
        List<int> indexes = new List<int>();
        for (int i = 0; i < shonkyInventory.Length; i++) {
            if (!SlotEmpty(i)){ //&& !InMineCurrently(i)) {
                indexes.Add(i);
            }
        }
        return indexes;
    }

    // Simply save..
    private void Save() {
        SaveManager.SaveShonkyInventory();
    }
}
