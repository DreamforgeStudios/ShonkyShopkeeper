using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory : MonoBehaviour {
    //public List<string> currentInventory = new List<string>();
    public List<Ore> inv = new List<Ore>();
    //public Ore testItem;
    //private Item itemToAdd;
    public static GameObject objectRepresentation;
    public static int numberOfDrawers = 3;
    public static int numberOfSlots = 15;
    public static int goldAmount;
    
    //Create initial empty inventory
    public void GenerateNewInventory() {
        goldAmount = 0;
        /*
        string[,] currentInventory = new string[numberOfDrawers, numberOfSlots];
        for (int i = 0; i < numberOfDrawers; i++) {
            for (int j = 0; j < numberOfSlots; j++) {
                currentInventory[i, j] = testItem;
                Debug.Log(i + j + " is " + currentInventory[i, j]);
            }
        } */
        for (int i = 0; i < numberOfSlots; i++) {
            inv.Add(null);
        }
    }

    public List<Ore> ReturnInventory() {
        return inv;
    }

    //Add item to inventory if there is space
    public bool AddItem(Ore itemToAdd) {
        bool itemAdded = false;
        if (EmptySlot()) {
            /*
            for (int i = 0; i < numberOfDrawers; i++) {
                for (int j = 0; j < numberOfSlots; j++) {
                    if (currentInventory[i, j] == null && !itemAdded) {
                        currentInventory[i, j] = itemToAdd;
                        itemAdded = true;
                    }
                }
            }*/
            for (int i = 0; i < numberOfSlots; i++) {
                if (inv[i] == null && !itemAdded) {
                    inv[i] = itemToAdd;
                    itemAdded = true;
                    Debug.Log("added item at " + i);
                }
            }
        }
        return itemAdded;
    }

    //Add Gold
    public static void AddGold(int amountToAdd) {
        goldAmount = goldAmount + amountToAdd;
    }

    //Remove Gold
    public static bool RemoveGold(int amountToRemove) {
        if ((goldAmount - amountToRemove) >= 0) {
            goldAmount = goldAmount - amountToRemove;
            return true;
        }
        else {
            return false;
        }
    }
    
    //Check if there is an empty slot
    public bool EmptySlot() {
        bool freeSlot = false;
        /*
        for (int i = 0; i < numberOfDrawers; i++) {
            for (int j = 0; j < numberOfSlots; j++) {
                if (currentInventory[i, j] == "null") {
                    freeSlot = true;
                }
            }
        }
        */
        for (int i = 0; i < numberOfSlots; i++) {
            if (inv[i] == null) {
                freeSlot = true;
            }
        }
        return freeSlot;
    }

    /*
    //Return item at index
    public Ore GetItem(List<Ore> inventory, int slotIndex) {
        //Debug.Log("Item in " + drawerIndex + " drawer and slot " + slotIndex + " is " + currentInventory[drawerIndex, slotIndex]);
        //return currentInventory[drawerIndex, slotIndex];
        return inventory[slotIndex];
    }
    */

    private void Start() {
        GenerateNewInventory();
        Debug.Log("Generated Inventory");
        inv = ReturnInventory();
        //this.GetComponent<InventoryPopulator>().PopulateWithJunk(this);
        Debug.Log("adding item");
        Ore itemAdd = new Ore(3);
        //ScriptableObject ore = ScriptableObject.CreateInstance("Ore");
        AddItem(itemAdd);
        //Debug.Log("Item at 0,0 is " + inv[0].quantity);
    }

}

