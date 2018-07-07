using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GolemPickup : MonoBehaviour {
    //Currently held golem
    private GameObject pickedupGolem;
    //List of returning golems
    List<GameObject> golems = new List<GameObject>();

    //Grabbing Variables
    private Vector3 modifiedMousePos;
    private Vector3 mousePos;
    private bool overPortal = false;

    //Exit Portal Location - used for 'respawning' returning golems
    public GameObject exitPortal;
    private Vector3 exitPortalLocation;

    // Use this for initialization
    void Start() {
        exitPortalLocation = exitPortal.transform.position;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButton(0)) {
            GolemGrab();
        }
        else if (overPortal) {
            Debug.Log("Sending to Mine");
            Mine.AddGolemAndTime(System.DateTime.Now,pickedupGolem);
            pickedupGolem.SetActive(false);
            pickedupGolem = null;
            overPortal = false;
        }
        else if (pickedupGolem != null){
            ResetGolem();
        }
    }

    private void GolemGrab() {
        Debug.Log("Casting Ray");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 20)) {
            if (hit.transform.gameObject.tag == "Golem") {
                GameManager.pickedUpGolem = true;
                pickedupGolem = hit.transform.gameObject;
                HoldGolem(hit);
            } else if (Mine.ReadyToCollect() && hit.transform.gameObject.tag == "PortalExit") {
                golems = Mine.ReturnReadyGolems();
                foreach(GameObject golem in golems) {
                    ReturnGolem(golem);
                }
            }
        }
    }

    private void ResetGolem() {
        if (pickedupGolem != null) {
            pickedupGolem.GetComponent<NavMeshAgent>().enabled = true;
            GameManager.pickedUpGolem = false;
            pickedupGolem.GetComponent<ShonkyWander>().pickedUp = false;
            pickedupGolem = null;
        }
    }
    //This method will eventually be used to give returning golems resource pouches
    private void ReturnGolem(GameObject golem) {
        golem.transform.position = exitPortalLocation;
        golem.SetActive(true);
        golem.GetComponent<NavMeshAgent>().enabled = true;
        golem.GetComponent<ShonkyWander>().pickedUp = false;

    }

    private void HoldGolem(RaycastHit hit) {
        pickedupGolem.GetComponent<ShonkyWander>().pickedUp = true;
        pickedupGolem.GetComponent<NavMeshAgent>().enabled = false;
        modifiedMousePos = Camera.main.ScreenPointToRay(Input.mousePosition).GetPoint(6.5f);
        pickedupGolem.transform.position = modifiedMousePos;
        CheckIfOverPortal();
    }

    private void CheckIfOverPortal() {
        //Continue to raycast to check if it is over entry portal
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit secondHit;
        int layerMask = LayerMask.GetMask("MinePortal");
        if (Physics.Raycast(ray, out secondHit, 10, layerMask)) {
            overPortal = true;
        }
        else {
            overPortal = false;
        }
        Debug.Log(overPortal + " is over portal");
    }
}
