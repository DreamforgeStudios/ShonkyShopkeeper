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

	public float ScaleOverride;


	private GameObject spawnedClone;

	// Use this for initialization
	void Start() {
		spawnedClone = Instantiate(Ore, Ore.transform.position, Ore.transform.rotation, transform);
		spawnedClone.transform.DOLocalMove(Vector3.zero, InitialTweenDuration).SetEase(Ease.InBack);
	}

	public void Upgrade(bool success) {
		if (success)
		{
			var spawn = Instantiate(Brick, Brick.transform.position, Brick.transform.rotation, transform);

			var a = Instantiate(SmokeParticleSystem, spawn.transform.position + Vector3.back * 1.5f,
				Quaternion.identity, transform);
			a.transform.localScale = new Vector3(ScaleOverride, ScaleOverride, ScaleOverride);

			a = Instantiate(ShineParticleSystem, spawn.transform.position + Vector3.forward * 1.5f, Quaternion.identity,
				transform);
			a.transform.localScale = new Vector3(ScaleOverride, ScaleOverride, ScaleOverride);

			spawnedClone.SetActive(false);
			SmeltingPot.SetActive(false);
		}
		else
		{
			var a = Instantiate(SmokeParticleSystem, Brick.transform.position + Vector3.back * 1.5f,
				Quaternion.identity, transform);
			a.transform.localScale = new Vector3(ScaleOverride, ScaleOverride, ScaleOverride);
			spawnedClone.SetActive(false);
			SmeltingPot.SetActive(false);
		}
	}
}
