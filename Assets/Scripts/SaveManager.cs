using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour {
	public Inventory inventoryTemplate;

	// Use this for initialization
	void Start () {
		// Saving and loading.
		if (System.IO.File.Exists(System.IO.Path.Combine(Application.persistentDataPath, "inventory.json"))) {
			Debug.Log("Loading inventory.");
			Inventory.LoadFromJSON(System.IO.Path.Combine(Application.persistentDataPath, "inventory.json"));
		} else {
			Inventory.InitializeFromDefault(inventoryTemplate);
		}

		Inventory.Instance.SaveToJSON(System.IO.Path.Combine(Application.persistentDataPath, "inventory.json"));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
