using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GolemPickup : MonoBehaviour {
    private GameObject pickedupGolem;
    private Vector3 modifiedMousePos;
    private Vector3 mousePos;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(0)) {
           Debug.Log("Casting Ray");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 20)) {
                //Debug.Log(modifiedMousePos + "hit point is " + hit.point);
                //Debug.Log("hit");
                if (hit.transform.gameObject.tag == "Golem") {
                    GameManager.pickedUpGolem = true;
                    pickedupGolem = hit.transform.gameObject;
                    HoldGolem(hit);
                } else {
                    ResetGolem();
                }               
            } else {
                ResetGolem();
            }
        } else {
            ResetGolem();
        }
    }

    private void ResetGolem() {
        if (pickedupGolem != null) {
            pickedupGolem.GetComponent<NavMeshAgent>().enabled = true;
            GameManager.pickedUpGolem = false;
            pickedupGolem = null;
        }
    }

    private void HoldGolem(RaycastHit hit) {
        pickedupGolem.GetComponent<ShonkyWander>().pickedUp = true;
        pickedupGolem.GetComponent<NavMeshAgent>().enabled = false;
        //modifiedMousePos = Input.mousePosition;
        //modifiedMousePos.z = pickedupGolem.transform.position.z - Camera.main.transform.position.z;
        //modifiedMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        modifiedMousePos = Camera.main.ScreenPointToRay(Input.mousePosition).GetPoint(6.5f);
        //modifiedMousePos.z = hit.point.z;
        pickedupGolem.transform.position = modifiedMousePos;
        Debug.Log(modifiedMousePos);
    }
}
