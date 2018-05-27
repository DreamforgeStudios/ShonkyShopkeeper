using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class NPCInteraction : MonoBehaviour {

    //Variables related to NPC interaction
    private int numberOfPlayerShonkys;
    public float baseChanceOfApproach = 30.0f;
    private int numberOfInteractionsLately;
    public int maxInteractions = 5;
    public GameObject shopFront;
    private Vector3 shopFrontPos;

    //Variables related to numberOfInteractions
    private float nextResetTime;
    public float cooldown = 10.0f;


    private GameObject[] penShonkys;
    System.Random generator;

    // Use this for initialization
    void Start() {
        numberOfInteractionsLately = 0;
        penShonkys = GameObject.FindGameObjectsWithTag("Shonky");
        generator = new System.Random();
        nextResetTime = Time.time + cooldown;
        shopFrontPos = shopFront.transform.position;
    }

    // Update is called once per frame
    void Update() {
        CheckForInput();
        //RefreshInteractions();

    }
    /*
    private void RefreshInteractions() {
        if (Time.time > nextResetTime) {
            nextResetTime = Time.time + cooldown;
            numberOfInteractionsLately -= 1;
        }
    }
    */

    private void CheckForInput() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //Debug.Log(Camera.main.ScreenPointToRay(Input.mousePosition));
            RaycastHit hit;
            Debug.DrawRay(ray.origin, ray.direction * 10, Color.red, 5f);
            if (Physics.Raycast(ray, out hit, 100)) {
                //Debug.Log(hit.transform.gameObject.name);
                if (hit.transform.gameObject.tag == "NPC") {
                    //Need to determine if the player has any shonkys and what indexes they are at
                    List<int> shonkyIndexes = ShonkyInventory.Instance.PopulatedShonkySlots();
                    //If they do, select a random one and pass to the barter screen
                    Debug.Log(shonkyIndexes.Count);
                    if (shonkyIndexes.Count >= 0) {
                        DataTransfer.currentSprite = hit.transform.GetComponent<SpriteRenderer>().sprite;
                        DataTransfer.currentPersonality = hit.transform.GetComponent<NPCWalker>().personality;

                        int randomShonky = shonkyIndexes[UnityEngine.Random.Range(0, shonkyIndexes.Count)];
                        ItemInstance chosenShonky;
                        if (ShonkyInventory.Instance.GetItem(randomShonky, out chosenShonky)) {
                            DataTransfer.shonkyIndex = randomShonky;

                            //Move NPC to shop front and initiate barter
                            hit.transform.GetComponent<NPCWalker>().walkNormal = false;
                            hit.transform.DOScale(1.2f, 2f);
                            hit.transform.DOMove(shopFrontPos, 2f, false).OnComplete(() => SceneManager.LoadScene("Barter"));
                        }
                    }
                }
            }
        }
    }
}
