using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items", fileName = "NewItem.asset")]
[System.Serializable]
public class Item : ScriptableObject {
    //Enum used to determine item type
    public enum ItemType {
        Ore, Brick, Shell, Gem, Jewel, ChargedJewel, Shonky, ResourcePouch
    }
    //Enum used for Gem/Jewel/Charged/Shonky 
    public enum GemType {
        NotGem, Ruby, Diamond, Sapphire, Emerald
    }
    //Enum used for Shells to determine final shonky design
    public enum RuneType {
        NoRune, Rune1, Rune2, Rune3
    }

    //Various attributes of Items
    [SerializeField]
    private GameObject physicalRepresentation;
    [SerializeField]
    private Quality.QualityGrade qualityGrade;
    [SerializeField]
    private ItemType itemType;
    [SerializeField]
    private GemType gemType;
    [SerializeField]
    private RuneType runeType;
    [SerializeField, Range(0, 3)]
    private int quantity;
    [SerializeField, Range(1, 3)]
    private int stackLimit;
    [SerializeField]
    private bool merging;
    [SerializeField, Multiline]
    private string description;

    private ItemHelper helper = new ItemHelper();

    //Constructor used to create items, especially for inventory
    public Item(ItemType itemType, GemType gemType, RuneType rune, Quality.QualityGrade grade, int quantity, int stackLimit, bool canMerge) {
        this.itemType = itemType;
        this.gemType = gemType;
        this.runeType = rune;
        qualityGrade = grade;
        this.quantity = quantity;
        this.stackLimit = stackLimit;
        merging = canMerge;
        physicalRepresentation = helper.ReturnCorrectGameObject(this);
            }

    public void SetGameObject(GameObject representation) {
        physicalRepresentation = representation;
    }
    public string ItemName() {
        if (this.gemType == GemType.NotGem) {
            return itemType.ToString();
        } else {
            return (gemType.ToString() + " " + itemType.ToString());
        }
    }

    public string ItemInfo() {
        if (this.gemType == GemType.NotGem) {
            description = itemType.ToString() + " with a " + Quality.GradeToString(qualityGrade) + " quality";
        }
        else {
            description = (gemType.ToString() + " " + itemType.ToString() + " with a " + Quality.GradeToString(qualityGrade) + " quality");
        }
        return description;
    }

    public string QualityString() {
        return this.qualityGrade.ToString();
    }

    public Quality.QualityGrade ReturnQuality() {
        return qualityGrade;
    }

    public ItemType ReturnItemType() {
        return itemType;
    }

    public GemType ReturnGemType() {
        return gemType;
    }

    public RuneType ReturnRune() {
        return runeType;
    }

    public int Quantity() {
        return quantity;
    }

    public Quality.QualityGrade ItemQuality() {
        return qualityGrade;
    }
}