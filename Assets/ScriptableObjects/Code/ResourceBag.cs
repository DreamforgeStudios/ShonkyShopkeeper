﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Items/ResourcePouch", fileName = "ResourcePouch.asset")]
public class ResourceBag : Item {
    public enum GemType {
        Ruby, Amethyst, Sapphire, Emerald
    }
    
    public GemType gemType;
}
