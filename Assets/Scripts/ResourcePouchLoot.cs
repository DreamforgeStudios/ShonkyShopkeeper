using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePouchLoot {
    // In the situation where we haven't saved an inventory before.
    public Inventory defaultInventory;

    //Test resource pouch
    public ItemInstance resourcePouch;
    public enum MineVeinType {
        Diamond,
        Sapphire,
        Emerald,
        Ruby
    }

    //Type of this bag
    private MineVeinType veinType;

    //Percentages for drops
    public float orePercentage;
    public float gemPercentage;

    //Total number of items to receive
    public float numberItems = Random.Range(1, 5);

    //Total number of items received
    private int receivedSoFar;

    public int CalculateOreDrop(float oreDropRate, float numberOfItems) {
        //CalculateDropRates(vein);
        int oreAmount = (int)Mathf.Round((numberOfItems / 100) * oreDropRate);
        Debug.Log("Ore amount = " + oreAmount);
        return oreAmount;
    }

    public int CalculateGemDrop(int oreAmount, float totalNumberToDrop) {
        Debug.Log("Gem amount = " + (int)(totalNumberToDrop - oreAmount));
        return (int)(totalNumberToDrop - oreAmount);
    }

    public int CalculateDropRates() {
        /*
        switch (vein) {
            case MineVeinType.Diamond:
                orePercentage = Random.Range(50, 70);
                break;
            case MineVeinType.Sapphire:
                orePercentage = Random.Range(60, 80);
                break;
            case MineVeinType.Emerald:
                orePercentage = Random.Range(40, 70);
                break;
            case MineVeinType.Ruby:
                orePercentage = Random.Range(70, 75);
                break;
            default:
                orePercentage = 50;
                break;
        }
        */
        return (Random.Range(50, 70));
        //gemPercentage = 100 - orePercentage;
    }
}
