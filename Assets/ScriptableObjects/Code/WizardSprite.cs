using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Wizard Sprite", fileName = "WizardSprite.asset")]
public class WizardSprite : ScriptableObject {
	public string Name;
	public Sprite Front, Side, Best, Good, OK, Bad;
}