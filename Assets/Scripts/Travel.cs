﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public static class Travel {
    public static List<Towns> unlockedTowns {
        get { return Inventory.Instance.GetUnlockedTowns(); }
    }

    public static Towns currentTown {
        get { return Inventory.Instance.GetCurrentTown(); }
    }

    //Replace town names once confirmed
    //All Possible towns in game
    public enum Towns {
        Tutorial,
        WickedGrove,
        FlamingPeak,
        GiantsPass,
        SkyCity
    }

    public static void initialSetup() {
        /*
        lockedTowns.Add(Towns.WickedGrove);
        lockedTowns.Add(Towns.Chelm);
        lockedTowns.Add(Towns.Town3);
        lockedTowns.Add(Towns.Town4);
        lockedTowns.Add(Towns.Town5);
        */
    }

    //Costs to unlock various towns
    public static int[] costsToUnlock = {
        0,
        0,
        150,
        200,
        300
    };

    //Method to unlock new town
    public static bool UnlockNewTown(Towns newTown) {
        int newTownCost = NextPurchaseCost();
        if (Inventory.Instance.GetUnlockedTowns().Contains(newTown)) {
            Debug.Log("UnlockNewTown(): " + newTown.ToString() + " is already unlocked.");
            return false;
        }
        
        if (Inventory.Instance.RemoveGold(newTownCost)) {
            Inventory.Instance.UnlockTown(newTown);
            AchievementManager.Get("unlock_town_01");
            AchievementManager.Increment("all_towns");
            //unlockedTowns.Add(newTown);
            //lockedTowns.Remove(newTown);
            Debug.Log("Town is unlocked: " + newTown);
            return true;
        } else {
            return false;
        }
    }

    public static void ChangeCurrentTown(Towns newTown) {
        if (Inventory.Instance.GetUnlockedTowns().Contains(newTown)) {
            Inventory.Instance.SetCurrentTown(newTown);
        }
    }

    public static int NextPurchaseCost() {
        return costsToUnlock[Inventory.Instance.GetUnlockedTowns().Count];
    }

    public static Towns ReturnCurrentTown() {
        return Inventory.Instance.GetCurrentTown();
    }

    public static Item.GemType CurrentTownGemType()
    {
        switch (currentTown)
        {
                case Towns.WickedGrove:
                    return Item.GemType.Emerald;
                case Towns.FlamingPeak:
                    return Item.GemType.Ruby;
                case Towns.SkyCity:
                    return Item.GemType.Sapphire;
                case Towns.GiantsPass:
                    return Item.GemType.Amethyst;
                default:
                    return Item.GemType.Ruby;
        }
    }
}
