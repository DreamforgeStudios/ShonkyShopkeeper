using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class NPCSpawner : MonoBehaviour {
	// The points where NPCS should spawn from.
	public Vector3[] spawns;
	// The direction that the NPC should walk in.
	public int[] spawnDirections;
	// Potential NPC gameobjects to spawn.
	public GameObject[] potentialSpawns;
	// How fast should each NPC move?
	public float walkSpeed;

	// A random scale will be chosen between these two values (inclusive - exclusive).
	public float scaleMin;
	public float scaleMax;

	// Time between spawns.
	// Alter this from another class to increase / decrease spawn rate.
	public float spawnInterval;
	// How long should the NPC walk for before they're hidden?
	public float walkTime;
	private float timer;

	// For drawing gizmos.
	public bool debug = true;

	[ShowNonSerializedField]
	public const int POOL_SIZE = 4;
	private GameObject[] spawnPool;

	void Start() {
		spawnPool = new GameObject[POOL_SIZE];
		GameObject clone;
		for (int i = 0; i < spawnPool.Length; i++) {
			clone = Instantiate(DetermineNPCToSpawn());
			clone.SetActive(false);
			spawnPool[i] = clone;
		}
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		// Instantiate an NPC and set them walking.
		if (timer > spawnInterval) {
			// Find a clone to use.
			GameObject clone = null;
			for (int i = 0; i < spawnPool.Length; i++) {
				if (!spawnPool[i].activeSelf) {
					clone = spawnPool[i];
					break;
				}
			}

			// No object is available at this time.
			if (clone == null)
				return;
			
			clone.SetActive(true);
			int positionToSpawn = Random.Range(0, spawns.Length);
			clone.transform.position = spawns[positionToSpawn];
			
			NPCWalker walker = clone.GetComponent<NPCWalker>();
			walker.SetWalkDirection(spawnDirections[positionToSpawn]);
			walker.SetWalkSpeed(walkSpeed);
			walker.SetScale(Random.Range(scaleMin, scaleMax));

			StartCoroutine(HideAfterSeconds(clone, walkTime));

			timer = 0f;
		}
	}

	public void SetSpawnInterval(float interval) {
		this.spawnInterval = interval;
	}

	private void OnDrawGizmos() {
		if (!debug) {
			return;
		}

		for (int i = 0; i < spawns.Length; i++) {
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireCube(spawns[i], Vector3.one);
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(spawns[i], spawns[i] + new Vector3(spawnDirections[i] * 5, 0, 0));
		}
	}

    private GameObject DetermineNPCToSpawn() {
        switch (Travel.ReturnCurrentTown()) {
            case Travel.Towns.WickedGrove:
                return potentialSpawns[0];
            case Travel.Towns.FlamingPeak:
                return potentialSpawns[2];
            case Travel.Towns.GiantsPass:
                return potentialSpawns[1];
            case Travel.Towns.SkyCity:
                return potentialSpawns[3];
            default:
                return potentialSpawns[0];
        }
    }

	IEnumerator HideAfterSeconds(GameObject obj, float seconds) {
		yield return new WaitForSeconds(seconds);
		obj.SetActive(false);
	}
}