using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour {
    //Time till the golem is complete mining
    public static int mineTimeSeconds = 10;
    private static Dictionary<DateTime, GameObject> returningGolems = new Dictionary<DateTime, GameObject>();

    private static Mine _instance;
    public static Mine Instance {
        get {
            return _instance;
        }
    }

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        }
        else {
            _instance = this;
        }
    }
    public static Dictionary<DateTime, GameObject> golemTable = new Dictionary<DateTime, GameObject>();
	
    public static void AddGolemAndTime(DateTime currentTime, GameObject golem) {
        Debug.Log(currentTime + " and does contain already " + golemTable.ContainsKey(currentTime));
        golemTable.Add(currentTime, golem);
    }

    public static List<GameObject> ReturnReadyGolems() {
        //Find Golems to be returned
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
        return golems;
    }

    public static bool ReadyToCollect() {
        foreach (KeyValuePair<DateTime, GameObject> golem in golemTable) {
            TimeSpan elapsedTime;
            elapsedTime = DateTime.Now - golem.Key;
            if (elapsedTime.Seconds > mineTimeSeconds)
                return true;
        }
        return false;
    }
}
