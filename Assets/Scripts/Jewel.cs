using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jewel : Item {

    private ItemType type;
    private Quality quality;
    private GemType gem;

    public Jewel(GemType gemType, Quality refinedQuality) {
        type = ItemType.Jewel;
        quality = refinedQuality;
        gem = gemType;
        quantity = 1;
        stackLimit = 1;
        merging = false;
    }
}
