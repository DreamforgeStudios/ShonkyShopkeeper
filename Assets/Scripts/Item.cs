using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {
    public enum ItemType {
        Ore, Brick, Shell, Gem, Jewel, ChargedJewel
    }
    public enum Quality {
        NotGraded, F, E, D, C, B, A
    }
    public enum GemType {
        NotGem, Ruby, Diamond, Sapphire, Emerald
    }

    public int quantity;
    public int stackLimit;
    public bool merging;
    // Use this for initialization
    public Item () {

    }
}

