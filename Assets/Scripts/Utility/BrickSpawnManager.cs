﻿using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class BrickSpawnManager : MonoBehaviour {
	//public float InitialTweenDuration;
	
	public GameObject Brick;
	public GameObject Shell;
	
	public GameObject ShineParticleSystem;
	public GameObject SmokeParticleSystem;

	public float ScaleOverrideShine;
	public float ScaleOverrideSmoke;


	//private GameObject spawnedClone;

	// Use this for initialization
	void Start() {
		//spawnedClone = Instantiate(Ore, Ore.transform.position, Ore.transform.rotation, transform);
		//spawnedClone.transform.DOMove(Vector3.zero, InitialTweenDuration).SetEase(Ease.InBack);
	}

	public void Upgrade() {
		// Workaround for orthographic camera in tracing scene.....
		var clone = Instantiate(SmokeParticleSystem, transform.position + Vector3.back * 2.5f, Quaternion.identity, transform);
		clone.transform.localScale = new Vector3(ScaleOverrideSmoke, ScaleOverrideSmoke, ScaleOverrideSmoke);
		
		clone = Instantiate(ShineParticleSystem, transform.position + Vector3.forward * 2.5f, Quaternion.identity, transform);
		clone.transform.localScale = new Vector3(ScaleOverrideShine, ScaleOverrideShine, ScaleOverrideShine);
		
		//spawnedClone.SetActive(false);
		//SmeltingPot.SetActive(false);
		
		Instantiate(Shell, Shell.transform.position, Shell.transform.rotation, transform);
	}
}
