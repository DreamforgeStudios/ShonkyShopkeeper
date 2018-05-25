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

	// Load from default.
	public void LoadFromTemplate(Inventory template) {
		Inventory.InitializeFromDefault(template);
	}
}
