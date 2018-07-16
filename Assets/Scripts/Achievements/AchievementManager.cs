using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AchievementManager {
    private static AchievementDatabase
        achievementDB = (AchievementDatabase) Resources.Load("AchievementDatabase");

    public static bool Get(string key) {
        // If true, successfully unlocked.  If false, already unlocked.
        Achievement a;
        if (achievementDB.TryFindAchievementWithKey(key, out a)) {
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
