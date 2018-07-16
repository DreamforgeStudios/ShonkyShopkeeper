using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using NaughtyAttributes;
using DG.Tweening;

[System.Serializable]
[CreateAssetMenu(menuName = "AchievementDatabase", fileName = "AchievementDatabase.asset")]
public class AchievementDatabase : ScriptableObject {
    public GameObject AchievementPrefab;
    
    private AchievementGameObject achievementPrefabInstance = null;
    public AchievementGameObject AchievementPrefabInstance {
        get {
            if (achievementPrefabInstance == null)
                achievementPrefabInstance = Instantiate(AchievementPrefab, FindObjectOfType<Canvas>().transform)
                                            .GetComponent<AchievementGameObject>();
            
            return achievementPrefabInstance;
        }
    }
    
    // Unity doesn't show dictionaries in the inspector, so they cannot be assigned in the inspector.
    // ... thus, we use a List and lazily translate it at runtime.
    // If translating the list to a dictionary proves bad for performance, we can do it in Awake() instead.
    public List<KeyedAchievement> AchievementList;

    private Dictionary<string, Achievement> achievementDictionary = null;
    public Dictionary<string, Achievement> AchievementDictionary {
        get {
            if (achievementDictionary == null) {
                achievementDictionary = BuildDictionary(AchievementList);
            }

            return achievementDictionary;
        }
    }

    private List<KeyValuePair<string, int>> achievedList = null;
    public List<KeyValuePair<string, int>> AchievedList {
        get {
            if (achievedList == null)
                achievedList = LoadAchieved();
            
            return achievedList;
        }
    }

    public bool TryFindAchievementWithKey(string key, out Achievement a) {
        if (AchievementDictionary.TryGetValue(key, out a)) {
            return true;
        }

        Debug.Log("Couldn't find an achievement matching key \"" + key + "\".");
        return false;
    }

    public bool Unlock(Achievement a) {
        // Already unlocked.
        if (a.Unlocked)
            return false;
        
        a.Unlocked = true;
        Display(a);
        return true;
    }

    public bool Increment(Achievement a, int amount) {
        if (a.Unlocked || a.Progress >= a.FinalProgress)
            return false;
        
        a.Progress += amount;
        if (a.Progress == a.FinalProgress)
            Unlock(a);
            
        return true;
    }
    
    // TODO: tween in and out.
    private void Display(Achievement achievement) {
        var clone = AchievementPrefabInstance;
        clone.Icon.sprite = achievement.Icon;
        clone.Heading.text = achievement.Heading;
        clone.Description.text = achievement.Description;
        // clone.Background.sprite = achievement.Background;

        clone.transform.localScale = Vector3.zero;
        clone.gameObject.SetActive(true);
        clone.transform.DOScale(1, .35f).SetEase(Ease.OutBack);
        clone.transform.DOScale(0, .35f).SetEase(Ease.OutBack).SetDelay(3f);
    }
    
    
    
    private Dictionary<string, Achievement> BuildDictionary(List<KeyedAchievement> achievements) {
        var dict = new Dictionary<string, Achievement>();
        achievements.ForEach(x => dict.Add(x.Key, x.Achievement));

        foreach (var kvp in AchievedList) {
            if (AchievementDictionary.ContainsKey(kvp.Key)) {
                AchievementDictionary[kvp.Key].Unlocked = true;
                AchievementDictionary[kvp.Key].Progress = kvp.Value;
            } else if (kvp.Key != "") {
                Debug.Log("Did not find an achievement that corresponded to the key \""
                          + kvp.Key + "\", is there a typo in achievements.txt?");
            }
        }
        
        Debug.Log("Built dictionary.");

        return dict;
    }
    
    private List<KeyValuePair<string, int>> LoadAchieved() {
        var path = Path.Combine(Application.persistentDataPath, "achievements.txt");
        var list = new List<KeyValuePair<string, int>>();

        try {
            var lines = File.ReadAllLines(path);
            foreach (var line in lines) {
                var kvp = line.Split(',');
                list.Add(new KeyValuePair<string, int>(kvp[0], Int32.Parse(kvp[1])));
            }
        }
        catch (FileNotFoundException) {
            Debug.Log("achievements.txt was not found and will be created.");
        }
        catch (IndexOutOfRangeException) {
            Debug.Log("Problem parsing achievements.txt, probably missing a ',' somewhere.");
        }
        catch (FormatException) {
            Debug.Log("Failed to parse an int in achievements.txt.");
        }

        return list;
    }

    [Button("Save")]
    private void SaveAchieved() {
        List<string> achievedKeys = new List<string>();
        
        foreach (var pair in AchievementDictionary) {
            if (pair.Value.Unlocked || pair.Value.Progress > 0) {
                achievedKeys.Add(pair.Key + "," + pair.Value.Progress);
            }
        }
        
        var path = Path.Combine(Application.persistentDataPath, "achievements.txt");
        File.WriteAllLines(path, achievedKeys.ToArray());
    }

    [Button("Load")]
    private void Load() {
        achievedList = LoadAchieved();
    }

    [Button("Print Achieved")]
    private void Print() {
        AchievedList.ForEach(i => Debug.Log(i));
    }

    [Button("Lock All")]
    private void Lock() {
        AchievementList.ForEach(x => {
            x.Achievement.Unlocked = false;
            x.Achievement.Progress = 0;
        });

        achievementDictionary = BuildDictionary(AchievementList);
    }
}
