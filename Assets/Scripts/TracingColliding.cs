using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TracingColliding : MonoBehaviour {
    public int counter = 0;
    public GameObject colliderHit;
    public Image holder;
    public Sprite bad;
    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "TracingCollider") {
            counter++;
            Instantiate(colliderHit, this.transform.position,this.transform.rotation);
                holder.enabled = true;
                holder.sprite = bad;
           
        }
    }

    private void OnCollisionStay(Collision collision) {
        if (collision.gameObject.tag == "TracingCollider") {
            counter++;
            
        }
    }
}
