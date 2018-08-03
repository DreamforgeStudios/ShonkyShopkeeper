﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemSpawnManager : MonoBehaviour {
	public GameObject RubyBefore, EmeraldBefore, SapphireBefore, AmethystBefore;
	public GameObject RubyAfter, EmeraldAfter, SapphireAfter, AmethystAfter;
	public GameObject ParticleSystem;

	private GameObject spawnedClone;
	private GameObject cloneAfter;
	
	// Use this for initialization
	void Start () {
		GameObject clone;
		switch (GameManager.Instance.GemTypeTransfer) {
			case (Item.GemType.Ruby): {
				clone = RubyBefore;
				cloneAfter = RubyAfter;
				break;
			}
			case (Item.GemType.Emerald): {
				clone = EmeraldBefore;
				cloneAfter = EmeraldAfter;
				break;
			}
			case (Item.GemType.Sapphire): {
				clone = SapphireBefore;
				cloneAfter = SapphireAfter;
				break;
			}
			case (Item.GemType.Amethyst): {
				clone = AmethystBefore;
				cloneAfter = AmethystAfter;
				break;
			}

			default: {
				clone = RubyBefore;
				cloneAfter = RubyAfter;
				break;
			}
		}

		// Note: using the clone position allows for some more agency in placing gems, but means that the cutting game
		// may use the wrong position.  A fix may be to expose the spawned gem for the cut manager to use.
		spawnedClone = Instantiate(clone, clone.transform.position, clone.transform.rotation, transform);
		//clone.GetComponent<Rotate>().Enable = true;
	}

	public void UpgradeGem() {
		Instantiate(ParticleSystem, transform.position + Vector3.forward * 2.5f, Quaternion.identity, transform);
		
		Destroy(spawnedClone);
		
		Instantiate(cloneAfter, cloneAfter.transform.position, cloneAfter.transform.rotation, transform);

		// TODO: Change gem...
		// Destroy(spawnedClone);
	}
}
