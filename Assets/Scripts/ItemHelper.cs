using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHelper : MonoBehaviour {

    //Used to determine correct model
    public Item.ItemType type;
    public Item.GemType gemType;
    public Item.RuneType rune;

    //Object to be returned
    private GameObject correctObject;

    //Various Models for resources
    public GameObject resourcePouch;
    public GameObject ore;
    public GameObject brick;

    //Various models for each type of gem in each stage
    public GameObject gemRuby;
    public GameObject gemSapphire;
    public GameObject gemEmerald;
    public GameObject gemDiamond;

    public GameObject jewelRuby;
    public GameObject jewelSapphire;
    public GameObject jewelEmerald;
    public GameObject jewelDiamond;

    public GameObject chargedJewelRuby;
    public GameObject chargedJewelSapphire;
    public GameObject chargedJewelEmerald;
    public GameObject chargedJewelDiamond;

    //Various models for shells
    public GameObject shellRune1;
    public GameObject shellRune2;
    public GameObject shellRune3;

    //Various models for Shonkys
    public GameObject shonkyRuby;
    public GameObject shonkyEmerald;
    public GameObject shonkyDiamond;
    public GameObject shonkySapphire;

    public GameObject ReturnCorrectGameObject(Item item) {
        type = item.ReturnItemType();
        gemType = item.ReturnGemType();
        rune = item.ReturnRune();

        if (gemType == Item.GemType.NotGem) {
            switch (rune) {
                case Item.RuneType.NoRune:
                    switch (type) {
                        case Item.ItemType.Ore:
                            correctObject = ore;
                            break;
                        case Item.ItemType.Brick:
                            correctObject = brick;
                            break;
                        case Item.ItemType.ResourcePouch:
                            correctObject = resourcePouch;
                            break;
                    }
                    break;
                case Item.RuneType.Rune1:
                    correctObject = shellRune1;
                    break;
                case Item.RuneType.Rune2:
                    correctObject = shellRune2;
                    break;
                case Item.RuneType.Rune3:
                    correctObject = shellRune3;
                    break;
            }
        } else {
            switch (type) {
                case Item.ItemType.Gem:
                    switch (gemType) {
                        case Item.GemType.Ruby:
                            correctObject = gemRuby;
                            break;
                        case Item.GemType.Diamond:
                            correctObject = gemDiamond;
                            break;
                        case Item.GemType.Sapphire:
                            correctObject = gemSapphire;
                            break;
                        case Item.GemType.Emerald:
                            correctObject = gemEmerald;
                            break;
                    }
                    break;
                case Item.ItemType.Jewel:
                    switch (gemType) {
                        case Item.GemType.Ruby:
                            correctObject = jewelRuby;
                            break;
                        case Item.GemType.Diamond:
                            correctObject = jewelDiamond;
                            break;
                        case Item.GemType.Sapphire:
                            correctObject = jewelSapphire;
                            break;
                        case Item.GemType.Emerald:
                            correctObject = jewelEmerald;
                            break;
                    }
                    break;
                case Item.ItemType.ChargedJewel:
                    switch (gemType) {
                        case Item.GemType.Ruby:
                            correctObject = chargedJewelRuby;
                            break;
                        case Item.GemType.Diamond:
                            correctObject = chargedJewelDiamond;
                            break;
                        case Item.GemType.Sapphire:
                            correctObject = chargedJewelSapphire;
                            break;
                        case Item.GemType.Emerald:
                            correctObject = chargedJewelEmerald;
                            break;
                    }
                    break;
                case Item.ItemType.Shonky:
                    switch (gemType) {
                        case Item.GemType.Ruby:
                            correctObject = shonkyRuby;
                            break;
                        case Item.GemType.Diamond:
                            correctObject = shonkyDiamond;
                            break;
                        case Item.GemType.Sapphire:
                            correctObject = shonkySapphire;
                            break;
                        case Item.GemType.Emerald:
                            correctObject = shonkyEmerald;
                            break;
                    }
                    break;
                case Item.ItemType.ResourcePouch:
                    correctObject = resourcePouch;
                    break;
            }
        }
        return correctObject;
    }
}