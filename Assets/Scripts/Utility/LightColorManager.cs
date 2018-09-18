using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TownColorDictionary : SerializableDictionary<Travel.Towns, Color> {}

public class LightColorManager : MonoBehaviour {
	public TownColorDictionary TownColorDict;
	public Light lightObj;
	
	// Use this for initialization
	void Start () {
		Travel.Towns town = Inventory.Instance.GetCurrentTown();
		Color col;
		if (TownColorDict.TryGetValue(town, out col)) {
			lightObj.color = col;
		} else {
			Debug.LogWarning("Did not find specified town, please add it to the dictionary.");
		}
	}
}
