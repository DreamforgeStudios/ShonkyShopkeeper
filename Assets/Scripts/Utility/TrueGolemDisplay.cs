using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

public class TrueGolemDisplay : MonoBehaviour
{

	public List<GameObject> trueGolemModels;
	// Use this for initialization
	void Start ()
	{
		ResetGolems();
		List<TrueGolems.TrueGolem> golemsUnlocked = new List<TrueGolems.TrueGolem>(); 
		golemsUnlocked = Inventory.Instance.GetUnlockedTrueGolems();
		//Greater than one because of the nill in enum used for instantiation purposes
		if (golemsUnlocked.Count > 1)
		{
			foreach (TrueGolems.TrueGolem trueGolem in golemsUnlocked)
			{
				switch (trueGolem)
				{
					case TrueGolems.TrueGolem.rubyGolem:
						trueGolemModels[0].SetActive(true);
						break;
					case TrueGolems.TrueGolem.emeraldGolem:
						trueGolemModels[1].SetActive(true);
						break;
					case TrueGolems.TrueGolem.sapphireGolem:
						trueGolemModels[2].SetActive(true);
						break;
					case TrueGolems.TrueGolem.amethystGolem:
						trueGolemModels[3].SetActive(true);
						break;
				}
			}
		}
	}

	private void ResetGolems()
	{
		foreach (GameObject obj in trueGolemModels)
		{
			obj.SetActive(false);
		}
	}
}
