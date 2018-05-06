using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class InventoryManager : object {
    Item[,] currentInventory;
    //Save Inventory
    public void SaveInventory() {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/ShopInventory.dat");
        //currentInventory = Inventory.ReturnInventory();
        bf.Serialize(file, currentInventory);
        file.Close();
    }

    public Item[,] LoadInventory() {
        if (File.Exists(Application.persistentDataPath + "/ShopInventory.dat")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/ShopInventory.dat", FileMode.Open);
            Item[,] InventoryLoaded = (Item[,])bf.Deserialize(file);
            file.Close();

            currentInventory = InventoryLoaded;
        }
        return currentInventory;
    }
}
/*
[System.Serializable]
public static class Inventory {
    public static Item[,] currentInventory;
    public static int numberOfDrawers = 3;
    public static int numberOfSlots = 15;
    public static int goldAmount;

    //Create initial empty inventory
    public static void GenerateNewInventory() {
        goldAmount = 0;
        Item[,] currentInventory = new Item[numberOfDrawers, numberOfSlots];
        for (int i = 0; i < numberOfDrawers; i++) {
            for (int j = 0; j < numberOfSlots; j++) {
                currentInventory[i, j] = null;
            }
        }
    }

    public static Item[,] ReturnInventory() {
        return currentInventory;
    }

    //Add item to inventory if there is space
    public static bool AddItem(Item itemToAdd) {
        bool itemAdded = false;
        if (EmptySlot()) {
            for (int i = 0; i < numberOfDrawers; i++) {
                for (int j = 0; j < numberOfSlots; j++) {
                    if (currentInventory[i, j] == null && !itemAdded) {
                        currentInventory[i, j] = itemToAdd;
                        itemAdded = true;
                    }
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
        } else {
            return false;
        }
    }
    //Check if there is an empty slot
    public static bool EmptySlot() {
        bool freeSlot = false;
        for (int i = 0; i < numberOfDrawers; i++) {
            for (int j = 0; j < numberOfSlots; j++) {
                if (currentInventory[i, j] == null) {
                    freeSlot = true;
                }
            }
        }
        return freeSlot;
    }

    //Return item at index
    public static Item GetItem(int drawerIndex, int slotIndex) {
        return currentInventory[drawerIndex, slotIndex];
    }
    
}
*/

