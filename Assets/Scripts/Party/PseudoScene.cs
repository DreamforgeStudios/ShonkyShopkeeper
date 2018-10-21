using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PseudoScene : MonoBehaviour {
	// Insert scene.
	public virtual void Arrive() {
		gameObject.SetActive(true);
	}
	
	// TODO, implement animations for each scene through this.
	// Remove scene.
	public virtual void Depart() {
		gameObject.SetActive(false);
	}
}
