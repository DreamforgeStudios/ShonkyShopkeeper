using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TracingColliding : MonoBehaviour {
    public int counter = 0;
    public GameObject badFeedback;

    //Need time variables so crosses aren't spammed and kill the game.
    private float cooldown = 1f;
    private float nextTime;
    public void Awake() {
        nextTime = Time.time;
    }

    public void ResetCounter()
    {
        counter = 0;
    }
    
    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "TracingCollider") {
            //Debug.Log("Hit + " + collision.gameObject.name);
            counter++;
            if (Time.time > nextTime) {
                SFX.Play("Tracing_badtouch",1f,1f,0f,false,0f);
                Vector3 newPos = transform.position;
                newPos.z += 1;
                //Instantiate(badFeedback, newPos, badFeedback.transform.rotation);
                nextTime = Time.time + cooldown;
            }
        } else if (collision.gameObject.tag == "Tracing Rune")
        {
            Debug.Log("hitting sprite collider");
        }
    }

    private void OnCollisionStay(Collision collision) {
        if (collision.gameObject.tag == "TracingCollider") {
            counter++;
            //Debug.Log("In collider + " + collision.gameObject.name);
            if (Time.time > nextTime) {
                SFX.Play("Tracing_badtouch",1f,1f,0f,false,0f);
                Vector3 newPos = transform.position;
                newPos.z += 1;
               // Instantiate(badFeedback, newPos, badFeedback.transform.rotation);
                nextTime = Time.time + cooldown;
            }
        }else if (collision.gameObject.tag == "Tracing Rune")
        {
            Debug.Log("hitting sprite collider");
        }
    }
}
