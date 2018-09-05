using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class BrickSpawnManager : MonoBehaviour {
	public Vector3 BeforePosition, AfterPosition,
				   SmokePosition, ShinePosition;
	
	public GameObject Brick;
	public GameObject Shell;
	public GameObject TracingSceneBrick;
	
	public GameObject ShineParticleSystem;
	public GameObject SmokeParticleSystem;

	public float ScaleOverrideShine, ScaleOverrideSmoke;
	public Vector3 RotationOverrideShine, RotationOverrideSmoke;
	
	public bool Debug;

	// Use this for initialization
	void Start() {
		//spawnedClone = Instantiate(Ore, Ore.transform.position, Ore.transform.rotation, transform);
		//spawnedClone.transform.DOMove(Vector3.zero, InitialTweenDuration).SetEase(Ease.InBack);
	}

	public void Upgrade(bool Success) {
		TracingSceneBrick.SetActive(false);

		if (Success) {
			var clone = Instantiate(SmokeParticleSystem, SmokePosition, Quaternion.Euler(RotationOverrideShine), transform);
			clone.transform.localScale = new Vector3(ScaleOverrideSmoke, ScaleOverrideSmoke, ScaleOverrideSmoke);

			clone = Instantiate(ShineParticleSystem, ShinePosition, Quaternion.Euler(RotationOverrideShine), transform);
			clone.transform.localScale = new Vector3(ScaleOverrideShine, ScaleOverrideShine, ScaleOverrideShine);

			clone = Instantiate(Shell, AfterPosition, Shell.transform.rotation, transform);
			//clone.transform.localScale = new Vector3(ScaleOverrideShell, ScaleOverrideShell, ScaleOverrideShell);
		} else {
			var clone = Instantiate(SmokeParticleSystem, SmokePosition, Quaternion.Euler(RotationOverrideSmoke), transform);
			clone.transform.localScale = new Vector3(ScaleOverrideSmoke, ScaleOverrideSmoke, ScaleOverrideSmoke);
			
		}
	}
	
	private void OnDrawGizmos() {
		if (Debug) {
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
