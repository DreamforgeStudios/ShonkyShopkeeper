using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
This should eventually replace item/brick/jewel/charged jewel/gem/shonky/org.cs
public abstract class Item : MonoBehaviour {
    public enum ItemType {
        Ore, Brick, Shell, Gem, Jewel, ChargedJewel, Shonky
    }
    
    public enum GemType {
        NotGem, Ruby, Diamond, Sapphire, Emerald
    }

    public Quality.QualityGrade qualityGrade;

    public ItemType itemType;
    public GemType gemType;

    public int quantity;
    public int stackLimit;
    public bool merging;

    public abstract string ItemName();
    public abstract string ItemInfo();
}

public class Brick : Item {

    private ItemType type;

    public Brick(Quality.QualityGrade refinedGrade) {
        type = ItemType.Brick;
        qualityGrade = refinedGrade;
        this.gemType = GemType.NotGem;
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

public class ChargedJewel : Item {

    private ItemType type;
    //public GemType gem;

    public ChargedJewel(GemType gemType, Quality.QualityGrade refinedQuality) {
        type = ItemType.ChargedJewel;
        qualityGrade = refinedQuality;
        this.gemType= gemType;
        quantity = 1;
        stackLimit = 1;
        merging = true;
    }

    public override string ItemName() {
        switch (gemType) {
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

public class Gem : Item {

    private ItemType type;
    //private GemType gem;

    public Gem(GemType gemType) {
        type = ItemType.Gem;
        this.gemType = gemType;
        quantity = 1;
        stackLimit = 1;
        merging = false;
    }

    public override string ItemName() {
        switch (gemType) {
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

public class Jewel : Item {

    private ItemType type;
    //private GemType gem;

    public Jewel(GemType gemType, Quality.QualityGrade refinedQuality) {
        type = ItemType.Jewel;
        qualityGrade = refinedQuality;
        this.gemType = gemType;
        quantity = 1;
        stackLimit = 1;
        merging = false;
    }

    public override string ItemName() {
        switch (gemType) {
            case GemType.Ruby:
                return "Cut Ruby";
            case GemType.Sapphire:
                return "Cut Sapphire";
            case GemType.Emerald:
                return "Cut Emerald";
            case GemType.Diamond:
                return "Cut Diamond";
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

public class Ore : Item {

    private ItemType type;

    public Ore() {
        type = ItemType.Ore;
        quantity = 1;
        stackLimit = 3;
        merging = false;
    }

    public Ore(int quantity) {
        type = ItemType.Ore;
        this.quantity = quantity;
        this.gemType = GemType.NotGem;
        stackLimit = 3;
        merging = false;
    }

    public override string ItemName() {
        return "Ore";
    }

    public override string ItemInfo() {
        return string.Format("Quantity: {0}" + System.Environment.NewLine +
					         "Stack Limit: {1}" + System.Environment.NewLine +
                             "Mergable: {2}", quantity, stackLimit, merging);
    }
}

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
        this.gemType = GemType.NotGem;
        quantity = 1;
        stackLimit = 1;
        merging = true;
    }

    public override string ItemName() {
        return "Shonky Shell";
    }

    public override string ItemInfo() {
        string strRune = "";
        switch (rune) {
            case RuneType.Rune1:
                strRune = "Rune1";
                break;
            case RuneType.Rune2:
                strRune = "Rune2";
                break;
        }

        return string.Format("Quality: {0}" + System.Environment.NewLine +
                             "Rune: {1}" + System.Environment.NewLine +
                             "Quantity: {2}" + System.Environment.NewLine +
					         "Stack Limit: {3}" + System.Environment.NewLine +
                             "Mergable: {4}", Quality.GradeToString(qualityGrade), strRune, quantity, stackLimit, merging);
    }
}

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

*/