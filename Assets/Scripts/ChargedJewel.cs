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

    public override string ItemName() {
        switch (gem) {
            case GemType.Ruby:
                return "Charged Ruby";
            case GemType.Sapphire:
                return "Charged Sapphire";
            case GemType.Emerald:
                return "Charged Emerald";
            case GemType.Diamond:
                return "Charged Diamond";
            default:
                return "";
        }
    }

    public override string ItemInfo() {
        return string.Format("Quality: {0}" + System.Environment.NewLine +
                             "Quantity: {1}" + System.Environment.NewLine +
					         "Stack Limit: {2}" + System.Environment.NewLine +
                             "Mergable: {3}", Quality.GradeToString(qualityGrade), quantity, stackLimit, merging);
    }
}
