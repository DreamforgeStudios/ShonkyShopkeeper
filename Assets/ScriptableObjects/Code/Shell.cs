using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Items/Shell", fileName = "Shell.asset")]
public class Shell : Item {
    public enum RuneType {
        Rune1, Rune2, Rune3
    }
    
    public RuneType runeType;
}