using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : Item {

    private ItemType type;

    public Brick(Quality.QualityGrade refinedGrade) {
        type = ItemType.Brick;
        qualityGrade = refinedGrade;
        quantity = 1;
        stackLimit = 1;
        merging = false;
    }

}
