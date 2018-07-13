using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/mineInventory", fileName = "mineInventory.asset")]
[System.Serializable]
public class Mine : ScriptableObject {

    private static Mine _instance;
    public static Mine Instance {
        get {
            if (!_instance) {
                Mine[] tmp = Resources.FindObjectsOfTypeAll<Mine>();
                if (tmp.Length > 0) {
                    _instance = tmp[0];
                    Debug.Log("Found mine as: " + _instance);
                }
                else {
                    Debug.Log("did not find mine.");
                    _instance = null;
                }
            }

            return _instance;
        }
    }

    public static void InitializeFromDefault(Mine mineInventory) {
        if (_instance) DestroyImmediate(_instance);
        _instance = Instantiate(mineInventory);
        _instance.hideFlags = HideFlags.HideAndDontSave;
    }

    public static void LoadFromJSON(string path) {
        if (!_instance) DestroyImmediate(_instance);
        _instance = ScriptableObject.CreateInstance<Mine>();
        JsonUtility.FromJsonOverwrite(System.IO.File.ReadAllText(path), _instance);
        _instance.hideFlags = HideFlags.HideAndDontSave;
    }

    public void SaveToJSON(string path) {
        Debug.LogFormat("Saving mine to {0}", path);
        System.IO.File.WriteAllText(path, JsonUtility.ToJson(this, true));
    }

    /*
     * Mine START
     */
    //Time till the golem is complete mining
    public int mineTimeSeconds = 10;
    public ItemInstance[] mineInventory;
    private Dictionary<DateTime, GameObject> returningGolems = new Dictionary<DateTime, GameObject>();
    private Dictionary<DateTime, GameObject> golemTable = new Dictionary<DateTime, GameObject>();
    private List<GameObject> instantReturn = new List<GameObject>();
	
    public void AddGolemAndTime(DateTime currentTime, GameObject golem) {
        Debug.Log(currentTime + " and does contain already " + golemTable.ContainsKey(currentTime));
        golemTable.Add(currentTime, golem);
        Save();
    }

    public void AddGolemReadyToCollect(GameObject golem) {
        instantReturn.Add(golem);
    }
    public List<GameObject> ReturnReadyGolems() {
        //Find Golems to be returned
        returningGolems = new Dictionary<DateTime, GameObject>();
        foreach (KeyValuePair<DateTime, GameObject> golem in golemTable) {
            TimeSpan elapsedTime;
            elapsedTime = DateTime.Now - golem.Key;
            if (elapsedTime.Seconds > mineTimeSeconds) {
                returningGolems.Add(golem.Key,golem.Value);   
            }
        }
        //Remove found golems
        foreach(KeyValuePair<DateTime, GameObject> golem in returningGolems) {
            if (golemTable.ContainsKey(golem.Key)) {
                golemTable.Remove(golem.Key);
            }
        }
        //Return only golem objects
        List<GameObject> golems = new List<GameObject>();
        foreach(KeyValuePair<DateTime, GameObject> golem in returningGolems) {
            golems.Add(golem.Value);
        }

        //Add Instant return golems if any
        foreach(GameObject golem in instantReturn) {
            golems.Add(golem);
        }
        instantReturn = new List<GameObject>();
        Save();
        return golems;
    }

    public bool ReadyToCollect() {
        foreach (KeyValuePair<DateTime, GameObject> golem in golemTable) {
            TimeSpan elapsedTime;
            elapsedTime = DateTime.Now - golem.Key;
            if (elapsedTime.Seconds > mineTimeSeconds)
                return true;
        }
        if (instantReturn.Count > 0) {
            return true;
        }
        return false;
    }

    // Simply save..
    public void Save() {
        SaveManager.SaveMineInventory();
    }
}
