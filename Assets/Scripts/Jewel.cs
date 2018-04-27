using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jewel : Item {

    private ItemType type;
    private GemType gem;

    public Jewel(GemType gemType, Quality.QualityGrade refinedQuality) {
        type = ItemType.Jewel;
        qualityGrade = refinedQuality;
        gem = gemType;
        quantity = 1;
        stackLimit = 1;
        merging = false;
    }
}
