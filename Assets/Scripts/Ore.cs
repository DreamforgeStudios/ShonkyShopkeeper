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
        this.gemType = GemType.NotGem;
        stackLimit = 3;
        merging = false;
    }

    public override string ItemName() {
        return "Ore";
    }

    public override string ItemInfo() {
        return string.Format("Quantity: {0}" + System.Environment.NewLine +
					         "Stack Limit: {1}" + System.Environment.NewLine +
                             "Mergable: {2}", quantity, stackLimit, merging);
    }
}
