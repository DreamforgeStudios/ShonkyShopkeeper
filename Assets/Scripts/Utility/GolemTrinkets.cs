using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GolemTrinkets : MonoBehaviour
{

	//Trinkets
	public GameObject Ball;
	
	//Interaction variables
	public bool interactionMaster;
	public bool _interaction = false;
	private bool currentGolem = false;
	private float _cooldown = 10.0f;
	private float interactionLimit = 6.0f;
	private float _nextKick;

	//Used to store 'free' golems
	private List<GameObject> golems;
	private GameObject _chosenGolem;
	
	//Used to query for 'free' golems
	public PhysicalShonkyInventory GolemInv;
	
	// Use this for initialization
	void Start ()
	{
		_nextKick = Time.time + _cooldown;
	}
	
	// Update is called once per frame
	void Update()
	{
		if (interactionMaster)
		{
			if (currentGolem)
				_interaction = _chosenGolem.GetComponent<ShonkyWander>().TrinketInteraction();
			//Debug.Log("Next kick is " + _nextKick + " and time is " + Time.time + " and ball interaction is " + _interaction);
			if (_nextKick < Time.time && !_interaction)
			{
				if (GetActiveGolems())
				{
					_chosenGolem = ChooseRandomGolem();
					//Debug.Log("Start interaction");
					InteractWithItem();
				}
			}
		}
	}

	//Updates the list of golems that are 'free' to interact with objects
	//They must not be in the mine and cannot be currently picked up
	private bool GetActiveGolems()
	{
		golems = new List<GameObject>();
		List<int> indexes = new List<int>();
		indexes = ShonkyInventory.Instance.PopulatedShonkySlots();
		for (int i = 0; i < indexes.Count; i++)
		{
			ItemInstance item;
			if (ShonkyInventory.Instance.GetItem(indexes[i], out item))
				if (!item.InMine)
				{
					GameObject obj;
					if (GolemInv.shonkySlots[i].GetPrefabInstance(out obj))
						if (!obj.GetComponent<ShonkyWander>().pickedUp)
							golems.Add(obj);
				}
		}

		return (golems.Count >= 1);
	}

	private GameObject ChooseRandomGolem()
	{
		var rand = Random.Range(0, golems.Count - 1);
		return golems[rand];
	}

	private void InteractWithItem()
	{
		_interaction = true;
		_chosenGolem.GetComponent<ShonkyWander>().InteractWithBall(Ball);
		currentGolem = true;
		_nextKick = Time.time + _cooldown;
		//CheckForEnd();

	}

	/*
	private void CheckForEnd()
	{
		_interaction = _chosenGolem.GetComponent<ShonkyWander>().TrinketInteraction();
		if (!_interaction)
			_nextKick = Time.time + _cooldown;
	}
	*/
}
