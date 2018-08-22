using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemSpawnManager : MonoBehaviour {
	public GameObject RubyBefore, EmeraldBefore, SapphireBefore, AmethystBefore;
	public GameObject RubyAfter, EmeraldAfter, SapphireAfter, AmethystAfter;
	public GameObject ShineParticleSystem;
	public GameObject SmokeParticleSystem;

	[HideInInspector]
	public GameObject Gem {
		get { return spawnedClone; }
	}

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
		Instantiate(SmokeParticleSystem, transform.position + Vector3.back * 2.5f, Quaternion.identity, transform);
		Instantiate(ShineParticleSystem, transform.position + Vector3.forward * 2.5f, Quaternion.identity, transform);
		
		//Destroy(spawnedClone);
		// Setting inactive is faster.  We'll probably pay for the whole destroy cost in loading anyway though.
		spawnedClone.SetActive(false);
		
		Instantiate(cloneAfter, cloneAfter.transform.position, cloneAfter.transform.rotation, transform);

		// TODO: Change gem...
		// Destroy(spawnedClone);
	}
}
