using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AchievementManager {
    private static AchievementDatabase
        achievementDB = Object.Instantiate((AchievementDatabase) Resources.Load("AchievementDatabase"));

    public static void Get(string key) {
        // Queue up displays AFTER the scene has loaded.  Avoids things randomly popping up and being jarring while the
        // wipe is playing.  We lose the boolean return value here.  Bummer
        if (Initiate.IsFading) {
            Initiate.onFinishFading += () => DoGet(key);
        } else {
            DoGet(key);
        }
    }
    
    public static void Increment(string key, int amount = 1) {
        // Queue up displays AFTER the scene has loaded.  Avoids things randomly popping up and being jarring while the
        // wipe is playing.  We lose the boolean return value here.  Bummer
        if (Initiate.IsFading) {
            Initiate.onFinishFading += () => DoIncrement(key, amount);
        } else {
            DoIncrement(key, amount);
        }
    }

    public static bool CheckUnlocked(string key) {
        Achievement a;
        if (achievementDB.TryFindAchievementWithKey(key, out a)) {
            return a.Unlocked;
        }

        return false;
    }

    // Returns true if successfully unlocked.  If false, already unlocked.
    private static bool DoGet(string key) {
        Achievement a;
        if (achievementDB.TryFindAchievementWithKey(key, out a)) {
            return achievementDB.Unlock(a);
        }
        
        return false;
    }

    private static bool DoIncrement(string key, int amount) {
        Achievement a;
        if (achievementDB.TryFindAchievementWithKey(key, out a)) {
            return achievementDB.Increment(a, amount);
        }
        
        return false;
    }
}
