using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[ExecuteInEditMode]
public class ParticleDebug : MonoBehaviour {
	public List<ParticleSystem> ParticleSystems;

	// Use this for initialization
	void Start () {
		ParticleSystems.AddRange(GetComponentsInChildren<ParticleSystem>());
	}

	[Button("Execute")]
	private void RunParticle() {
		foreach (ParticleSystem p in ParticleSystems) {
			p.Play();
		}
	}
}
