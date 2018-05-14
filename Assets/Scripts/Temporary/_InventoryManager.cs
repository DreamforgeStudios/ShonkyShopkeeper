using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class _InventoryManager : MonoBehaviour {
    private Item[,] currentInventory;
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

    
*/

