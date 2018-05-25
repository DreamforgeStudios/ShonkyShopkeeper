using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SaveManager : ScriptableObject {
	public void LoadOrInitializeInventory(Inventory inventoryTemplate) {
		// Saving and loading.
		if (System.IO.File.Exists(System.IO.Path.Combine(Application.persistentDataPath, "inventory.json"))) {
			Debug.Log("Loading inventory.");
			Inventory.LoadFromJSON(System.IO.Path.Combine(Application.persistentDataPath, "inventory.json"));
		} else {
			Inventory.InitializeFromDefault(inventoryTemplate);
		}
	}
    public void SaveInventory() {
        Inventory.Instance.SaveToJSON(System.IO.Path.Combine(Application.persistentDataPath, "inventory.json"));
    }

    public void LoadOrInitializeShonkyInventory(ShonkyInventory shonkyInventoryTemplate) {
        // Saving and loading.
        if (System.IO.File.Exists(System.IO.Path.Combine(Application.persistentDataPath, "shonkyinventory.json"))) {
            Debug.Log("Loading shonky inventory.");
            ShonkyInventory.LoadFromJSON(System.IO.Path.Combine(Application.persistentDataPath, "shonkyinventory.json"));
        }
        else {
            ShonkyInventory.InitializeFromDefault(shonkyInventoryTemplate);
        }
    }

    public void SaveShonkyInventory() {
        ShonkyInventory.Instance.SaveToJSON(System.IO.Path.Combine(Application.persistentDataPath, "shonkyinventory.json"));
    }


}
