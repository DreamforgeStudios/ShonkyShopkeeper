using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AchievementManager {
    private static AchievementDatabase
        achievementDB = Object.Instantiate((AchievementDatabase) Resources.Load("AchievementDatabase"));

    public static bool Get(string key) {
        // If true, successfully unlocked.  If false, already unlocked.
        Achievement a;
        if (achievementDB.TryFindAchievementWithKey(key, out a)) {
            //SFX.Play("sound");
            SFX.Play("Achieve_Popup", 1f, 1f, 0f, false, 0f);
            return achievementDB.Unlock(a);
        }
        
        return false;
    }
    
    public static bool Increment(string key, int amount = 1) {
        Achievement a;
        if (achievementDB.TryFindAchievementWithKey(key, out a)) {
            return achievementDB.Increment(a, amount);
        }
        
        return false;
    }
}
