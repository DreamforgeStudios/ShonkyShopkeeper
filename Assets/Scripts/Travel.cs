using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public static class Travel {
    public static Towns currentTown;
    public static List<Towns> unlockedTowns = new List<Towns>();
    public static List<Towns> lockedTowns = new List<Towns>();

    //Replace town names once confirmed
    //All Possible towns in game
    public enum Towns {
        Town1,
        Town2,
        Town3,
        Town4,
        Town5
    }

    public static void initialSetup() {
        unlockedTowns.Add(Towns.Town1);
        lockedTowns.Add(Towns.Town2);
        lockedTowns.Add(Towns.Town3);
        lockedTowns.Add(Towns.Town4);
        lockedTowns.Add(Towns.Town5);
    }

    //Costs to unlock various towns
    public static int[] costsToUnlock = {
        0,
        1000,
        5000,
        13000,
        50000
    };

    //Method to unlock new town
    public static bool UnlockNewTown(Towns newTown) {
        int newTownCost = costsToUnlock[(int)newTown];
        if (true){//Inventory.RemoveGold(newTownCost)) {
            unlockedTowns.Add(newTown);
            lockedTowns.Remove(newTown);
            return true;
        } else {
            return false;
        }
    }

    public static void ChangeCurrentTown(Towns newTown) {
        if (unlockedTowns.Contains(newTown)) {
            currentTown = newTown;
        }
    }

}
