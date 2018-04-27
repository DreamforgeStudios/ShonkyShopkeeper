using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ore : Item {

    private ItemType type;


    public Ore() {
        type = ItemType.Ore;
        quantity = 1;
        stackLimit = 3;
        merging = false;
    }

    public Ore(int quantity) {
        type = ItemType.Ore;
        this.quantity = quantity;
        stackLimit = 3;
        merging = false;
    }
}
