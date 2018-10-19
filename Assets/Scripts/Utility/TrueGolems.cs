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
            Debug.Log("Have this true golem");
            return false;
        }

        Debug.Log("Don't have this True golem");
        return true;
    }

    public static TrueGolem GemStringToGolem(string input)
    {
        string caseFixed = input.ToLower();
        switch (caseFixed)
        {
            case "rubyGolem1":
                return TrueGolem.rubyGolem;
            case "sapphireGolem1":
                return TrueGolem.sapphireGolem;
            case "emeraldGolem1":
                return TrueGolem.emeraldGolem;
            case "amethystGolem1":
                return TrueGolem.amethystGolem;
            default:
                return TrueGolem.nill;
        }
    }
    
    
}
