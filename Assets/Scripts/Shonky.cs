using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
[CreateAssetMenu(menuName = "Items/Shonky", fileName = "Shonky.asset")]
public class Shonky : Item {
    //public Quality.QualityGrade quality;
    public GemType type;

    public string ItemName() {
        return "Shonky";
    }

    // Return a string to be used in the UI.
    // TODO: need a way to get item color.
    public string ItemInfo() {
        Color col = Quality.GradeToColor(Quality.QualityGrade.Passable);
        string colString = "<color=#" + ColorUtility.ToHtmlStringRGBA(col) + ">";
        string terminate = "</color>";
        return string.Format("Quality: {0}{1}{2}" + System.Environment.NewLine +
                             "Type: {3}{4}{5}", colString, Quality.QualityGrade.Passable, terminate, colString, type, terminate);
    }

    public override string GetItemInfo() {
        throw new System.NotImplementedException();
    }
}
