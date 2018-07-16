using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using NaughtyAttributes;

public class AchievementTest : MonoBehaviour {
    public string achievementKey;
    
    [Button("Get Achievement")]
    public void GetAchievementTest() {
        AchievementManager.Get(achievementKey);
    }
}
