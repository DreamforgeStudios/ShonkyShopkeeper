using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ore : Item {

    private ItemType type;
    private Quality quality;


    public Ore() {
        type = ItemType.Ore;
        quality = Quality.NotGraded;
        quantity = 1;
        stackLimit = 3;
        merging = false;
    }

    public Ore(int quantity) {
        type = ItemType.Ore;
        quality = Quality.NotGraded;
        this.quantity = quantity;
        stackLimit = 3;
        merging = false;
    }
}
