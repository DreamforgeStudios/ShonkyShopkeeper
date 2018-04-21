using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Move : MonoBehaviour {
    public Vector3 direction;
    public bool yes = false;

    private Rigidbody r;
    public List<Rigidbody> rs;

	// Use this for initialization
	void Start () {
        //foreach (GameObject obj in transform.GetComponentsInChildren) {
        //}
        rs.AddRange(transform.GetComponentsInChildren<Rigidbody>());
	}
	
	// Update is called once per frame
	void Update () {
        //transform.position += direction;
        if (yes) {
            foreach(Rigidbody r in rs) {
                r.AddForce(direction);
            }

            yes = false;
        }
	}

    public void AddToCollection(Rigidbody ri) {
        rs.Add(ri);
    }
}
