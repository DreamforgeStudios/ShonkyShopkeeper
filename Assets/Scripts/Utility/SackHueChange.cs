using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SackHueChange : MonoBehaviour
{

	public Color Ruby, Emerald, Sapphire, Amethyst;
	public GameObject CurrentSack;

	public void UpdateCurrentColor(Item.GemType type)
	{
		Debug.Log("Changing colour now");
		switch (type)
		{
			case Item.GemType.Ruby:
				CurrentSack.GetComponent<Renderer>().material.color = Ruby;
				break;
			case Item.GemType.Amethyst:
				CurrentSack.GetComponent<Renderer>().material.color = Amethyst;
				break;
			case Item.GemType.Emerald:
				CurrentSack.GetComponent<Renderer>().material.color = Emerald;
				break;
			case Item.GemType.Sapphire:
				CurrentSack.GetComponent<Renderer>().material.color = Sapphire;
				break;
		}
		Debug.Log("Color is changed");
	}
}
