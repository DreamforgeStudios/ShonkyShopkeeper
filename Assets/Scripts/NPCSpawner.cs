using System.Collections;
using System.Collections.Generic;
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
	private float timer;

	// For drawing gizmos.
	public bool debug = true;
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		// Instantiate an NPC and set them walking.
		if (timer > spawnInterval) {
			GameObject npcToSpawn = potentialSpawns[Random.Range(0, potentialSpawns.Length)];
			int positionToSpawn = Random.Range(0, spawns.Length);
			GameObject clone = Instantiate(npcToSpawn, spawns[positionToSpawn], Quaternion.identity);
			NPCWalker walker = clone.GetComponent<NPCWalker>();

			walker.SetWalkDirection(spawnDirections[positionToSpawn]);
			walker.SetWalkSpeed(walkSpeed);
			walker.SetScale(Random.Range(scaleMin, scaleMax));

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
}
