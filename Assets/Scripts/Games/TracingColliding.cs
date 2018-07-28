using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TracingColliding : MonoBehaviour {
    public int counter = 0;
    public GameObject badFeedback;

    //Need time variables so crosses aren't spammed and kill the game.
    private float cooldown = 5f;
    private float nextTime;
    public void Awake() {
        nextTime = Time.time;
    }
    
    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "TracingCollider") {
            counter++;
            if (Time.time > nextTime) {
                Vector3 newPos = transform.position;
                newPos.z += 1;
                Instantiate(badFeedback, newPos, badFeedback.transform.rotation);
                nextTime = Time.time + cooldown;
            }
        }
    }

    private void OnCollisionStay(Collision collision) {
        if (collision.gameObject.tag == "TracingCollider") {
            counter++;
            Debug.Log(counter);
            if (Time.time > nextTime) {
                Vector3 newPos = transform.position;
                newPos.z += 1;
               // Instantiate(badFeedback, newPos, badFeedback.transform.rotation);
                nextTime = Time.time + cooldown;
            }
        }
    }
}
