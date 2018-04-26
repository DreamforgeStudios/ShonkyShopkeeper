using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : Item {

    public enum RuneType {
        Rune1, Rune2
    }
    private ItemType type;
    private RuneType rune;

    public Shell(Quality.QualityGrade quality, RuneType rune) {
        type = ItemType.Shell;
        qualityGrade = quality;
        this.rune = rune;
        quantity = 1;
        stackLimit = 1;
        merging = true;
    }
}
