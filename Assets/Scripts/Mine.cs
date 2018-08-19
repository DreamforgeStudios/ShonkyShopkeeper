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
    private Dictionary<DateTime, int> returningGolems = new Dictionary<DateTime, int>();
    private Dictionary<DateTime, int> golemTable = new Dictionary<DateTime, int>();
    private List<int> instantReturn = new List<int>();
	
    public void AddGolemAndTime(DateTime currentTime, int penSlot) {
        //Debug.Log(currentTime + " and does contain already " + golemTable.ContainsKey(currentTime));
        golemTable.Add(currentTime, penSlot);
        //Save();
    }

    public void AddGolemReadyToCollect(int penSlot) {
        instantReturn.Add(penSlot);
    }
    public List<int> ReturnReadyGolems() {
        //Find Golems to be returned
        returningGolems = new Dictionary<DateTime, int>();
        foreach (KeyValuePair<DateTime, int> golem in golemTable) {
            TimeSpan elapsedTime = DateTime.Now - golem.Key;
            if (elapsedTime.Seconds > mineTimeSeconds) {
                returningGolems.Add(golem.Key,golem.Value);   
            }
        }
        //Remove found golems
        foreach(KeyValuePair<DateTime, int> golem in returningGolems) {
            if (golemTable.ContainsKey(golem.Key)) {
                golemTable.Remove(golem.Key);
            }
        }
        //Return only golem objects
        List<int> golems = new List<int>();
        foreach(KeyValuePair<DateTime, int> golem in returningGolems) {
            golems.Add(golem.Value);
        }

        //Add Instant return golems if any
        foreach(int golem in instantReturn) {
            golems.Add(golem);
        }
        instantReturn = new List<int>();
        //Save();
        return golems;
    }

    public bool ReadyToCollect() {
        foreach (KeyValuePair<DateTime, int> golem in golemTable) {
            TimeSpan elapsedTime = DateTime.Now - golem.Key;
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

    public bool GolemsInMine()
    {
        if (golemTable.Count > 0 || instantReturn.Count > 0)
        {
            return true;
        }

        return false;
    }

    public bool ReadyToReturn()
    {
        if (instantReturn.Count > 0)
        {
            return true;
        }

        return false;
    }

    public int AmountOfGolemsInMine()
    {
        return golemTable.Count;
    }

    public int AmountReadyToReturn()
    {
        return instantReturn.Count; //+ ReadyToReturnGolems();
    }
    
    /*
    private int ReadyToReturnGolems() {
        //Find Golems to be returned
        int numberOfGolems = 0;
        foreach (KeyValuePair<DateTime, int> golem in golemTable) {
            TimeSpan elapsedTime = DateTime.Now - golem.Key;
            if (elapsedTime.Seconds > mineTimeSeconds)
            {
                numberOfGolems++;
            }
        }

        return numberOfGolems;
    }
    */
    public List<DateTime> TimeRemaining()
    {
        List<DateTime> entryTimes = new List<DateTime>();
        foreach (KeyValuePair<DateTime, int> golem in golemTable)
        {
            entryTimes.Add(golem.Key);
        }

        return entryTimes;
    }

    public float MiningTime()
    {
        return mineTimeSeconds;
    }
}
