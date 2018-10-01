using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class OreSpawnManager : MonoBehaviour {
	public float InitialTweenDuration;
	
	public Vector3 BeforePosition, AfterPosition,
				   SmokePosition, ShinePosition;
	
	public GameObject Ore;
	public GameObject Brick;
	//public GameObject SmeltingPot;
	
	public GameObject ShineParticleSystem;
	public GameObject SmokeParticleSystem;

	public float ScaleOverrideSmoke, ScaleOverrideShine;

	public bool EnableDebug;


	private GameObject spawnedClone;

	// Use this for initialization
	void Start() {
		spawnedClone = Instantiate(Ore, BeforePosition, Ore.transform.rotation, transform);
		spawnedClone.transform.DOLocalMove(Vector3.zero, InitialTweenDuration).SetEase(Ease.InBack);
	}

	public void Upgrade(Quality.QualityGrade grade) {
		if (grade == Quality.QualityGrade.Junk) {
			var a = Instantiate(SmokeParticleSystem, SmokePosition, Quaternion.identity, transform);
			a.transform.localScale = new Vector3(ScaleOverrideSmoke, ScaleOverrideSmoke, ScaleOverrideSmoke);
			spawnedClone.SetActive(false);
			//SmeltingPot.SetActive(false);
		} else {
			Instantiate(Brick, AfterPosition, Brick.transform.rotation, transform);

			var a = Instantiate(SmokeParticleSystem, SmokePosition, Quaternion.identity, transform);
			a.transform.localScale = new Vector3(ScaleOverrideSmoke, ScaleOverrideSmoke, ScaleOverrideSmoke);

			a = Instantiate(ShineParticleSystem, ShinePosition, Quaternion.identity, transform);
			a.transform.localScale = new Vector3(ScaleOverrideShine, ScaleOverrideShine, ScaleOverrideShine);

			spawnedClone.SetActive(false);
			//SmeltingPot.SetActive(false);
			//Debug.Log("Was success" + success);
		}
	}
	
	private void OnDrawGizmos() {
		if (EnableDebug) {
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(BeforePosition, 1);
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(AfterPosition, 1);

			Gizmos.color = Color.magenta;
			Gizmos.DrawWireCube(SmokePosition, Vector3.one);
			Gizmos.DrawWireCube(ShinePosition, Vector3.one);
		}
	}
}
