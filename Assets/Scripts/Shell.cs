using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : Item {

    public enum RuneType {
        Rune1, Rune2
    }
    private ItemType type;
    private Quality quality;
    private RuneType rune;

    public Shell(Quality quality, RuneType rune) {
        type = ItemType.Shell;
        this.quality = quality;
        this.rune = rune;
        quantity = 1;
        stackLimit = 1;
        merging = true;
    }
}
