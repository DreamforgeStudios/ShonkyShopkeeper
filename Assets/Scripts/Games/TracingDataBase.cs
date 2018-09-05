using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TracingDataBase : MonoBehaviour
{

	//These Runes will need a specific structure. Currently it is
	// tracing colliders, Sprite, Points to hit.
	public GameObject Rune1, Rune2, Rune3, Rune4, Rune5;
	private List<GameObject> _allRunes;
	private SpriteRenderer _spriteRenderer;
	
	//These are all the rune effects Justin has made and the relevant UI object
	public RawImage runeEffect;
	public Texture effect1, effect2, effect3, effect4, effect5;
	private List<Texture> _allEffects;
	
	
	// Use this for initialization
	void Start () {
		AddAllRunes();
		AddAllEffects();
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
				ModifyAlpha(runeIndex);
				RotateRune(runeIndex);
				runeEffect.texture = _allEffects[runeIndex];
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

	public void HideUI()
	{
		_spriteRenderer.enabled = false;
		runeEffect.enabled = false;
	}

	private void AddAllEffects()
	{
		_allEffects = new List<Texture>()
		{
			effect1,
			effect2
		};
		//effect3, effect4, effect5
	}
	
	private void RotateRune(int runeIndex)
	{
		float randomRot = Random.Range(0f, 360f);
		_allRunes[runeIndex].transform.localEulerAngles = new Vector3(0,0,randomRot);
	}

	private void ModifyAlpha(int runeIndex)
	{
		_spriteRenderer = _allRunes[runeIndex].transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>();
		var alpha = _spriteRenderer.color;
		alpha.a = 0.1f;
		_spriteRenderer.color = alpha;
	}
	
}
