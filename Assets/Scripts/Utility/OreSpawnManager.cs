using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class OreSpawnManager : MonoBehaviour {
	public float InitialTweenDuration;
	
	public GameObject Ore;
	public GameObject Brick;
	public GameObject SmeltingPot;
	
	public GameObject ShineParticleSystem;
	public GameObject SmokeParticleSystem;


	private GameObject spawnedClone;

	// Use this for initialization
	void Start() {
		spawnedClone = Instantiate(Ore, Ore.transform.position, Ore.transform.rotation, transform);
		spawnedClone.transform.DOMove(Vector3.zero, InitialTweenDuration).SetEase(Ease.InBack);
	}

	public void Upgrade() {
		Instantiate(SmokeParticleSystem, transform.position + Vector3.back * 2.5f, Quaternion.identity, transform);
		Instantiate(ShineParticleSystem, transform.position + Vector3.forward * 2.5f, Quaternion.identity, transform);
		
		spawnedClone.SetActive(false);
		SmeltingPot.SetActive(false);
		
		Instantiate(Brick, Brick.transform.position, Brick.transform.rotation, transform);
	}
}
