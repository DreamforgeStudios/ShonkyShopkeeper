using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargedJewel : Item {

    private ItemType type;
    private Quality quality;
    private GemType gem;

    public ChargedJewel(GemType gemType, Quality refinedQuality) {
        type = ItemType.ChargedJewel;
        quality = refinedQuality;
        gem = gemType;
        quantity = 1;
        stackLimit = 1;
        merging = true;
    }
}
