using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteraction : MonoBehaviour {

    //Variables related to NPC interaction
    private int numberOfPlayerShonkys;
    public float baseChanceOfApproach = 30.0f;
    private int numberOfInteractionsLately;
    public int maxInteractions = 5;

    //Variables related to numberOfInteractions
    private float nextResetTime;
    public float cooldown = 10.0f;

    private GameObject[] penShonkys;
    System.Random generator;

    // Use this for initialization
    void Start () {
        numberOfInteractionsLately = 0;
        penShonkys = GameObject.FindGameObjectsWithTag("Shonky");
        generator = new System.Random();
        nextResetTime = Time.time + cooldown;
    }
	
	// Update is called once per frame
	void Update () {
        CheckForInput();
        RefreshInteractions();
        
	}

    private void RefreshInteractions() {
        if (Time.time > nextResetTime) {
            nextResetTime = Time.time + cooldown;
            numberOfInteractionsLately -= 1;
        }
    }

    private void CheckForInput() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //Debug.Log(Camera.main.ScreenPointToRay(Input.mousePosition));
            RaycastHit hit;
            Debug.DrawRay(ray.origin, ray.direction * 10, Color.red, 5f);
            if (Physics.Raycast(ray, out hit, 100)) {
                //Debug.Log(hit.transform.gameObject.name);
                if (hit.transform.gameObject.tag == "NPC") {
                    int newRandom = generator.Next(0, 100);
                    Debug.Log(newRandom);
                    if (newRandom <= baseChanceOfApproach && numberOfInteractionsLately <= maxInteractions) {
                        Debug.Log("Hit NPC and successfully got them to approach");
                        //Add when keeping list of shonkys
                        //int randomShonkyIndex = generator.Next(0, penShonkys.Length - 1);
                        //Begin Barter Game with the shonky at penShonkys[randomShonkyIndex];
                        numberOfInteractionsLately += 1;
                    } else {
                        //Debug.Log("Hit NPC but did not approach ");
                    }
                }
            }
        }
    }
}
