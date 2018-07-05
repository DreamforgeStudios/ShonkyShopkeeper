using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Items/Charged Jewel", fileName = "Charged Jewel.asset")]
public class ChargedJewel : Item {
    public enum GemType {
        Ruby, Diamond, Sapphire, Emerald
    }
    
    public GemType gemType;
}