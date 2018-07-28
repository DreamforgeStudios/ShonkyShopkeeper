using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TracingDataBase : MonoBehaviour
{

	//These Runes will need a specific structure. Currently it is
	// tracing colliders, Sprite, Points to hit.
	public GameObject Rune1, Rune2, Rune3, Rune4, Rune5;
	private List<GameObject> _allRunes;
	
	// Use this for initialization
	void Start () {
		AddAllRunes();
	}

	public void ActivateSpecificRune(int runeIndex)
	{
		for (int i = 0; i < _allRunes.Count; i++)
		{
			if (i != runeIndex)
			{
				_allRunes[i].SetActive(false);
			}
			else
			{
				_allRunes[i].SetActive(true);
				RotateRune(runeIndex);
			}
		}
	}

	public GameObject GetRandomRune()
	{
		int rand = Random.Range(0, _allRunes.Count);
		ActivateSpecificRune(rand);
		return _allRunes[rand];
	}

	private void AddAllRunes()
	{
		_allRunes = new List<GameObject>()
		{
			Rune1,
			Rune2
		};

		/*
		_allRunes.Add(Rune3);
		_allRunes.Add(Rune4);
		_allRunes.Add(Rune5);
		*/
	}
	
	private void RotateRune(int runeIndex)
	{
		float randomRot = Random.Range(-40f, 136f);
		_allRunes[runeIndex].transform.eulerAngles = new Vector3(0,0,randomRot);
	}
	
}
