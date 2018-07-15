using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using NaughtyAttributes;

[System.Serializable]
[CreateAssetMenu(menuName = "AchievementDatabase", fileName = "AchievementDatabase.asset")]
public class AchievementDatabase : ScriptableObject {
    public GameObject AchievementPrefab;
    
    // Unity doesn't show dictionaries in the inspector, so they cannot be assigned in the inspector.
    // ... thus, we use a List and lazily translate it at runtime.
    // If translating the list to a dictionary proves bad for performance, we can probably bake it in some way.
    public List<KeyedAchievement> AchievementList;

    public Dictionary<string, Achievement> achievementDictionary = null;
    public Dictionary<string, Achievement> AchievementDictionary {
        get {
            if (achievementDictionary == null)
                achievementDictionary = BuildDictionary(AchievementList);

            return achievementDictionary;
        }
    }

    private List<string> achievedList = null;
    public List<string> AchievedList {
        get {
            if (achievedList == null)
                achievedList = LoadAchieved();
            
            return achievedList;
        }
    }

    public bool Unlock(string key) {
        Achievement a;
        if (AchievementDictionary.TryGetValue(key, out a)) {
            a.Unlocked = true;
            Display(a);
            return true;
        }

        return false;
    }

    // TODO: tween in.
    private void Display(Achievement achievement) {
        var clone = Instantiate(AchievementPrefab, FindObjectOfType<Canvas>().transform).GetComponent<AchievementGameObject>();
        clone.Icon.sprite = achievement.Icon;
        clone.Heading.text = achievement.Heading;
        clone.Description.text = achievement.Description;
        // clone.Background.sprite = achievement.Background;
    }
    
    
    
    
    
    private Dictionary<string, Achievement> BuildDictionary(List<KeyedAchievement> achievements) {
        var dict = new Dictionary<string, Achievement>();
        AchievementList.ForEach(x => dict.Add(x.Key, x.Achievement));

        return dict;
    }
    
    private List<string> LoadAchieved() {
        var path = Path.Combine(Application.persistentDataPath, "achievements.txt");
        var list = new List<string>();

        try {
            list = File.ReadAllLines(path).ToList();
        } catch (FileNotFoundException) {
            list = new List<string>();
        }

        foreach (var key in list) {
            if (AchievementDictionary.ContainsKey(key))
                AchievementDictionary[key].Unlocked = true;
            else if (key != "")
                Debug.Log("Did not find an achievement that corresponded to the key \""
                          + key + "\", is there a typo in achievements.txt?");
        }

        return list;
    }

    [Button("Save")]
    private void SaveAchieved() {
        var achievedKeys = AchievementDictionary.Where(x => x.Value.Unlocked).Select(a => a.Key).ToArray();
        var path = Path.Combine(Application.persistentDataPath, "achievements.txt");
        File.WriteAllLines(path, achievedKeys);
    }

    [Button("Load")]
    private void Load() {
        achievedList = LoadAchieved();
    }

    [Button("Print Achieved")]
    private void Print() {
        AchievedList.ForEach(i => Debug.Log(i));
    }
}
