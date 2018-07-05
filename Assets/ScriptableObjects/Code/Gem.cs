using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Items/Gem", fileName = "GemName.asset")]
public class Gem : Item {
    public enum GemType {
        Ruby, Diamond, Sapphire, Emerald
    }
    
    public GemType gemType;
}