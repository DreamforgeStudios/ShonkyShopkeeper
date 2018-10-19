using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AchievementManager {
    private static AchievementDatabase
        achievementDB = Object.Instantiate((AchievementDatabase) Resources.Load("AchievementDatabase"));

    public static bool Get(string key) {
        var achievement = GetAchievement(key);
        if (achievement == null) {
            return false;
        }

        bool unlocked = achievement.Unlocked;
        
        if (Initiate.IsLoading) {
            Initiate.onFinishFading += () => DoGet(achievement);
        } else {
            DoGet(achievement);
        }

        return unlocked;
    }
    
    public static void Increment(string key, int amount = 1) {
        // Queue up displays AFTER the scene has loaded.  Avoids things randomly popping up and being jarring while the
        // wipe is playing.  We lose the boolean return value here.  Bummer
        if (Initiate.IsLoading) {
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

    private static Achievement GetAchievement(string key) {
        Achievement a;
        achievementDB.TryFindAchievementWithKey(key, out a);
        return a;
    }

    // Returns true if successfully unlocked.  If false, already unlocked.
    private static bool DoGet(string key) {
        Achievement a;
        if (achievementDB.TryFindAchievementWithKey(key, out a)) {
            return achievementDB.Unlock(a);
        }
        
        return false;
    }
    
    private static bool DoGet(Achievement a) {
        return achievementDB.Unlock(a);
    }

    private static bool DoIncrement(string key, int amount) {
        Achievement a;
        if (achievementDB.TryFindAchievementWithKey(key, out a)) {
            return achievementDB.Increment(a, amount);
        }
        
        return false;
    }
    
    private static bool DoIncrement(Achievement a, int amount) {
        return achievementDB.Increment(a, amount);
    }

    public static void Reinit() {
        achievementDB = Object.Instantiate((AchievementDatabase) Resources.Load("AchievementDatabase"));
    }
}
