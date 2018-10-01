using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public enum Difficulty {
    Easy,
    Normal,
    Hard
}

[CreateAssetMenu(menuName = "Data/PersistentData", fileName = "PersistentData.asset")]
[System.Serializable]
public class PersistentData : ScriptableObject {
    private static PersistentData _instance;
    public static PersistentData Instance {
        get {
            if (!_instance) {
                // Find the instance if it has already been instantiated -- we might have just lost reference.
                PersistentData[] tmp = Resources.FindObjectsOfTypeAll<PersistentData>();
                if (tmp.Length > 0) {
                    _instance = tmp[0];
                } else {
                    _instance = LoadOrInit();
                }
            }

            return _instance;
        }
    }

    // Simple links to other data things, to make things a bit easier.
    // Of course, you can just access these normally as well.
    
    // Not serialized because it uses get;
    public Inventory Inventory {
        get { return Inventory.Instance; }
    }

    public ShonkyInventory ShonkyInventory {
        get { return ShonkyInventory.Instance; }
    }


    // Missing some stuff here, I'm sure.
    public int GolemsCrafted;
    public int TrueGolemsCrafted {
        get { return Inventory.Instance.GetUnlockedTrueGolems().Count; }
    }
    
    public int TownsUnlocked {
        get { return Inventory.Instance.GetUnlockedTowns().Count; }
    }

    public int MysticItemsCrafted,
        MagicalItemsCrafted,
        SturdyItemsCrafted,
        PassableItemsCrafted,
        BrittleItemsCrafted,
        JunkItemsCrafted;

    public int JewelsCrafted,
        ChargedJewelsCrafted,
        BricksCrafted,
        ShellsCrafted;

    public int TotalItemsCrafted;

    public int GoldEarnt,
        GoldSpent;

    public Difficulty Difficulty;

    public void AddItem(ItemInstance item) {
        switch (item.Quality) {
            case Quality.QualityGrade.Mystic:
                MysticItemsCrafted++;
                break;
            case Quality.QualityGrade.Magical:
                MagicalItemsCrafted++;
                break;
            case Quality.QualityGrade.Sturdy:
                SturdyItemsCrafted++;
                break;
            case Quality.QualityGrade.Passable:
                PassableItemsCrafted++;
                break;
            case Quality.QualityGrade.Brittle:
                BrittleItemsCrafted++;
                break;
            case Quality.QualityGrade.Junk:
                JunkItemsCrafted++;
                break;
        }

        Type t = item.GetType();
        if (t == typeof(Jewel)) {
            JewelsCrafted++;
        } else if (t == typeof(ChargedJewel)) {
            ChargedJewelsCrafted++;
        } else if (t == typeof(Brick)) {
            BricksCrafted++;
        } else if (t == typeof(Shell)) {
            ShellsCrafted++;
        }
    }
    
    private static PersistentData LoadOrInit() {
        PersistentData pd;
        string path = Path.Combine(Application.persistentDataPath, "persistentdata.json");
        
        // Saving and loading.
        if (File.Exists(path)) {
            pd = CreateInstance<PersistentData>();
            JsonUtility.FromJsonOverwrite(File.ReadAllText(path), pd);
            pd.hideFlags = HideFlags.HideAndDontSave;
            
        } else {
            pd = Instantiate((PersistentData) Resources.Load("PersistentData"));
            pd.hideFlags = HideFlags.HideAndDontSave;
        }

        return pd;
    }

    public void Save() {
        string path = Path.Combine(Application.persistentDataPath, "persistentdata.json");
        File.WriteAllText(path, JsonUtility.ToJson(this, true));
    }
}
