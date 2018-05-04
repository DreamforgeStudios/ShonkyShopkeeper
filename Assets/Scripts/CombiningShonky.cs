using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombiningShonky : MonoBehaviour {
    //Selection holders
    private GameObject lastSelected;
    private Item.ItemType item;
    private GameObject combiner;
    public GameObject shonky;

    //Movement variables
    private bool moving = false;
    private bool inTransition;
    private Vector3 midPoint;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        CheckForInput();
        if (moving)
            MoveItemsTogether(lastSelected,combiner);
	}

    private void CheckForInput() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Debug.DrawRay(ray.origin, ray.direction * 10, Color.red, 5f);
            if (Physics.Raycast(ray, out hit, 100)) {
                if (hit.transform.gameObject && lastSelected == null) {
                    lastSelected = hit.transform.gameObject;
                    item = lastSelected.GetComponent<Item>().itemType;
                } else if (hit.transform.gameObject && lastSelected != null) {
                    if ((item == Item.ItemType.Shell && hit.transform.gameObject.GetComponent<Item>().itemType == Item.ItemType.ChargedJewel) ||
                        (item == Item.ItemType.ChargedJewel && hit.transform.gameObject.GetComponent<Item>().itemType == Item.ItemType.Shell)) {
                        combiner = hit.transform.gameObject;
                        moving = true;
                    } else {
                        lastSelected = hit.transform.gameObject;
                        item = lastSelected.GetComponent<Item>().itemType;
                    }
                }
            }
        }
    }

    private void MoveItemsTogether(GameObject a, GameObject b) {
        if (!inTransition) {
            Vector3 pos1 = a.transform.position;
            Vector3 pos2 = b.transform.position;
            midPoint = (pos1 + pos2) / 2f;
            inTransition = true;
        }

        if (inTransition) {
            a.transform.position = Vector3.MoveTowards(a.transform.position, midPoint, 1.0f);
            b.transform.position = Vector3.MoveTowards(b.transform.position, midPoint, 1.0f);
        }

        if (a.transform.position == midPoint && b.transform.position == midPoint) {
            inTransition = false;
            moving = false;
            lastSelected = null;
            combiner = null;
            Instantiate(shonky, a.transform.position, a.transform.rotation);
            Destroy(a, 2.0f);
            Destroy(b, 2.0f);
            
        }
    }
}
