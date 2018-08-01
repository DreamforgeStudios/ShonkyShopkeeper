using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInitialization : MonoBehaviour {

	//Default inventories
	public Inventory inventory;
	public ShonkyInventory shonkyInventory;
	public Mine mineInventory;
	
	//To tell if this is the first time starting
	int firstStart = 0;
	
	// Use this for initialization
	void Start () {
		//PlayerPrefs.DeleteKey("FirstStart");
		InitializeAllInventories();
		firstStart = PlayerPrefs.GetInt("FirstStart");
	}

	private void InitializeAllInventories()
	{
		Debug.Log("Loading Initial Inventories");
		SaveManager.LoadOrInitializeInventory(inventory);
		SaveManager.LoadOrInitializeShonkyInventory(shonkyInventory);
		SaveManager.LoadOrInitializeMineInventory(mineInventory);
		SaveManager.SaveInventory();
		SaveManager.SaveShonkyInventory();
		SaveManager.SaveMineInventory();
		Debug.Log("Saved Initial Inventories");
		if (firstStart != 0)
		{
			SceneManager.LoadScene("Shop");
		}
	}
}
