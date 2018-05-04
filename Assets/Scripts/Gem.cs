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

    public override string ItemName() {
        switch (gem) {
            case GemType.Ruby:
                return "Ruby";
            case GemType.Sapphire:
                return "Sapphire";
            case GemType.Emerald:
                return "Emerald";
            case GemType.Diamond:
                return "Diamond";
            default:
                return "";
        }
    }

    public override string ItemInfo() {
        return string.Format("Quantity: {0}" + System.Environment.NewLine +
					         "Stack Limit: {1}" + System.Environment.NewLine +
                             "Mergable: {2}", quantity, stackLimit, merging);
    }
}