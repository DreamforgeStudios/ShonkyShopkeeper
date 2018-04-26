using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : Item {

    private ItemType type;
    private GemType gem;

    public Gem(GemType gemType) {
        type = ItemType.Gem;
        gem = gemType;
        quantity = 1;
        stackLimit = 1;
        merging = false;
    }
}
