using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.CompilerServices;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;

[System.Serializable]
public class StringAchievementDictionary : SerializableDictionary<string, Achievement> {}
[System.Serializable]
public class StringNarrativeElementDictionary : SerializableDictionary<string, NarrativeElement> {}

[System.Serializable]
[CreateAssetMenu(menuName = "AchievementDatabase", fileName = "AchievementDatabase.asset")]
public class AchievementDatabase : ScriptableObject {
    public GameObject AchievementPrefab;
    
    // Working copy of the achievement prefab.  Trying to minimize instantiation when possible.
    private AchievementGameObject achievementPrefabInstance = null;
    public AchievementGameObject AchievementPrefabInstance {
        get {
            if (achievementPrefabInstance == null) {
                achievementPrefabInstance = Instantiate(AchievementPrefab, FindObjectOfType<Canvas>().transform)
                    .GetComponent<AchievementGameObject>();
            }

            return achievementPrefabInstance;
        }
    }
    
    // The dictionary we define achievements in.
    public StringAchievementDictionary KeyedAchievements;
    // The 'working' dictionary that we actually use.
    private StringAchievementDictionary achievementDictionary = null;
    public StringAchievementDictionary AchievementDictionary {
        get {
            if (achievementDictionary == null) {
                LoadAchieved();
                achievementDictionary = KeyedAchievements;
            }

            return achievementDictionary;
        }
    }

    // Tries to find an achievement which matches the provided key.
    // Returns true if an achievement was found, and false if not.
    public bool TryFindAchievementWithKey(string key, out Achievement a) {
        if (AchievementDictionary.TryGetValue(key, out a)) {
            return true;
        }

        Debug.Log("Couldn't find an achievement matching key \"" + key + "\".");
        return false;
    }

    // Unlocks the achievement passed in.
    // Returns true if successful, or false if the achievement was already unlocked.
    public bool Unlock(Achievement a) {
        // Already unlocked.
        if (a.Unlocked)
            return false;
        
        a.Unlocked = true;
        Display(a);
        
        SaveAchieved();
        
        return true;
    }

    // Increments an achievement.
    // Returns the same as Unlock().
    public bool Increment(Achievement a, int amount) {
        if (a.Unlocked || a.Progress >= a.FinalProgress)
            return false;
        
        a.Progress += amount;
        if (a.Progress == a.FinalProgress)
            Unlock(a);
        
        SaveAchieved();
            
        return true;
    }
    
    // Display a given achievement on the screen.
    private void Display(Achievement achievement) {
        SFX.Play("Achieve_Popup", 1f, 1f, 0f, false, 0f);
        
        var clone = AchievementPrefabInstance;
        clone.Icon.sprite = achievement.Icon;
        clone.Heading.text = achievement.Heading;
        clone.Description.text = achievement.Description;
        // clone.Background.sprite = achievement.Background;

        clone.transform.localScale = Vector3.zero;
        clone.gameObject.SetActive(true);
        clone.transform.DOScale(1, .35f).SetEase(Ease.InQuad);
        clone.transform.DOScale(0, .35f).SetEase(Ease.OutQuad).SetDelay(3f);
    }
    
    // Loads achievements from the achievements.txt file into the KeyedAchievements dictionary.
    private void LoadAchieved() {
        var path = Path.Combine(Application.persistentDataPath, "achievements.txt");

        try {
            var lines = File.ReadAllLines(path);
            foreach (var line in lines) {
                var kvp = line.Split(',');
                //list.Add(new KeyValuePair<string, int>(kvp[0], Int32.Parse(kvp[1])));
                
                if (KeyedAchievements.ContainsKey(kvp[0])) {
                    KeyedAchievements[kvp[0]].Unlocked = true;
                    KeyedAchievements[kvp[0]].Progress = Int32.Parse(kvp[1]);
                } else if (kvp[0] != "") {
                    Debug.Log("Did not find an achievement that corresponded to the key \""
                              + kvp[0] + "\", is there a typo in achievements.txt?");
                }
            }
            
        } catch (FileNotFoundException) {
            Debug.Log("achievements.txt was not found and will be created.");
        } catch (IsolatedStorageException) {
            Debug.Log("achievements.txt was not found and will be created.");
        } catch (IndexOutOfRangeException) {
            Debug.Log("Problem parsing achievements.txt, probably missing a ',' somewhere.");
        } catch (FormatException) {
            Debug.Log("Failed to parse an int in achievements.txt.");
        } catch (Exception e) {
            Debug.Log("Something went wrong in AchievementDatabase: " + Environment.NewLine + e.ToString());
        }
    }

    [Button("Save")]
    // Saves the achieved achievements (and their progress) to a text file.
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
        LoadAchieved();
    }

    [Button("Print Achieved")]
    private void Print() {
        foreach (var kvp in AchievementDictionary) {
            if (kvp.Value.Unlocked) {
                Debug.Log(kvp.Key);
            }
        }
    }

    public void ResetAchievementFile()
    {
        string[] emptyString =
        {

        };
        var path = Path.Combine(Application.persistentDataPath, "achievements.txt");
        File.WriteAllLines(path, emptyString);
    }
}
