using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour {
    public enum ItemType {
        Ore, Brick, Shell, Gem, Jewel, ChargedJewel, Shonky
    }

    public enum GemType {
        NotGem, Ruby, Diamond, Sapphire, Emerald
    }

    public Quality.QualityGrade qualityGrade;

    public int quantity;
    public int stackLimit;
    public bool merging;

    // Use this for initialization
    /*
    public Item () {
    }
    */

	public abstract string ItemName();
	public abstract string ItemInfo();
}

