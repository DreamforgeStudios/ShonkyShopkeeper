using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SaveManager : ScriptableObject {
	public static void LoadOrInitializeInventory(Inventory inventoryTemplate) {
		// Saving and loading.
		if (System.IO.File.Exists(System.IO.Path.Combine(Application.persistentDataPath, "inventory.json"))) {
			Debug.Log("Found file inventory.json, loading inventory.");
			Inventory.LoadFromJSON(System.IO.Path.Combine(Application.persistentDataPath, "inventory.json"));
		} else {
            Debug.Log("Couldn't find inventory.json, loading from template.");
			Inventory.InitializeFromDefault(inventoryTemplate);
		}
	}
    public static void SaveInventory() {
        Inventory.Instance.SaveToJSON(System.IO.Path.Combine(Application.persistentDataPath, "inventory.json"));
    }

    public static void LoadOrInitializeShonkyInventory(ShonkyInventory shonkyInventoryTemplate) {
        // Saving and loading.
        if (System.IO.File.Exists(System.IO.Path.Combine(Application.persistentDataPath, "shonkyinventory.json"))) {
            Debug.Log("Found file shonkyinventory.json, loading shonky inventory.");
            ShonkyInventory.LoadFromJSON(System.IO.Path.Combine(Application.persistentDataPath, "shonkyinventory.json"));
        }
        else {
            Debug.Log("Couldn't find shonkyinventory.json, loading from template.");
            ShonkyInventory.InitializeFromDefault(shonkyInventoryTemplate);
        }
    }

    public static void SaveShonkyInventory() {
        ShonkyInventory.Instance.SaveToJSON(System.IO.Path.Combine(Application.persistentDataPath, "shonkyinventory.json"));
    }


	// Load from default.
	public static void LoadFromTemplate(Inventory template) {
		Inventory.InitializeFromDefault(template);
	}

	// Load from default.
	public static void LoadFromShonkyTemplate(ShonkyInventory template) {
		ShonkyInventory.InitializeFromDefault(template);
	}
}
