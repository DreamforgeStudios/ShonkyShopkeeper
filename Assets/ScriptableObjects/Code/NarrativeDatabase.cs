using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.IO.IsolatedStorage;

// I'm not sure if it's better to merge this with AchievementDatabase using method overloading, or keep it separate...
[System.Serializable]
[CreateAssetMenu(menuName = "NarrativeDatabase", fileName = "NarrativeDatabase.asset")]
public class NarrativeDatabase : ScriptableObject {
	public GameObject GizmoPrefab;
    
    private PopupTextManager gizmoPrefabInstance;
    public PopupTextManager GizmoPrefabInstance {
        get {
            if (gizmoPrefabInstance == null) {
                gizmoPrefabInstance = Instantiate(GizmoPrefab).GetComponent<PopupTextManager>();
            }

            return gizmoPrefabInstance;
            
        }
    }
	
    // The dictionary we define narrative elements in.
    public StringNarrativeElementDictionary KeyedNarrativeElements;
    // The 'working' dictionary we actually use.
    private StringNarrativeElementDictionary narrativeElementDictionary = null;
    public StringNarrativeElementDictionary NarrativeElementDictionary {
        get {
            if (narrativeElementDictionary == null) {
                LoadNarratives();
                narrativeElementDictionary = KeyedNarrativeElements;
            }

            return narrativeElementDictionary;
        }
    }

    public bool TryFindNarrativeWithKey(string key, out NarrativeElement e) {
        if (NarrativeElementDictionary.TryGetValue(key, out e)) {
            return true;
        }
        
        Debug.Log("Couldn't find a narrative element matching key \"" + key + "\".");
        return false;
    }

    public bool Unlock(NarrativeElement e) {
        if (e.Unlocked)
            return false;

        e.Unlocked = true;
        Display(e);

        SaveRead();

        return true;
    }

    private void Display(NarrativeElement element) {
        var clone = GizmoPrefabInstance;
        clone.PopupTexts = element.Texts;
        clone.Init();
        clone.DoEnterAnimation();
    }

    private void LoadNarratives() {
        var path = Path.Combine(Application.persistentDataPath, "narratives.txt");
        
        try {
            var lines = File.ReadAllLines(path);
            foreach (var line in lines) {
                if (KeyedNarrativeElements.ContainsKey(line)) {
                    KeyedNarrativeElements[line].Unlocked = true;
                } else if (line != "") {
                    Debug.Log("Did not find a narrative element that corresponded to the key \""
                              + line + "\", is there a typo in narratives.txt?");
                }
            }
            
        } catch (FileNotFoundException) {
            Debug.Log("narratives.txt was not found and will be created.");
        } catch (IsolatedStorageException) {
            Debug.Log("narratives.txt was not found and will be created.");
        } catch (Exception e) {
            Debug.Log("Something went wrong in NarrativeDatabase: " + Environment.NewLine + e.ToString());
        }
    }

    private void SaveRead() {
        List<string> readKeys = new List<string>();
        
        foreach (var pair in NarrativeElementDictionary) {
            if (pair.Value.Unlocked) {
                readKeys.Add(pair.Key);
            }
        }
        
        var path = Path.Combine(Application.persistentDataPath, "narratives.txt");
        File.WriteAllLines(path, readKeys.ToArray());
    }
}
