using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : Item {

    private ItemType type;
    private Quality quality;


    public Brick(Quality quality) {
        type = ItemType.Brick;
        this.quality = quality;
        quantity = 1;
        stackLimit = 1;
        merging = false;
    }

}
