using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager {
    public static void LoadOrInitializeInventory(Inventory inventoryTemplate) {
        // Saving and loading.
        if (File.Exists(Path.Combine(Application.persistentDataPath, "inventory.json"))) {
            Debug.Log("Found file inventory.json, loading inventory.");
            Inventory.LoadFromJSON(Path.Combine(Application.persistentDataPath, "inventory.json"));
        } else {
            Debug.Log("Couldn't find inventory.json, loading from template.");
            Inventory.InitializeFromDefault(inventoryTemplate);
        }
    }
    public static void SaveInventory() {
        Inventory.Instance.SaveToJSON(Path.Combine(Application.persistentDataPath, "inventory.json"));
    }

    public static void LoadOrInitializeShonkyInventory(ShonkyInventory shonkyInventoryTemplate) {
        // Saving and loading.
        if (File.Exists(Path.Combine(Application.persistentDataPath, "shonkyinventory.json"))) {
            Debug.Log("Found file shonkyinventory.json, loading shonky inventory.");
            ShonkyInventory.LoadFromJSON(Path.Combine(Application.persistentDataPath, "shonkyinventory.json"));
        }
        else {
            Debug.Log("Couldn't find shonkyinventory.json, loading from template.");
            ShonkyInventory.InitializeFromDefault(shonkyInventoryTemplate);
        }
    }

    public static void SaveShonkyInventory() {
        ShonkyInventory.Instance.SaveToJSON(Path.Combine(Application.persistentDataPath, "shonkyinventory.json"));
    }

    public static void LoadOrInitializeMineInventory(Mine mineTemplate) {
        // Saving and loading.
        if (File.Exists(Path.Combine(Application.persistentDataPath, "mineinventory.json"))) {
            Debug.Log("Found file mineinventory.json, loading mine inventory.");
            Mine.LoadFromJSON(Path.Combine(Application.persistentDataPath, "mineinventory.json"));
        }
        else {
            Debug.Log("Couldn't find mineinventory.json, loading from template.");
            Mine.InitializeFromDefault(mineTemplate);
        }
    }

    public static void SaveMineInventory() {
        Mine.Instance.SaveToJSON(Path.Combine(Application.persistentDataPath, "mineinventory.json"));
    }


    // Load from default.
    public static void LoadFromTemplate(Inventory template) {
        Inventory.InitializeFromDefault(template);
    }


    // Load from default.
    public static void LoadFromMineTemplate(Mine template) {
        Mine.InitializeFromDefault(template);
    }

    // Load from default.
    public static void LoadFromShonkyTemplate(ShonkyInventory template) {
        ShonkyInventory.InitializeFromDefault(template);
    }
}
