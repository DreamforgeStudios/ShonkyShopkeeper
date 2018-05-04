using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shonky : Item {

    //public Quality.QualityGrade quality;
    public GemType type;
    public ItemType itemType = ItemType.Shonky;

    public override string ItemName() {
        return "Shonky";
    }

    // Return a string to be used in the UI.
    // TODO: need a way to get item color.
    public override string ItemInfo() {
        Color col = Quality.GradeToColor(qualityGrade);
        string colString = "<color=#" + ColorUtility.ToHtmlStringRGBA(col) + ">";
        string terminate = "</color>";
        return string.Format("Quality: {0}{1}{2}" + System.Environment.NewLine +
					         "Type: {3}{4}{5}", colString, qualityGrade, terminate, colString, type, terminate);
    }
}
