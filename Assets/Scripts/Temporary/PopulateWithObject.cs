using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulateWithObject : MonoBehaviour {
    public GameObject obj;
    public int number;
    public float delay;

    private float timer;
    private int spawned;

	// Use this for initialization
	void Start () {
        timer = 0f;
        spawned = 0;
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if (timer > delay && spawned < number) {
            timer = 0f;
            spawned++;
            GameObject clone = Instantiate(obj, transform);
            transform.GetComponent<Move>().AddToCollection(clone.GetComponent<Rigidbody>());
            //clone.transform.parent = transform;
        }
		
	}
}
