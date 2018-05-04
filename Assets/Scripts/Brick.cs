﻿using System.Collections;
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

    public override string ItemName() {
        return "Brick";
    }

    public override string ItemInfo() {
        return string.Format("Quality: {0}" + System.Environment.NewLine +
                             "Quantity: {1}" + System.Environment.NewLine +
					         "Stack Limit: {2}" + System.Environment.NewLine +
                             "Mergable: {3}", Quality.GradeToString(qualityGrade), quantity, stackLimit, merging);
    }

}
