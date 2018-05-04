using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shonky : Item {

    public Quality.QualityGrade quality;
    public GemType type;
    public ItemType itemType = ItemType.Shonky;

    // Return a string to be used in the UI.
    // TODO: need a way to get item color.
    public string GetInfoString() {
        Color col = Quality.GradeToColor(quality);
        string colString = "<color=#" + ColorUtility.ToHtmlStringRGBA(col) + ">";
        string terminate = "</color>";
        return string.Format("Quality: {0}{1}{2}" + System.Environment.NewLine +
					         "Type: {3}{4}{5}", colString, quality, terminate, colString, type, terminate);
    }
}
