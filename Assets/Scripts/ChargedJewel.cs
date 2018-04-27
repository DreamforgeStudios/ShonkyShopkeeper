using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargedJewel : Item {

    private ItemType type;
    private GemType gem;

    public ChargedJewel(GemType gemType, Quality.QualityGrade refinedQuality) {
        type = ItemType.ChargedJewel;
        qualityGrade = refinedQuality;
        gem = gemType;
        quantity = 1;
        stackLimit = 1;
        merging = true;
    }
}
