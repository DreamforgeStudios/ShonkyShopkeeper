using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : Item {

    private ItemType type;
    private Quality quality;
    private GemType gem;

    public Gem(GemType gemType) {
        type = ItemType.Gem;
        quality = Quality.NotGraded;
        gem = gemType;
        quantity = 1;
        stackLimit = 1;
        merging = false;
    }
}
