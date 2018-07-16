using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Achievement {
    //[Tooltip("A string that should uniquely identify this achievement.")]
    //public string Key;
    public Sprite Icon;
    public string Heading = "Achievement Get!";
    public string Description = "Something good happened.";
    public int Progress = 0;
    public int FinalProgress = 0;
    public bool Unlocked = false;
}

[System.Serializable]
public class KeyedAchievement {
    [Tooltip("A string that should uniquely identify this achievement.")]
    public string Key;
    public Achievement Achievement;
}
