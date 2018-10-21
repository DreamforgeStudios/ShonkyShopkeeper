﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public static class TrueGolems {

    public enum TrueGolem
    {
        nill,
        rubyGolem,
        sapphireGolem,
        amethystGolem,
        emeraldGolem
    }

    public static List<TrueGolem> unlockedTrueGolems {
        get { return Inventory.Instance.GetUnlockedTrueGolems(); }
    }

    public static bool PotentialUnlockTrueGolem(TrueGolem golemToUnlock)
    {
        if (unlockedTrueGolems.Contains(golemToUnlock))
        {
            Debug.Log("not true golem");
            return false;
        }

        Debug.Log("True golem");
        return true;
    }

    public static TrueGolem GemStringToGolem(string input)
    {
        string caseFixed = input.ToLower();
        switch (caseFixed)
        {
            case "ruby":
                return TrueGolem.rubyGolem;
            case "sapphire":
                return TrueGolem.sapphireGolem;
            case "emerald":
                return TrueGolem.emeraldGolem;
            case "amethyst":
                return TrueGolem.amethystGolem;
            default:
                return TrueGolem.nill;
        }
    }
    
    
}
