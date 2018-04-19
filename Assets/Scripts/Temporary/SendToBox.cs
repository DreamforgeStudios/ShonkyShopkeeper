using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendToBox : MonoBehaviour {
    public GameObject where;
    public float speed;

    private Vector3 fromTo;
    private bool stop;

	// Use this for initialization
	void Start () {
        stop = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (!stop)
            this.GetComponent<Rigidbody>().MovePosition(Vector3.MoveTowards(this.transform.position, where.transform.position, speed));
	}

    private void OnTriggerExit(Collider other) {
        stop = true;
    }
}
