using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Avatar", fileName = "Avatar.asset")]
public class Avatar : ScriptableObject {
    public Sprite Sprite;
    public Color Color;
    public Item.GemType GemType;
}
