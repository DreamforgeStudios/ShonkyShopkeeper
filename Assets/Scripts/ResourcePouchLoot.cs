using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePouchLoot : MonoBehaviour {

    public enum MineVeinType {
        Diamond,
        Sapphire,
        Emerald,
        Ruby
    }

    //Type of this bag
    private MineVeinType veinType;

    public ResourcePouchLoot() {

    }
    //Percentages for drops
    public float orePercentage;
    public float gemPercentage;

    //Total number of items to receive
    public static float numberOfItems = 15;

    //Total number of items received
    private int receivedSoFar;

    public int CalculateOreDrop(MineVeinType vein, float oreDropRate) {
        CalculateDropRates(vein);
        int oreAmount = (int)Mathf.Round((numberOfItems / 100) * oreDropRate);
        Debug.Log("Ore amount = " + oreAmount);
        return oreAmount;
    }

    public int CalculateGemDrop(int oreAmount, float totalNumberToDrop) {
        return (int)(totalNumberToDrop - oreAmount);
    }

    public void CalculateDropRates(MineVeinType vein) {
        switch (vein) {
            case MineVeinType.Diamond:
                orePercentage = 70;
                gemPercentage = 30;
                break;
            case MineVeinType.Sapphire:
                orePercentage = 70;
                gemPercentage = 30;
                break;
            case MineVeinType.Emerald:
                orePercentage = 70;
                gemPercentage = 30;
                break;
            case MineVeinType.Ruby:
                orePercentage = 70;
                gemPercentage = 30;
                break;
            default:
                orePercentage = 80;
                gemPercentage = 20;
                break;
        }
    }
}
